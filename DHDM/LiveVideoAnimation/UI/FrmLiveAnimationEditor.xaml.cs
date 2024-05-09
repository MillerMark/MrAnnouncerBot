using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Imaging;
using ObsControl;
//using InTheHand.Bluetooth;
using System.Security.Policy;
using Newtonsoft.Json;
using System.Windows.Media.Media3D;
using WpfEditorControls;

#if LiveVideoAnimationSandbox
using LiveVideoAnimationSandbox;
#endif

namespace DHDM
{
    /// <summary>
    /// Interaction logic for FrmLiveAnimationEditor.xaml
    /// </summary>
    public partial class FrmLiveAnimationEditor : Window
    {
        public FrmLiveAnimationEditor()
        {
            initializing = true;
            try
            {
                InitializeComponent();
                liveVideoEditor = new LiveVideoEditor();
            }
            finally
            {
                initializing = false;
            }
        }

        public LightingSequence LightingSequence { get; set; }


        List<ObsTransformEdit> allFrames
        {
            get
            {
                return ActiveMovement?.ObsTransformEdits;
            }
        }

        string[] backFiles;
        string[] frontFiles;
        int frameIndex;
        bool settingInternally;
        const double frameRate = 30;  // fps
        const double secondsPerFrame = 1 / frameRate;
        const string STR_EditorPath = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\Editor";
        List<AnimatorWithTransforms> liveFeedAnimators;
        AnimatorWithTransforms activeMovement;

        void UpdateMovementOnTimeline()
        {
            scaleSequenceVisualizer.VisualizeData(activeMovement?.ObsTransformEdits, TotalAnimationFrames, x => x.Scale, GraphStyle.BoxFromBelow);
            opacitySequenceVisualizer.VisualizeData(activeMovement?.ObsTransformEdits, TotalAnimationFrames, x => x.Opacity, GraphStyle.Opacity);
            rotationSequenceVisualizer.VisualizeData(activeMovement?.ObsTransformEdits, TotalAnimationFrames, x => x.Rotation, GraphStyle.BoxFromBelow);
            xSequenceVisualizer.VisualizeData(activeMovement?.ObsTransformEdits, TotalAnimationFrames, x => x.GetX(), GraphStyle.BoxFromBelow);
            ySequenceVisualizer.VisualizeData(activeMovement?.ObsTransformEdits, TotalAnimationFrames, y => y.GetY(), GraphStyle.BoxFromBelow);
            leftLightSequenceVisualizer.VisualizeData(GetLightFromId(BluetoothLights.Left_ID)?.SequenceData, TotalAnimationFrames, x => GetColor(x));
            centerLightSequenceVisualizer.VisualizeData(GetLightFromId(BluetoothLights.Center_ID)?.SequenceData, TotalAnimationFrames, w => GetColor(w));
            rightLightSequenceVisualizer.VisualizeData(GetLightFromId(BluetoothLights.Right_ID)?.SequenceData, TotalAnimationFrames, v => GetColor(v));
        }

        Color GetColor(LightSequenceData x)
        {
            double lightness;
            if (x.Lightness > 0 && x.Lightness < 30)
                lightness = x.Lightness + 30;
            else
                lightness = x.Lightness;

            HueSatLight hsl = new HueSatLight((double)x.Hue / 360, (double)x.Saturation / 100, (double)lightness / 100);
            return hsl.AsRGB;
        }

        AnimatorWithTransforms ActiveMovement
        {
            get
            {
                return activeMovement;
            }

            set
            {
                if (activeMovement == value)
                    return;
                activeMovement = value;
                UpdateMovementOnTimeline();
            }

        }
        int digitCount;
        void LoadAnimation()
        {
            spSourceRadioButtons.Children.Clear();
            ActiveMovementFileName = null;
            List<VideoAnimationBinding> allBindings = AllVideoBindings.GetAll(SelectedSceneName);
            if (allBindings.Count == 0)
                return;
            CreateRadioButtons(allBindings);

            LoadAllFrames(allBindings);
            // TODO: Abort load if path not found.
            LoadAllImages(System.IO.Path.Combine(STR_EditorPath, SelectedSceneName));

            if (frontFiles == null)
                relativePathFront = null;
            else
                relativePathFront = GetRelativePathBaseName(frontFiles[frameIndex]);
            if (backFiles == null)
                relativePathBack = null;
            else
            {
                relativePathBack = GetRelativePathBaseName(backFiles[frameIndex]);
                if (digitCount == 0)
                {
                    int lastIndex = TotalAnimationFrames - 1;
                    digitCount = lastIndex.ToString().Length;
                }
                sldFrameIndex.Maximum = TotalAnimationFrames - 1;
            }
            UpdateUI();
            frameIndex = 0;
            DrawActiveFrame();
            ClearEditorValues();
            UpdateFrameUI();
        }

        private void CreateRadioButtons(List<VideoAnimationBinding> allBindings)
        {
            if (allBindings.Count >= 2)
            {
                spSourceRadioButtons.Visibility = Visibility.Visible;
                foreach (VideoAnimationBinding videoAnimationBinding in allBindings)
                {
                    System.Windows.Controls.RadioButton radioButton = new System.Windows.Controls.RadioButton();
                    radioButton.Margin = new Thickness(15, 0, 0, 0);
                    radioButton.Content = videoAnimationBinding.MovementFileName;
                    radioButton.Click += RadioButton_Click;
                    if (ActiveMovementFileName == null)
                    {
                        ActiveMovementFileName = videoAnimationBinding.MovementFileName;
                        radioButton.IsChecked = true;
                    }

                    spSourceRadioButtons.Children.Add(radioButton);
                }
            }
            else
            {
                spSourceRadioButtons.Visibility = Visibility.Collapsed;
                ActiveMovementFileName = allBindings[0].MovementFileName;
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.RadioButton radioButton)
            {
                ActiveMovementFileName = radioButton.Content as string;
                ActiveMovement = liveFeedAnimators.FirstOrDefault(x => x.MovementFileName == ActiveMovementFileName);
                ClearEditorValues();
                UpdateFrameUI();
            }
        }

        void ClearEditorValues()
        {
            initializing = true;
            try
            {
                tbxDeltaX.Text = "0";
                sldDeltaX.Value = 0;
                tbxDeltaY.Text = "0";
                sldDeltaY.Value = 0;
                tbxDeltaScale.Text = "1";
                sldDeltaScale.Value = 1;
                tbxDeltaRotation.Text = "0";
                sldDeltaRotation.Value = 0;
                tbxDeltaOpacity.Text = "1";
                sldDeltaOpacity.Value = 1;
            }
            finally
            {
                initializing = false;
            }
        }

        string GetRelativePath(string str)
        {
            const string parentFolder = @"\wwwroot\GameDev\Assets\";
            int parentFolderIndex = str.IndexOf(parentFolder);
            if (parentFolderIndex >= 0)
            {
                return str.Substring(parentFolderIndex + parentFolder.Length);
            }
            return null;
        }

        void DrawActiveFrame()
        {
            if (backFiles == null)
                return;
            if (frameIndex < 0 || frameIndex >= TotalAnimationFrames)
            {
                liveVideoEditor.ShowImageBack(null);
                liveVideoEditor.ShowImageFront(null);
                return;
            }

            liveVideoEditor.ShowImageBack(GetRelativePath(backFiles[frameIndex]));
            liveVideoEditor.ShowImageFront(GetRelativePath(frontFiles[frameIndex]));

            foreach (AnimatorWithTransforms animatorWithTransform in liveFeedAnimators)
            {
                DrawFrameSource(animatorWithTransform);
            }

        }

        private void DrawFrameSource(AnimatorWithTransforms animatorWithTransform)
        {
#if LiveVideoAnimationSandbox
            return;
#endif
            ObsTransformEdit liveFeedEdit;
            if (frameIndex >= animatorWithTransform.ObsTransformEdits.Count)
                liveFeedEdit = animatorWithTransform.ObsTransformEdits.Last();
            else
                liveFeedEdit = animatorWithTransform.ObsTransformEdits[frameIndex];

            animatorWithTransform.LiveFeedAnimator.ScreenAnchorLeft = liveFeedEdit.GetX();

            animatorWithTransform.LiveFeedAnimator.ScreenAnchorTop = liveFeedEdit.GetY();

            double rotation = liveFeedEdit.GetRotation();
            double scale = liveFeedEdit.GetScale();
            double opacity = liveFeedEdit.GetOpacity();

            animatorWithTransform.LiveFeedAnimator.SetCamera(liveFeedEdit.Camera);
            ObsControl.ObsManager.SizeAndPositionItem(animatorWithTransform.LiveFeedAnimator, scale, opacity, rotation, liveFeedEdit.Flipped);
        }

        void Change(Attribute attribute, double value)
        {
            if (initializing || allFrames == null || frameIndex >= allFrames.Count)
                return;
            ObsTransformEdit liveFeedEdit = allFrames[frameIndex];
            switch (attribute)
            {
                case Attribute.X:
                    liveFeedEdit.DeltaX = (int)value;
                    break;
                case Attribute.Y:
                    liveFeedEdit.DeltaY = (int)value;
                    break;
                case Attribute.Scale:
                    liveFeedEdit.DeltaScale = value;
                    break;
                case Attribute.Rotation:
                    liveFeedEdit.DeltaRotation = value;
                    break;
                case Attribute.Opacity:
                    liveFeedEdit.DeltaOpacity = value;
                    break;
            }
            DrawActiveFrame();
        }

        public int TotalAnimationFrames => backFiles?.Length ?? 0;


        private void UpdateUI()
        {
            if (backFiles != null)
                tbTotalFrameCount.Text = TotalAnimationFrames.ToString();
        }

        void LoadAllImages(string selectedPath)
        {
            if (!System.IO.Directory.Exists(selectedPath))
                return;
            backFiles = System.IO.Directory.GetFiles(selectedPath, "back*.png");
            frontFiles = System.IO.Directory.GetFiles(selectedPath, "front*.png");
        }

        private LiveFeedAnimator GetLiveFeedAnimator(string movementFileName)
        {
            VideoAnimationBinding binding = AllVideoBindings.Get(movementFileName);
            string fullPathToMovementFile = VideoAnimationManager.GetFullPathToMovementFile(movementFileName);
            VideoFeed[] videoFeeds = AllVideoFeeds.GetAll(binding);
            return VideoAnimationManager.LoadLiveAnimation(fullPathToMovementFile, binding, videoFeeds);
        }

        private void btnLoadAnimation_Click(object sender, RoutedEventArgs e)
        {
            List<string> allLiveAnimationScenes = AllVideoBindings.AllBindings.Select(x => x.SceneName).Distinct().ToList();
            FrmPickScene frmPickScene = new FrmPickScene(allLiveAnimationScenes);
            if (frmPickScene.ShowDialog() == true)
            {
                SelectedSceneName = frmPickScene.SelectedScene;
                PngPath = System.IO.Path.Combine(STR_EditorPath, SelectedSceneName);
                LoadAnimation();
            }

            //FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            //folderBrowserDialog.SelectedPath = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\Editor";
            //if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //	LoadAnimation(folderBrowserDialog.SelectedPath);
            //}
        }

        private static bool FramesAreClose(ObsTransformEdit currentFrame, ObsTransformEdit frame)
        {
            return AreClose(frame.Origin.X, currentFrame.Origin.X, 1) &&
                                                    AreClose(frame.Origin.Y, currentFrame.Origin.Y, 1) &&
                                                    AreClose(frame.Rotation, currentFrame.Rotation, 0.1) &&
                                                    AreClose(currentFrame.Scale, frame.Scale, 0.01) &&
                                                    AreClose(frame.Opacity, currentFrame.Opacity, 0.01);
        }

        private void btnPreviousFrame_Click(object sender, RoutedEventArgs e)
        {
            FrameIndex--;
        }


        void PreloadAroundActiveFrame(int extraFramesCount)
        {
            if (backFiles == null)
                return;
            int startFrame = Math.Max(0, frameIndex - extraFramesCount);
            int lastIndex = TotalAnimationFrames - 1;
            int endFrame = Math.Min(lastIndex, frameIndex + extraFramesCount);

            if (relativePathBack != null)
                liveVideoEditor.PreloadImageBack(relativePathBack, startFrame, endFrame, digitCount);
            if (relativePathFront != null)
                liveVideoEditor.PreloadImageFront(relativePathFront, startFrame, endFrame, digitCount);
        }

        private string GetRelativePathBaseName(string fileName)
        {
            char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string relativePath = GetRelativePath(fileName);
            string baseFileName = System.IO.Path.GetFileNameWithoutExtension(relativePath).TrimEnd(digits);
            string directoryName = System.IO.Path.GetDirectoryName(relativePath);

            return System.IO.Path.Combine(directoryName, baseFileName);
        }

        string FormatFrameIndex(int frameIndex)
        {
            int seconds = frameIndex / 30;
            int frames = frameIndex % 30;
            return $"{seconds:00}:{frames:00}";
        }

        void UpdateFrameIndex(ObsTransformEdit liveFeedEdit)
        {
            tbFrameIndexFromMovementFile.Text = FormatFrameIndex(liveFeedEdit.FrameIndex);
            tbFrameIndexFromMemory.Text = FormatFrameIndex(FrameIndex);
        }

        void UpdateFrameUI()
        {
            if (FramesAreNotGood())
                return;

            ObsTransformEdit liveFeedEdit = allFrames[frameIndex];

            changingInternally = true;
            try
            {
                UpdateFrameIndex(liveFeedEdit);
                //tbxDeltaOpacity.Text = liveFeedEdit.Opacity;
                tbxDeltaX.Text = liveFeedEdit.DeltaX.ToString();
                tbxDeltaY.Text = liveFeedEdit.DeltaY.ToString();
                sldDeltaX.Value = liveFeedEdit.DeltaX;
                sldDeltaY.Value = liveFeedEdit.DeltaY;
                tbxDeltaRotation.Text = liveFeedEdit.DeltaRotation.ToString();
                sldDeltaRotation.Value = liveFeedEdit.DeltaRotation;
                tbxDeltaScale.Text = liveFeedEdit.DeltaScale.ToString();
                sldDeltaScale.Value = liveFeedEdit.DeltaScale;
                tbxDeltaOpacity.Text = liveFeedEdit.DeltaOpacity.ToString();
                sldDeltaOpacity.Value = liveFeedEdit.DeltaOpacity;
                UpdateLights();
                UpdateValuePreviews(liveFeedEdit);
            }
            finally
            {
                changingInternally = false;
            }
        }

        private void UpdateLights()
        {
            if (LightingSequence == null)
                return;
            foreach (Light light in LightingSequence.Lights)
                if (frameIndex < light.SequenceData.Count)
                    SetLightColor(light.ID, light.SequenceData[frameIndex]);
        }

        private bool FramesAreNotGood()
        {
            return allFrames == null || frameIndex < 0 || frameIndex > allFrames.Count - 1;
        }

        private void UpdateValuePreviews(ObsTransformEdit liveFeedEdit = null)
        {
            if (liveFeedEdit == null)
            {
                if (FramesAreNotGood())
                    return;
                liveFeedEdit = allFrames[frameIndex];
            }
            tbCurrentX.Text = liveFeedEdit.Origin.X.ToString();
            tbNewX.Text = liveFeedEdit.GetX().ToString();
            tbCurrentY.Text = liveFeedEdit.Origin.Y.ToString();
            tbNewY.Text = liveFeedEdit.GetY().ToString();
            tbCurrentOpacity.Text = liveFeedEdit.Opacity.ToString();
            tbNewOpacity.Text = liveFeedEdit.GetOpacity().ToString();
            tbCurrentRotation.Text = liveFeedEdit.Rotation.ToString();
            tbNewRotation.Text = liveFeedEdit.GetRotation().ToString();
            tbCurrentScale.Text = liveFeedEdit.Scale.ToString();
            tbNewScale.Text = liveFeedEdit.GetScale().ToString();
        }

        public int FrameIndex
        {
            get { return frameIndex; }
            set
            {
                if (frameIndex == value)
                    return;

                if (backFiles != null && value >= TotalAnimationFrames)
                    value = TotalAnimationFrames - 1;

                if (value < 0)
                    value = 0;

                frameIndex = value;
                DrawActiveFrame();
                UpdateCurrentFrameNumber();
                PreloadAroundActiveFrame(10);
                UpdateFrameUI();
            }
        }
        public string SelectedSceneName { get; set; }
        public string PngPath { get; set; }
        string activeMovementFileName;
        public string ActiveMovementFileName
        {
            get => activeMovementFileName;
            set
            {
                if (activeMovementFileName == value)
                    return;
                activeMovementFileName = value;
                if (string.IsNullOrWhiteSpace(value))
                    Title = $"Live Animation Editor";
                else
                    Title = $"Live Animation Editor - {value}";
            }
        }


        private void btnNextFrame_Click(object sender, RoutedEventArgs e)
        {
            FrameIndex++;
        }

        private void UpdateCurrentFrameNumber()
        {
            settingInternally = true;
            try
            {
                tbxFrameNumber.Text = frameIndex.ToString();
            }
            finally
            {
                settingInternally = false;
            }
        }

        private void tbxFrameNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (settingInternally)
                return;

            if (int.TryParse(tbxFrameNumber.Text, out int newFrameIndex))
                FrameIndex = newFrameIndex;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            VideoAnimationManager.ClosingEditor();
        }

        bool changingInternally;
        bool initializing;
        string? relativePathFront;
        string? relativePathBack;
        bool settingColorInternally;
        bool dragStarted;
        LiveVideoEditor liveVideoEditor;
        public enum Attribute
        {
            X,
            Y,
            Scale,
            Rotation,
            Opacity
        }

        private void tbxDeltaX_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged(Attribute.X, tbxDeltaX, sldDeltaX);
        }

        private void sldDeltaX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderChanged(tbxDeltaX, sldDeltaX);
        }

        private void tbxDeltaY_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged(Attribute.Y, tbxDeltaY, sldDeltaY);
        }

        private void sldDeltaY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderChanged(tbxDeltaY, sldDeltaY);
        }

        private void tbxDeltaScale_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged(Attribute.Scale, tbxDeltaScale, sldDeltaScale);
        }

        private void sldDeltaScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderChanged(tbxDeltaScale, sldDeltaScale);
        }

        private void tbxDeltaRotation_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged(Attribute.Rotation, tbxDeltaRotation, sldDeltaRotation);
        }

        private void sldDeltaRotation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderChanged(tbxDeltaRotation, sldDeltaRotation);
        }

        private void tbxDeltaOpacity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged(Attribute.Opacity, tbxDeltaOpacity, sldDeltaOpacity);
        }

        private void sldDeltaOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderChanged(tbxDeltaOpacity, sldDeltaOpacity);
        }

        private void TextChanged(Attribute attribute, System.Windows.Controls.TextBox textBox, Slider slider)
        {
            if (initializing)
                return;
            changingInternally = true;
            try
            {
                if (double.TryParse(textBox.Text, out double value))
                {
                    Change(attribute, value);
                    slider.Value = value;
                    UpdateValuePreviews();
                }
            }
            finally
            {
                changingInternally = false;
            }
        }

        private void SliderChanged(System.Windows.Controls.TextBox textBox, Slider slider)
        {
            if (changingInternally || initializing)
                return;
            textBox.Text = slider.Value.ToString();
            UpdateValuePreviews();
        }

        private void btnReloadAnimation_Click(object sender, RoutedEventArgs e)
        {
            List<VideoAnimationBinding> allBindings = AllVideoBindings.GetAll(SelectedSceneName);
            LoadAllFrames(allBindings);
            DrawActiveFrame();
        }

        private void btnSaveAnimation_Click(object sender, RoutedEventArgs e)
        {
            if (backFiles == null || TotalAnimationFrames == 0)
                return;
            SaveAllFrames();
        }

        private string GetMovementFileName()
        {
            if (FramesAreNotGood())
                return null;
            return ActiveMovementFileName;
        }

        private void btnCopyDeltaXForward_Click(object sender, RoutedEventArgs e)
        {
            CopyForward(Attribute.X);
        }

        private void btnCopyDeltaYForward_Click(object sender, RoutedEventArgs e)
        {
            CopyForward(Attribute.Y);
        }

        private void btnCopyDeltaRotationForward_Click(object sender, RoutedEventArgs e)
        {
            CopyForward(Attribute.Rotation);
        }

        private void btnCopyDeltaScaleForward_Click(object sender, RoutedEventArgs e)
        {
            CopyForward(Attribute.Scale);
        }

        private void btnCopyDeltaOpacityForward_Click(object sender, RoutedEventArgs e)
        {
            CopyForward(Attribute.Opacity);
        }

        void CopyForward(Attribute attribute)
        {
            if (frameIndex < 0)
                return;
            if (frameIndex >= allFrames.Count)
                return;

            ObsTransformEdit currentFrame = allFrames[frameIndex];

            int indexToChange = frameIndex + 1;
            while (indexToChange < allFrames.Count)
            {
                ObsTransformEdit frame = allFrames[indexToChange];
                switch (attribute)
                {
                    case Attribute.X:
                        if (AreClose(frame.Origin.X, currentFrame.Origin.X, 1))
                        {
                            frame.DeltaX = currentFrame.DeltaX;
                            break;
                        }
                        else
                            return;
                    case Attribute.Y:
                        if (AreClose(frame.Origin.Y, currentFrame.Origin.Y, 1))
                        {
                            frame.DeltaY = currentFrame.DeltaY;
                            break;
                        }
                        else
                            return;
                    case Attribute.Rotation:
                        if (AreClose(frame.Rotation, currentFrame.Rotation, 0.1))
                        {
                            frame.DeltaRotation = currentFrame.DeltaRotation;
                            break;
                        }
                        else
                            return;
                    case Attribute.Scale:
                        if (AreClose(currentFrame.Scale, frame.Scale, 0.01))
                        {
                            frame.DeltaScale = currentFrame.DeltaScale;
                            break;
                        }
                        else
                            return;
                    case Attribute.Opacity:
                        if (AreClose(frame.Opacity, currentFrame.Opacity, 0.01))
                        {
                            frame.DeltaOpacity = currentFrame.DeltaOpacity;
                            break;
                        }
                        else
                            return;
                }
                indexToChange++;
            }
        }

        private static bool AreClose(double currentValue, double compareValue, double wiggleRoom = 0)
        {
            return Math.Abs(compareValue - currentValue) <= wiggleRoom;
        }

        void UpdatePlayhead()
        {
            if (sldFrameIndex.Maximum == 0)
                return;
            playheadGraphic.Height = spTimeline.ActualHeight;
            playheadGraphicInnerLine.Height = spTimeline.ActualHeight;
            double percentageOfTheWayAcross = FrameIndex / sldFrameIndex.Maximum;
            double availableWidth = opacitySequenceVisualizer.ActualWidth - opacitySequenceVisualizer.LabelWidth;
            double xPos = opacitySequenceVisualizer.LabelWidth + percentageOfTheWayAcross * availableWidth;
            Canvas.SetLeft(playheadGraphic, xPos - 2);
            Canvas.SetLeft(playheadGraphicInnerLine, xPos);
        }

        private void sldFrameIndex_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FrameIndex = (int)sldFrameIndex.Value;
            changingInternally = true;
            try
            {
                tbxFrameNumber.Text = FrameIndex.ToString();
                UpdatePlayhead();
            }
            finally
            {
                changingInternally = false;
            }
        }

        void SaveAllFrames()
        {
            if (ActiveMovementFileName == null)
                return;
            foreach (AnimatorWithTransforms animatorWithTransforms in liveFeedAnimators)
                SaveFrames(animatorWithTransforms.MovementFileName);

            SaveLightingData();
        }

        bool HasAnyLightingChanges(LightingSequence lightingSequence)
        {
            foreach (Light light in lightingSequence.Lights)
                foreach (LightSequenceData lightSequenceData in light.SequenceData)
                    if (lightSequenceData.Lightness > 0)
                        return true;
            return false;
        }

        private void SaveFrames(string movementFileName)
        {
            LiveFeedAnimator liveFeedAnimator = GetLiveFeedAnimator(movementFileName);

            liveFeedAnimator.LiveFeedSequences.Clear();

            bool firstTime = true;

            ObsTransform lastLiveFeedSequence = null;
            foreach (ObsTransformEdit liveFeedEdit in allFrames)
            {
                ObsTransform liveFeedSequence = new ObsTransform()
                {
                    Camera = liveFeedEdit.Camera,
                    Rotation = liveFeedEdit.GetRotation(),
                    Scale = liveFeedEdit.GetScale(),
                    Opacity = liveFeedEdit.GetOpacity(),
                    Origin = new CommonCore.Point2d(liveFeedEdit.GetX(), liveFeedEdit.GetY()),
                    Flipped = liveFeedEdit.Flipped,
                    Duration = secondsPerFrame
                };
                if (firstTime)
                {
                    firstTime = false;
                }
                else
                {
                    if (lastLiveFeedSequence.Matches(liveFeedSequence))  // Compress movement...
                    {
                        lastLiveFeedSequence.Duration += secondsPerFrame;
                        continue;
                    }
                }
                liveFeedAnimator.LiveFeedSequences.Add(liveFeedSequence);

                lastLiveFeedSequence = liveFeedSequence;
            }

            // Only saves for the active movement file. So if I make changes to two different movement files in
            // the same edit session, I need to save twice - use the radio buttons to decide what I'm saving.
            string fullPathToMovementFile = VideoAnimationManager.GetFullPathToMovementFile(movementFileName);
            string serializedTransforms = Newtonsoft.Json.JsonConvert.SerializeObject(liveFeedAnimator.LiveFeedSequences, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(fullPathToMovementFile, serializedTransforms);
        }

        private void SaveLightingData()
        {
            string fullPathToLightingFile = VideoAnimationManager.GetFullPathToLightsFile(SelectedSceneName);
            if (HasAnyLightingChanges(LightingSequence))
            {
                LightingSequence compressedLightingSequence = LightingSequence.Compress();
                string serializedLights = Newtonsoft.Json.JsonConvert.SerializeObject(compressedLightingSequence, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(fullPathToLightingFile, serializedLights);
            }
            else
            {
                System.Diagnostics.Debugger.Break();
                // TODO: Delete the file!!!
            }
        }

        bool HasLight(List<Light> lights, string id)
        {
            return lights.Any(x => x.ID == id);
        }

        LightingSequence LoadLightingSequence(string sceneName, int totalFrameCount)
        {
            string fullPathToLightsFile = VideoAnimationManager.GetFullPathToLightsFile(sceneName);
            if (System.IO.File.Exists(fullPathToLightsFile))
            {
                string lightsJson = System.IO.File.ReadAllText(fullPathToLightsFile);
                LightingSequence lightingSequence = JsonConvert.DeserializeObject<LightingSequence>(lightsJson);

                LightingSequence decompressedLightingSequence = lightingSequence.Decompress();

                AddCenterLightIfNeeded(totalFrameCount, decompressedLightingSequence);

                return decompressedLightingSequence;
            }
            else
            {
                LightingSequence lightingSequence = new LightingSequence();
                lightingSequence.Lights.Add(NewLight(BluetoothLights.Left_ID, totalFrameCount));
                lightingSequence.Lights.Add(NewLight(BluetoothLights.Center_ID, totalFrameCount));
                lightingSequence.Lights.Add(NewLight(BluetoothLights.Right_ID, totalFrameCount));
                return lightingSequence;
            }
        }

        private void AddCenterLightIfNeeded(int totalFrameCount, LightingSequence decompressedLightingSequence)
        {
            if (!HasLight(decompressedLightingSequence.Lights, BluetoothLights.Center_ID))
                decompressedLightingSequence.Lights.Add(NewLight(BluetoothLights.Center_ID, totalFrameCount));
        }

        private static Light NewLight(string lightId, int totalFrameCount)
        {
            Light light = new Light() { ID = lightId };
            for (int i = 0; i < totalFrameCount; i++)
                light.SequenceData.Add(new LightSequenceData());
            return light;
        }

        void RenderCurrentSequence()
        {

        }
        private void LoadAllFrames(List<VideoAnimationBinding> allBindings)
        {
            liveFeedAnimators = new List<AnimatorWithTransforms>();
            ActiveMovement = null;

            int totalFrameCount = 0;

            LightingSequence = LoadLightingSequence(allBindings.FirstOrDefault().SceneName, totalFrameCount);

            foreach (VideoAnimationBinding videoAnimationBinding in allBindings)
            {
                AnimatorWithTransforms animatorWithTransforms = new AnimatorWithTransforms();
                animatorWithTransforms.MovementFileName = videoAnimationBinding.MovementFileName;
                animatorWithTransforms.LiveFeedAnimator = GetLiveFeedAnimator(videoAnimationBinding.MovementFileName);
                animatorWithTransforms.ObsTransformEdits = new List<ObsTransformEdit>();
                int frameIndex = 0;
                foreach (ObsTransform liveFeedSequence in animatorWithTransforms.LiveFeedAnimator.LiveFeedSequences)
                {
                    int frameCount = (int)Math.Round(liveFeedSequence.Duration / secondsPerFrame);
                    for (int i = 0; i < frameCount; i++)
                    {
                        animatorWithTransforms.ObsTransformEdits.Add(liveFeedSequence.CreateLiveFeedEdit(frameIndex));
                        frameIndex++;
                    }
                }
                if (totalFrameCount == 0)
                    totalFrameCount = frameIndex;

                liveFeedAnimators.Add(animatorWithTransforms);
                if (ActiveMovement == null)
                    ActiveMovement = animatorWithTransforms;
            }

            RenderCurrentSequence();
        }

        Light GetLightFromId(string id)
        {
            if (LightingSequence == null)
                return null;
            return LightingSequence.Lights.Find(x => x.ID == id);
        }

        private void leftLight_ColorChanged(object sender, RoutedEventArgs e)
        {
            if (changingInternally || settingColorInternally)
                return;

            DmxLight.Left.SetColor(leftLight.Hue, leftLight.Saturation, leftLight.Lightness);
            //await BluetoothLights.Left.SetAsync((int)leftLight.Hue, (int)leftLight.Saturation, (int)leftLight.Lightness);
            SetLightFrame(GetLightFromId(BluetoothLights.Left_ID), leftLight);
        }

        private void centerLight_ColorChanged(object sender, RoutedEventArgs e)
        {
            if (changingInternally || settingColorInternally)
                return;

            DmxLight.Center.SetColor(centerLight.Hue, centerLight.Saturation, centerLight.Lightness);
            SetLightFrame(GetLightFromId(BluetoothLights.Center_ID), centerLight);
        }

        private void rightLight_ColorChanged(object sender, RoutedEventArgs e)
        {
            if (changingInternally || settingColorInternally)
                return;

            DmxLight.Right.SetColor(rightLight.Hue, rightLight.Saturation, rightLight.Lightness);
            SetLightFrame(GetLightFromId(BluetoothLights.Right_ID), rightLight);
        }

        private void SetLightFrame(Light light, FrmColorPicker colorPicker)
        {
            SetLightFrame(light, colorPicker, FrameIndex);
        }

        private static void SetLightFrame(Light light, FrmColorPicker colorPicker, int frameIndex)
        {
            if (light == null)
                return;
            while (frameIndex >= light.SequenceData.Count)
                light.SequenceData.Add(new LightSequenceData());
            light.SequenceData[frameIndex].Hue = (int)colorPicker.Hue;
            light.SequenceData[frameIndex].Saturation = (int)colorPicker.Saturation;
            light.SequenceData[frameIndex].Lightness = (int)colorPicker.Lightness;
        }

        async void SetLightColor(string iD, LightSequenceData lightSequenceData)
        {
            if (settingColorInternally || dragStarted)
                return;
            if (iD == BluetoothLights.Left_ID)
            {
                DmxLight.Left.SetColor(lightSequenceData.Hue, lightSequenceData.Saturation, lightSequenceData.Lightness);
                SetColorPicker(lightSequenceData, leftLight);
            }
            else if (iD == BluetoothLights.Right_ID)
            {
                DmxLight.Right.SetColor(lightSequenceData.Hue, lightSequenceData.Saturation, lightSequenceData.Lightness);
                SetColorPicker(lightSequenceData, rightLight);
            }
            else if (iD == BluetoothLights.Center_ID)
            {
                DmxLight.Center.SetColor(lightSequenceData.Hue, lightSequenceData.Saturation, lightSequenceData.Lightness);
                SetColorPicker(lightSequenceData, centerLight);
            }
            else
                System.Diagnostics.Debugger.Break();  // What Light is this?
        }

        private void SetColorPicker(LightSequenceData lightSequenceData, FrmColorPicker colorPicker)
        {
            settingColorInternally = true;
            try
            {
                colorPicker.Hue = (int)lightSequenceData.Hue;
                colorPicker.Saturation = (int)lightSequenceData.Saturation;
                colorPicker.Lightness = (int)lightSequenceData.Lightness;
            }
            finally
            {
                settingColorInternally = false;
            }
        }

        private void btnCopyLightsForward_Click(object sender, RoutedEventArgs e)
        {
            if (backFiles == null || FrameIndex >= TotalAnimationFrames - 1)
                return;
            SetLightFrame(GetLightFromId(BluetoothLights.Right_ID), rightLight, FrameIndex + 1);
            SetLightFrame(GetLightFromId(BluetoothLights.Center_ID), centerLight, FrameIndex + 1);
            SetLightFrame(GetLightFromId(BluetoothLights.Left_ID), leftLight, FrameIndex + 1);
            FrameIndex++;
        }

        private void sldFrameIndex_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStarted = true;
        }

        private void sldFrameIndex_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStarted = false;
            UpdateLights();
        }

        private void CopyColors_Click(object sender, RoutedEventArgs e)
        {
            string colorStr = $"{leftLight.Color.AsHtml}\t{centerLight.Color.AsHtml}\t{rightLight.Color.AsHtml}";
            System.Windows.Clipboard.SetText(colorStr);
        }

        private void PasteColors_Click(object sender, RoutedEventArgs e)
        {
            string clipboardText = System.Windows.Clipboard.GetText();
            if (string.IsNullOrEmpty(clipboardText))
                return;

            string[] parts = clipboardText.Split('\t');
            if (parts.Length != 3)
                return;

            leftLight.Color = new HueSatLight(parts[0]);
            centerLight.Color = new HueSatLight(parts[1]);
            rightLight.Color = new HueSatLight(parts[2]);
        }
    }
}
