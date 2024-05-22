using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using Imaging;
using ObsControl;
using Newtonsoft.Json;
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
        VisualizerSelector visualizerSelector = new VisualizerSelector();
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

        public LightingSequence? LightingSequence { get; set; }


        List<ObsTransformEdit>? allFrames
        {
            get
            {
                return ActiveMovement?.ObsTransformEdits;
            }
        }

        string[]? backFiles;
        string[]? frontFiles;
        int frameIndex;
        bool settingInternally;
        const double frameRate = 30;  // fps
        const double secondsPerFrame = 1 / frameRate;
        const string STR_EditorPath = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\Editor";
        List<AnimatorWithTransforms>? liveFeedAnimators;
        AnimatorWithTransforms? activeMovement;


        public enum LightKind
        {
            None,
            Left,
            Center,
            Right
        }

        void UpdateEverythingOnTimeline()
        {
            UpdateMovementOnTimeline();
            UpdateLightsOnTimeline();
        }

        private void UpdateMovementOnTimeline()
        {
            List<ObsTransformEdit>? obsTransformEdits = activeMovement?.ObsTransformEdits;
            if (obsTransformEdits == null)
                return;

            scaleSequenceVisualizer.VisualizeData(obsTransformEdits, TotalFrames, x => x.GetScale(), GraphStyle.BoxFromBelow);
            opacitySequenceVisualizer.VisualizeData(obsTransformEdits, TotalFrames, x => x.GetOpacity(), GraphStyle.Opacity);
            rotationSequenceVisualizer.VisualizeData(obsTransformEdits, TotalFrames, x => x.GetRotation(), GraphStyle.BoxFromBelow);
            xSequenceVisualizer.VisualizeData(obsTransformEdits, TotalFrames, x => x.GetX(), GraphStyle.BoxFromBelow);
            ySequenceVisualizer.VisualizeData(obsTransformEdits, TotalFrames, y => y.GetY(), GraphStyle.BoxFromBelow);
        }

        private void UpdateLightsOnTimeline()
        {
            leftLightSequenceVisualizer.VisualizeData(GetLightFromId(BluetoothLights.Left_ID)?.SequenceData, TotalFrames, x => GetColor(x));
            centerLightSequenceVisualizer.VisualizeData(GetLightFromId(BluetoothLights.Center_ID)?.SequenceData, TotalFrames, w => GetColor(w));
            rightLightSequenceVisualizer.VisualizeData(GetLightFromId(BluetoothLights.Right_ID)?.SequenceData, TotalFrames, v => GetColor(v));
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

        AnimatorWithTransforms? ActiveMovement
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
                UpdateEverythingOnTimeline();
            }

        }
        int digitCount;
        void LoadAnimation()
        {
            spSourceRadioButtons.Children.Clear();
            backFiles = null;
            frontFiles = null;
            digitCount = 0;
            ActiveMovementFileName = null;
            if (SelectedSceneName == null)
                return;

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
                relativePathBack = GetRelativePathBaseName(backFiles[frameIndex]);

            if (digitCount == 0)
            {
                int lastIndex = TotalFrames - 1;
                digitCount = lastIndex.ToString().Length;
            }
            //sldFrameIndex.Maximum = TotalAnimationFrames - 1;

            UpdateTotalFrameCount();
            frameIndex = 0;
            DrawActiveFrame();
            ClearEditorValues();
            UpdateFrameUI();
            UpdateEverythingOnTimeline();
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
            if (liveFeedAnimators == null)
                return;
            if (sender is RadioButton radioButton)
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
            liveVideoEditor.ShowImageFront(GetRelativePath(frontFiles![frameIndex]));

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

        ObsTransformEdit? GetActiveFrame()
        {
            if (allFrames == null || frameIndex >= allFrames.Count || frameIndex < 0)
                return null;
            return allFrames[frameIndex];
        }

        void SetAttributeInFrame(int frameIndex, ObsFramePropertyAttribute attribute, double value)
        {
            if (initializing || allFrames == null || frameIndex >= allFrames.Count)
                return;
            ObsTransformEdit liveFeedEdit = allFrames[frameIndex];
            switch (attribute)
            {
                case ObsFramePropertyAttribute.X:
                    liveFeedEdit.DeltaX = (int)value;
                    break;
                case ObsFramePropertyAttribute.Y:
                    liveFeedEdit.DeltaY = (int)value;
                    break;
                case ObsFramePropertyAttribute.Scale:
                    liveFeedEdit.DeltaScale = value;
                    break;
                case ObsFramePropertyAttribute.Rotation:
                    liveFeedEdit.DeltaRotation = value;
                    break;
                case ObsFramePropertyAttribute.Opacity:
                    liveFeedEdit.DeltaOpacity = value;
                    break;
            }
        }

        public int TotalFrames
        {
            get
            {
                if (ActiveMovement != null)
                    return Math.Max(TotalAnimationFrames, ActiveMovement.ObsTransformEdits.Count);
                return TotalAnimationFrames;
            }
        }


        public int TotalAnimationFrames
        {
            get
            {
                int backFilesCount = 0;
                if (backFiles != null)
                    backFilesCount = backFiles.Length;
                int frontFilesCount = 0;
                if (frontFiles != null)
                    frontFilesCount = frontFiles.Length;
                return Math.Max(backFilesCount, frontFilesCount);
            }
        }

        private void UpdateTotalFrameCount()
        {
            if (backFiles != null)
                tbTotalFrameCount.Text = TotalFrames.ToString();
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
            string? directoryName = System.IO.Path.GetDirectoryName(relativePath);
            if (directoryName == null)
                return fileName;

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
            tbFrameIndex.Text = FormatFrameIndex(liveFeedEdit.FrameIndex);
        }

        void UpdateFrameUI()
        {
            if (FramesAreNotGood())
                return;

            ObsTransformEdit liveFeedEdit = allFrames![frameIndex];

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

        private void UpdateValuePreviews(ObsTransformEdit? liveFeedEdit = null)
        {
            if (liveFeedEdit == null)
            {
                if (FramesAreNotGood())
                    return;
                liveFeedEdit = allFrames![frameIndex];
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

                if (value >= TotalFrames)
                    value = TotalFrames - 1;

                if (value < 0)
                    value = 0;

                frameIndex = value;
                DrawActiveFrame();
                UpdateCurrentFrameNumber();
                PreloadAroundActiveFrame(10);
                UpdateFrameUI();

                //changingInternally = true;
                //try
                //{
                //    sldFrameIndex.Value = frameIndex;
                //}
                //finally
                //{
                //    changingInternally = false;
                //}
                UpdatePlayhead();
            }
        }
        public string? SelectedSceneName { get; set; }

        // TODO: Remove this PngPath if we don't need this.
        public string? PngPath { get; set; }

        string? activeMovementFileName;
        public string? ActiveMovementFileName
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
                tbxFrameNumber.Text = (frameIndex + 1).ToString();
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
            {
                FrameIndex = newFrameIndex - 1;
            }
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
        int selectionAnchorFrameIndex;
        bool shiftKeyIsDown;
        int mouseDragStartingFrameIndex;

        private void tbxDeltaX_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged(ObsFramePropertyAttribute.X, tbxDeltaX, sldDeltaX);
        }

        private void sldDeltaX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderChanged(tbxDeltaX, sldDeltaX);
        }

        private void tbxDeltaY_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged(ObsFramePropertyAttribute.Y, tbxDeltaY, sldDeltaY);
        }

        private void sldDeltaY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderChanged(tbxDeltaY, sldDeltaY);
        }

        private void tbxDeltaScale_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged(ObsFramePropertyAttribute.Scale, tbxDeltaScale, sldDeltaScale);
        }

        private void sldDeltaScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderChanged(tbxDeltaScale, sldDeltaScale);
        }

        private void tbxDeltaRotation_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged(ObsFramePropertyAttribute.Rotation, tbxDeltaRotation, sldDeltaRotation);
        }

        private void sldDeltaRotation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderChanged(tbxDeltaRotation, sldDeltaRotation);
        }

        private void tbxDeltaOpacity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged(ObsFramePropertyAttribute.Opacity, tbxDeltaOpacity, sldDeltaOpacity);
        }

        private void sldDeltaOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderChanged(tbxDeltaOpacity, sldDeltaOpacity);
        }

        private void TextChanged(ObsFramePropertyAttribute attribute, System.Windows.Controls.TextBox textBox, Slider slider)
        {
            if (initializing)
                return;
            changingInternally = true;
            try
            {
                if (double.TryParse(textBox.Text, out double value))
                {
                    if (SelectionExists)
                    {
                        for (int i = SelectionLeft; i < SelectionRight; i++)
                            SetAttributeInFrame(i, attribute, value);
                        UpdateEverythingOnTimeline();
                    }
                    else
                    {
                        SetAttributeInFrame(frameIndex, attribute, value);
                    }

                    DrawActiveFrame();

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
            if (SelectedSceneName == null)
                return;
            List<VideoAnimationBinding> allBindings = AllVideoBindings.GetAll(SelectedSceneName);
            LoadAllFrames(allBindings);
            ClearEditorValues();
            DrawActiveFrame();
        }

        private void btnSaveAnimation_Click(object sender, RoutedEventArgs e)
        {
            if (TotalFrames == 0)
                return;
            SaveAllFrames();
        }

        private string? GetMovementFileName()
        {
            if (FramesAreNotGood())
                return null;
            return ActiveMovementFileName;
        }

        private void btnCopyDeltaXForward_Click(object sender, RoutedEventArgs e)
        {
            CopyForward(ObsFramePropertyAttribute.X);
        }

        private void btnCopyDeltaYForward_Click(object sender, RoutedEventArgs e)
        {
            CopyForward(ObsFramePropertyAttribute.Y);
        }

        private void btnCopyDeltaRotationForward_Click(object sender, RoutedEventArgs e)
        {
            CopyForward(ObsFramePropertyAttribute.Rotation);
        }

        private void btnCopyDeltaScaleForward_Click(object sender, RoutedEventArgs e)
        {
            CopyForward(ObsFramePropertyAttribute.Scale);
        }

        private void btnCopyDeltaOpacityForward_Click(object sender, RoutedEventArgs e)
        {
            CopyForward(ObsFramePropertyAttribute.Opacity);
        }

        void CopyForward(ObsFramePropertyAttribute attribute)
        {
            if (frameIndex < 0 || allFrames == null)
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
                    case ObsFramePropertyAttribute.X:
                        if (AreClose(frame.Origin.X, currentFrame.Origin.X, 1))
                        {
                            frame.DeltaX = currentFrame.DeltaX;
                            break;
                        }
                        else
                            return;
                    case ObsFramePropertyAttribute.Y:
                        if (AreClose(frame.Origin.Y, currentFrame.Origin.Y, 1))
                        {
                            frame.DeltaY = currentFrame.DeltaY;
                            break;
                        }
                        else
                            return;
                    case ObsFramePropertyAttribute.Rotation:
                        if (AreClose(frame.Rotation, currentFrame.Rotation, 0.1))
                        {
                            frame.DeltaRotation = currentFrame.DeltaRotation;
                            break;
                        }
                        else
                            return;
                    case ObsFramePropertyAttribute.Scale:
                        if (AreClose(currentFrame.Scale, frame.Scale, 0.01))
                        {
                            frame.DeltaScale = currentFrame.DeltaScale;
                            break;
                        }
                        else
                            return;
                    case ObsFramePropertyAttribute.Opacity:
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
            if (TotalFrames == 0)
                return;

            double leftEdge = GetLabelLeftEdgeFromFrameIndex(FrameIndex, tbFrameIndex);
            double xPos = GetXFromFrameIndex(frameIndex);
            Canvas.SetLeft(playheadGraphic, xPos - 2);
            Canvas.SetLeft(playheadGraphicInnerLine, xPos);
            Canvas.SetLeft(tbFrameIndex, leftEdge);

            if (SelectionExists)
                DrawSelection();
            else
                ClearSelection();
        }

        private bool SelectionExists => selectionAnchorFrameIndex != FrameIndex && selectionAnchorFrameIndex >= 0;

        public int SelectionLeft => Math.Min(selectionAnchorFrameIndex, FrameIndex);
        public int SelectionRight => Math.Max(selectionAnchorFrameIndex, FrameIndex);

        private double GetLabelLeftEdgeFromFrameIndex(double frameIndex, TextBlock textBlock)
        {
            double xPos = GetXFromFrameIndex(frameIndex);
            double leftEdge = xPos - textBlock.ActualWidth / 2;
            if (leftEdge < scaleSequenceVisualizer.LabelWidth)
                leftEdge = scaleSequenceVisualizer.LabelWidth;
            double rightEdge = leftEdge + textBlock.ActualWidth;
            if (rightEdge > spTimeline.ActualWidth)
            {
                double deltaX = spTimeline.ActualWidth - rightEdge;
                leftEdge += deltaX;
            }

            return leftEdge;
        }

        private double GetXFromFrameIndex(double frameIndex)
        {
            double percentageOfTheWayAcross = frameIndex / TotalFrames;
            double availableWidth = opacitySequenceVisualizer.ActualWidth - opacitySequenceVisualizer.LabelWidth;
            return opacitySequenceVisualizer.LabelWidth + percentageOfTheWayAcross * availableWidth;
        }

        void DrawSelection()
        {
            brdFrame.Visibility = Visibility.Visible;
            lnFrameSpan.Visibility = Visibility.Visible;
            double numFramesSelected = Math.Abs(selectionAnchorFrameIndex - FrameIndex);
            
            bool pointingFromTheOutside = numFramesSelected < 52;
            if (pointingFromTheOutside)
                tbNumFramesSelected.Text = $"{numFramesSelected} frames selected";
            else if (numFramesSelected < 88)
                tbNumFramesSelected.Text = $"{numFramesSelected}";
            else
                tbNumFramesSelected.Text = $"{numFramesSelected} frames";
            double numFramesLeftEdge = GetLabelLeftEdgeFromFrameIndex((selectionAnchorFrameIndex + FrameIndex) / 2, tbNumFramesSelected);

            bool selectingLeftToRight = FrameIndex > selectionAnchorFrameIndex;
            const double padding = 10d;
            double halfWidthOfFrameIndexPlusPadding = tbFrameIndex.ActualWidth / 2 + padding;

            if (selectingLeftToRight)
            {
                numFramesLeftEdge -= halfWidthOfFrameIndexPlusPadding / 2;
            }
            else
            {
                numFramesLeftEdge += halfWidthOfFrameIndexPlusPadding / 2;
            }
           

            Canvas.SetTop(brdFrame, cvsFeedbackUI.ActualHeight - brdFrame.ActualHeight);


            double leftEdgeOffset;
            if (selectingLeftToRight)
                leftEdgeOffset = 0;
            else
                leftEdgeOffset = halfWidthOfFrameIndexPlusPadding;

            double frameX = GetXFromFrameIndex(FrameIndex);
            double anchorX = GetXFromFrameIndex(selectionAnchorFrameIndex);
            rectSelection.Width = Math.Abs(frameX - anchorX);
            double selectionLeftEdge = Math.Min(frameX, anchorX);
            double selectionRightEdge = selectionLeftEdge + rectSelection.Width;
            Canvas.SetLeft(rectSelection, selectionLeftEdge);
            

            rectSelection.Visibility = Visibility.Visible;

            lnFrameSpan.Y1 = cvsFeedbackUI.Height / 2;
            lnFrameSpan.Y2 = cvsFeedbackUI.Height / 2;
            lnFrameSpan.X1 = 0;

            bool frameIndexIsLeftOfCenter = (double)FrameIndex / TotalFrames < 0.5;
            if (pointingFromTheOutside)
            {
                const double outsideArrowLength = 33d;
                lnFrameSpan.X2 = outsideArrowLength;
                if (frameIndexIsLeftOfCenter)
                {
                    arrowLeft.Visibility = Visibility.Visible;
                    arrowRight.Visibility = Visibility.Hidden;
                    double arrowLeftIndent;
                    if (selectingLeftToRight)
                        arrowLeftIndent = halfWidthOfFrameIndexPlusPadding;
                    else
                        arrowLeftIndent = 0;
                    double frameIndexRight = Canvas.GetLeft(tbFrameIndex) + tbFrameIndex.ActualWidth + padding;
                    double arrowX = Math.Max(selectionRightEdge + arrowLeftIndent, frameIndexRight);
                    
                    Canvas.SetLeft(arrowLeft, arrowX);
                    Canvas.SetLeft(lnFrameSpan, arrowX);
                    Canvas.SetLeft(brdFrame, arrowX + lnFrameSpan.X2 + padding / 2);
                }
                else
                {
                    arrowRight.Visibility = Visibility.Visible;
                    arrowLeft.Visibility = Visibility.Hidden;
                    double frameIndexLeft = Canvas.GetLeft(tbFrameIndex) - padding;
                    double arrowX = Math.Min(selectionLeftEdge, frameIndexLeft) - arrowRight.ActualWidth;
                    
                    Canvas.SetLeft(arrowRight, arrowX);
                    double arrowTailLeft = arrowX - lnFrameSpan.X2 + arrowRight.ActualWidth;
                    Canvas.SetLeft(lnFrameSpan, arrowTailLeft);
                    Canvas.SetLeft(brdFrame, arrowTailLeft - brdFrame.ActualWidth - padding / 2);
                }
            }
            else
            {
                arrowLeft.Visibility = Visibility.Visible;
                arrowRight.Visibility = Visibility.Visible;
                double leftEdge = selectionLeftEdge + leftEdgeOffset;
                Canvas.SetLeft(lnFrameSpan, leftEdge);
                lnFrameSpan.X2 = rectSelection.Width - halfWidthOfFrameIndexPlusPadding;
                Canvas.SetLeft(arrowRight, leftEdge + lnFrameSpan.X2 - arrowRight.ActualWidth);
                Canvas.SetLeft(arrowLeft, selectionLeftEdge + leftEdgeOffset);
                Canvas.SetLeft(brdFrame, numFramesLeftEdge - brdFrame.Padding.Left);
            }
            Canvas.SetTop(arrowLeft, (cvsFeedbackUI.ActualHeight - arrowLeft.ActualHeight) / 2);
            Canvas.SetTop(arrowRight, (cvsFeedbackUI.ActualHeight - arrowLeft.ActualHeight) / 2);
        }

        void ClearSelection()
        {
            brdFrame.Visibility = Visibility.Hidden;
            arrowLeft.Visibility = Visibility.Hidden;
            arrowRight.Visibility = Visibility.Hidden;
            lnFrameSpan.Visibility = Visibility.Hidden;
            rectSelection.Visibility = Visibility.Hidden;
        }

        //private void sldFrameIndex_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    if (changingInternally)
        //        return;

        //    FrameIndex = (int)sldFrameIndex.Value;
        //    UpdatePlayhead();

        //    changingInternally = true;
        //    try
        //    {
        //        tbxFrameNumber.Text = (FrameIndex + 1).ToString();
        //    }
        //    finally
        //    {
        //        changingInternally = false;
        //    }
        //}

        void SaveAllFrames()
        {
            if (ActiveMovementFileName == null)
                return;
            foreach (AnimatorWithTransforms animatorWithTransforms in liveFeedAnimators)
                CompressAndSaveFrames(animatorWithTransforms.MovementFileName);

            SaveLightingData();
        }

        bool HasAnyLightingChanges(LightingSequence? lightingSequence)
        {
            if (lightingSequence == null)
                return false;
            foreach (Light light in lightingSequence.Lights)
                foreach (LightSequenceData lightSequenceData in light.SequenceData)
                    if (lightSequenceData.Lightness > 0)
                        return true;
            return false;
        }

        private void CompressAndSaveFrames(string movementFileName)
        {
            LiveFeedAnimator liveFeedAnimator = GetLiveFeedAnimator(movementFileName);

            liveFeedAnimator.LiveFeedSequences.Clear();

            bool firstTime = true;

            ObsTransform? lastLiveFeedSequence = null;
            if (allFrames != null)
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
                    else if (lastLiveFeedSequence != null)
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
            if (SelectedSceneName == null)
                return;
            string fullPathToLightingFile = VideoAnimationManager.GetFullPathToLightsFile(SelectedSceneName);
            if (HasAnyLightingChanges(LightingSequence))
            {
                LightingSequence compressedLightingSequence = LightingSequence!.Compress();
                string serializedLights = JsonConvert.SerializeObject(compressedLightingSequence, Formatting.Indented);
                System.IO.File.WriteAllText(fullPathToLightingFile, serializedLights);
            }
            else
            {
                // TODO: Delete the file!!!
            }
        }

        bool HasLight(List<Light>? lights, string id)
        {
            return lights?.Any(x => x.ID == id) == true;
        }

        LightingSequence? LoadLightingSequence(string? sceneName, int totalFrameCount)
        {
            if (sceneName == null)
                return null;
            string fullPathToLightsFile = VideoAnimationManager.GetFullPathToLightsFile(sceneName);
            if (System.IO.File.Exists(fullPathToLightsFile))
            {
                string lightsJson = System.IO.File.ReadAllText(fullPathToLightsFile);
                LightingSequence? lightingSequence = JsonConvert.DeserializeObject<LightingSequence>(lightsJson);

                LightingSequence? decompressedLightingSequence = lightingSequence?.Decompress();

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

        private void AddCenterLightIfNeeded(int totalFrameCount, LightingSequence? decompressedLightingSequence)
        {
            if (!HasLight(decompressedLightingSequence?.Lights, BluetoothLights.Center_ID))
                decompressedLightingSequence?.Lights.Add(NewLight(BluetoothLights.Center_ID, totalFrameCount));
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

            LightingSequence = LoadLightingSequence(allBindings?.FirstOrDefault()?.SceneName, totalFrameCount);

            if (allBindings != null)
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

        Light? GetLightFromId(string id)
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
            ApplyLightColor(GetLightFromId(BluetoothLights.Left_ID), leftLight);
        }

        private void centerLight_ColorChanged(object sender, RoutedEventArgs e)
        {
            if (changingInternally || settingColorInternally)
                return;

            DmxLight.Center.SetColor(centerLight.Hue, centerLight.Saturation, centerLight.Lightness);
            ApplyLightColor(GetLightFromId(BluetoothLights.Center_ID), centerLight);
        }

        private void rightLight_ColorChanged(object sender, RoutedEventArgs e)
        {
            if (changingInternally || settingColorInternally)
                return;

            DmxLight.Right.SetColor(rightLightColor.Hue, rightLightColor.Saturation, rightLightColor.Lightness);
            ApplyLightColor(GetLightFromId(BluetoothLights.Right_ID), rightLightColor);
        }

        private void ApplyLightColor(Light? light, FrmColorPicker colorPicker)
        {
            if (SelectionExists)
            {
                for (int i = SelectionLeft; i < SelectionRight; i++)
                    SetLightFrame(light, colorPicker, i);
                UpdateLightsOnTimeline();
            }
            else
                SetLightFrame(light, colorPicker, FrameIndex);
        }

        private static void SetLightFrame(Light? light, FrmColorPicker colorPicker, int frameIndex)
        {
            if (light == null)
                return;
            while (frameIndex >= light.SequenceData.Count)
                light.SequenceData.Add(new LightSequenceData());
            light.SequenceData[frameIndex].Hue = (int)colorPicker.Hue;
            light.SequenceData[frameIndex].Saturation = (int)colorPicker.Saturation;
            light.SequenceData[frameIndex].Lightness = (int)colorPicker.Lightness;
        }

        void SetLightColor(string iD, LightSequenceData lightSequenceData)
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
                SetColorPicker(lightSequenceData, rightLightColor);
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
            if (backFiles == null || FrameIndex >= TotalFrames - 1)
                return;
            SetLightFrame(GetLightFromId(BluetoothLights.Right_ID), rightLightColor, FrameIndex + 1);
            SetLightFrame(GetLightFromId(BluetoothLights.Center_ID), centerLight, FrameIndex + 1);
            SetLightFrame(GetLightFromId(BluetoothLights.Left_ID), leftLight, FrameIndex + 1);
            FrameIndex++;
        }

        //private void sldFrameIndex_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        //{
        //    dragStarted = true;
        //}

        //private void sldFrameIndex_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        //{
        //    dragStarted = false;
        //    UpdateLights();
        //}

        private void CopyColors_Click(object sender, RoutedEventArgs e)
        {
            string colorStr = $"{leftLight.Color.AsHtml}\t{centerLight.Color.AsHtml}\t{rightLightColor.Color.AsHtml}";
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
            rightLightColor.Color = new HueSatLight(parts[2]);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateEverythingOnTimeline();
        }

        int GetFrameIndexFromMousePosition(MouseEventArgs e)
        {
            if (TotalFrames == 0)
                return 0;

            Point position = e.GetPosition(cvsPlayhead);

            if (position.X < scaleSequenceVisualizer.LabelWidth)
                return 0;

            double frameWidth = (cvsPlayhead.ActualWidth - scaleSequenceVisualizer.LabelWidth) / TotalFrames;

            return (int)Math.Round((position.X - scaleSequenceVisualizer.LabelWidth) / frameWidth);
        }

        private void cvsInput_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SequenceVisualizer? sequenceVisualizer = GetSequenceVisualizerFromPosition(e);
            if (sequenceVisualizer != null)
                visualizerSelector.SelectVisualizer(sequenceVisualizer);

            if (TotalFrames == 0)
                return;

            if (cvsInput.IsMouseCaptured)
                return;

            cvsInput.CaptureMouse();

            int currentFrameIndex = GetFrameIndexFromMousePosition(e);
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                if (!shiftKeyIsDown)
                {
                    shiftKeyIsDown = true;
                    selectionAnchorFrameIndex = currentFrameIndex; // FrameIndex
                }
            }
            else
            {
                mouseDragStartingFrameIndex = currentFrameIndex;
                selectionAnchorFrameIndex = -1;
            }

            FrameIndex = currentFrameIndex;
        }

        private void cvsInput_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!cvsInput.IsMouseCaptured)
                return;
            // We're creating a selection.
            FrameIndex = GetFrameIndexFromMousePosition(e);
        }

        private void cvsInput_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Math.Abs(selectionAnchorFrameIndex - FrameIndex) < 2)
                selectionAnchorFrameIndex = FrameIndex;
            ShiftKeyIsUp();
            cvsInput.ReleaseMouseCapture();
            UpdatePlayhead();
        }

        void ShiftKeyIsUp()
        {
            shiftKeyIsDown = false;
        }

        void ShiftKeyIsDown()
        {
            if (cvsInput.IsMouseCaptured && selectionAnchorFrameIndex == -1)
            {
                selectionAnchorFrameIndex = mouseDragStartingFrameIndex;
                shiftKeyIsDown = true;
                UpdatePlayhead();
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                ShiftKeyIsUp();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                ShiftKeyIsDown();

        }

        bool IsMouseOverSequenceVisualizer(SequenceVisualizer sequenceVisualizer, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(sequenceVisualizer);

            Size controlSize = sequenceVisualizer.RenderSize;

            Rect controlRect = new Rect(new Point(0, 0), controlSize);
            return controlRect.Contains(mousePosition);
        }

        LightKind GetLightKindFromPosition(MouseButtonEventArgs e)
        {
            if (IsMouseOverSequenceVisualizer(leftLightSequenceVisualizer, e)) 
                return LightKind.Left;
            if (IsMouseOverSequenceVisualizer(rightLightSequenceVisualizer, e)) 
                return LightKind.Right;
            if (IsMouseOverSequenceVisualizer(centerLightSequenceVisualizer, e))
                return LightKind.Center;

            return LightKind.None;
        }

        ObsFramePropertyAttribute GetAttributeFromPosition(MouseButtonEventArgs e)
        {
            if (IsMouseOverSequenceVisualizer(opacitySequenceVisualizer, e)) 
                return ObsFramePropertyAttribute.Opacity;
            if (IsMouseOverSequenceVisualizer(scaleSequenceVisualizer, e)) 
                return ObsFramePropertyAttribute.Scale;
            if (IsMouseOverSequenceVisualizer(xSequenceVisualizer, e)) 
                return ObsFramePropertyAttribute.X;
            if (IsMouseOverSequenceVisualizer(ySequenceVisualizer, e)) 
                return ObsFramePropertyAttribute.Y;
            if (IsMouseOverSequenceVisualizer(rotationSequenceVisualizer, e)) 
                return ObsFramePropertyAttribute.Rotation;
            return ObsFramePropertyAttribute.None;
        }

        SequenceVisualizer? GetSequenceVisualizerFromPosition(MouseButtonEventArgs e)
        {
            if (IsMouseOverSequenceVisualizer(opacitySequenceVisualizer, e))
                return opacitySequenceVisualizer;
            if (IsMouseOverSequenceVisualizer(scaleSequenceVisualizer, e))
                return scaleSequenceVisualizer;
            if (IsMouseOverSequenceVisualizer(xSequenceVisualizer, e))
                return xSequenceVisualizer;
            if (IsMouseOverSequenceVisualizer(ySequenceVisualizer, e))
                return ySequenceVisualizer;
            if (IsMouseOverSequenceVisualizer(rotationSequenceVisualizer, e))
                return rotationSequenceVisualizer;
            if (IsMouseOverSequenceVisualizer(rightLightSequenceVisualizer, e))
                return rightLightSequenceVisualizer;
            if (IsMouseOverSequenceVisualizer(leftLightSequenceVisualizer, e))
                return leftLightSequenceVisualizer;
            if (IsMouseOverSequenceVisualizer(centerLightSequenceVisualizer, e))
                return centerLightSequenceVisualizer;
            return null;
        }

        void SelectAllMatchingFrames(ObsFramePropertyAttribute attribute, int frameIndex)
        {
            if (allFrames == null)
                return;

            if (frameIndex < 0 || frameIndex >= allFrames.Count)
                return;

            ObsTransformEdit obsTransformEdit = allFrames[frameIndex];

            double comparisonValue;

            switch (attribute)
            {
                case ObsFramePropertyAttribute.None:
                    return;
                case ObsFramePropertyAttribute.X:
                    comparisonValue = obsTransformEdit.GetX();
                    break;
                case ObsFramePropertyAttribute.Y:
                    comparisonValue = obsTransformEdit.GetY();
                    break;
                case ObsFramePropertyAttribute.Scale:
                    comparisonValue = obsTransformEdit.GetScale();
                    break;
                case ObsFramePropertyAttribute.Rotation:
                    comparisonValue = obsTransformEdit.GetRotation();
                    break;
                case ObsFramePropertyAttribute.Opacity:
                    comparisonValue = obsTransformEdit.GetOpacity();
                    break;
                default:
                    return;
            }


            Func<ObsTransformEdit, bool> isMatch = x => x.Matches(attribute, comparisonValue);
            int leftMatchingFrame = SearchBackwards(allFrames, frameIndex, isMatch);
            int rightMatchingFrame = SearchForwards(allFrames, frameIndex, isMatch);
            SelectRange(leftMatchingFrame, rightMatchingFrame);
        }

        private void SelectRange(int leftFrame, int rightFrame)
        {
            selectionAnchorFrameIndex = leftFrame;
            FrameIndex = rightFrame;
            UpdatePlayhead();
        }

        int SearchBackwards<T>(List<T> sequenceData, int frameIndex, Func<T, bool> isMatch)
        {
            while (frameIndex > 0)
            {
                if (!isMatch(sequenceData[frameIndex - 1]))
                    return frameIndex;
                frameIndex--;
            }
            return frameIndex;
        }

        int SearchForwards<T>(List<T> sequenceData, int frameIndex, Func<T, bool> isMatch)
        {
            while (frameIndex < sequenceData.Count - 1)
            {
                if (!isMatch(sequenceData[frameIndex + 1]))
                    return frameIndex;
                frameIndex++;
            }
            return frameIndex;
        }

        void SelectAllMatchingFrames(LightKind lightKind, int frameIndex)
        {
            Light? light = null;

            switch (lightKind)
            {
                case LightKind.Left:
                    light = GetLightFromId(BluetoothLights.Left_ID);
                    break;
                case LightKind.Center:
                    light = GetLightFromId(BluetoothLights.Center_ID);
                    break;
                case LightKind.Right:
                    light = GetLightFromId(BluetoothLights.Right_ID);
                    break;
            }

            if (light == null)
                return;

            if (frameIndex < 0 || frameIndex >= light.SequenceData.Count)
                return;


            int hue = light.SequenceData[frameIndex].Hue;
            int saturation = light.SequenceData[frameIndex].Saturation;
            int lightness = light.SequenceData[frameIndex].Lightness;

            Func<LightSequenceData, bool> isMatch = x => x.Hue == hue && x.Saturation == saturation && x.Lightness == lightness;
            int leftMatchingFrame = SearchBackwards(light.SequenceData, frameIndex, isMatch);
            int rightMatchingFrame = SearchForwards(light.SequenceData, frameIndex, isMatch);
            SelectRange(leftMatchingFrame, rightMatchingFrame);
        }

        private void cvsInput_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                cvsInput.ReleaseMouseCapture();

                int currentFrameIndex = GetFrameIndexFromMousePosition(e);
                var attribute = GetAttributeFromPosition(e);
                if (attribute == ObsFramePropertyAttribute.None)
                {
                    var lightKind = GetLightKindFromPosition(e);
                    if (lightKind != LightKind.None)
                    {
                        SelectAllMatchingFrames(lightKind, currentFrameIndex);
                    }

                }
                else
                    SelectAllMatchingFrames(attribute, currentFrameIndex);

                e.Handled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (UIElement uIElement in spVisualizers.Children)
            {
                if (uIElement is ISelectableVisualizer selectableVisualizer)
                    visualizerSelector.AddSelectableVisualizer(selectableVisualizer);
            }
            
        }

        ObsFramePropertyAttribute GetObsFrameAttributeFromVisualizer(ISelectableVisualizer selectedVisualizer)
        {
            if (selectedVisualizer == scaleSequenceVisualizer)
                return ObsFramePropertyAttribute.Scale;
            if (selectedVisualizer == rotationSequenceVisualizer)
                return ObsFramePropertyAttribute.Rotation;
            if (selectedVisualizer == opacitySequenceVisualizer)
                return ObsFramePropertyAttribute.Opacity;
            if (selectedVisualizer == xSequenceVisualizer)
                return ObsFramePropertyAttribute.X;
            if (selectedVisualizer == ySequenceVisualizer)
                return ObsFramePropertyAttribute.Y;

            return ObsFramePropertyAttribute.None;
        }

        LightKind GetLightKindFromVisualizer(ISelectableVisualizer selectedVisualizer)
        {
            if (selectedVisualizer == rightLightSequenceVisualizer)
                return LightKind.Right;
            if (selectedVisualizer == leftLightSequenceVisualizer)
                return LightKind.Left;
            if (selectedVisualizer == centerLightSequenceVisualizer)
                return LightKind.Center;
            return LightKind.None;
        }

        int GetPreviousFrameMatching(Func<ObsTransformEdit, double> getValue)
        {
            if (allFrames == null || allFrames.Count == 0)
                return 0;

            var index = FrameIndex;
            if (index >= allFrames.Count)
                index = allFrames.Count() - 1;

            ObsTransformEdit? activeFrame = allFrames[index];

            if (activeFrame == null)
                return 0;

            double currentValue = getValue(activeFrame);

            if (index > 1)
                if (getValue(allFrames[index - 1]) != currentValue)
                {
                    currentValue = getValue(allFrames[index - 1]);
                    index--;
                }

            while (index > 0 && getValue(allFrames[index]) == currentValue)
            {
                index--;
            }
            return index;
        }

        int GetPreviousFrameMatching(List<LightSequenceData> sequenceData)
        {
            if (allFrames == null)
                return 0;
            var index = FrameIndex;

            if (index >= sequenceData.Count)
                index = sequenceData.Count() - 1;

            LightSequenceData currentValue = sequenceData[index];
            if (index >= sequenceData.Count)
                index = sequenceData.Count - 1;

            if (index > 1)
                if (!sequenceData[index - 1].Matches(currentValue))
                {
                    currentValue = sequenceData[index - 1];
                    index--;
                }

            while (index > 0 && sequenceData[index].Matches(currentValue))
            {
                index--;
            }
            return index;
        }

        ObsTransformEdit? GetCurrentObsTransformEdit()
        {
            if (allFrames == null)
                return null;
            var index = FrameIndex;
            if (index >= allFrames.Count)
                index = allFrames.Count - 1;

            return allFrames[index];
        }

        int GetNextFrameMatching(Func<ObsTransformEdit, double> getValue)
        {
            if (allFrames == null || allFrames.Count == 0)
                return 0;

            var index = FrameIndex;
            if (index >= allFrames.Count)
                index = allFrames.Count - 1;

            ObsTransformEdit? activeFrame = allFrames[index];

            if (activeFrame == null)
                return 0;

            double currentValue = getValue(activeFrame);

            if (index < allFrames.Count - 2)
                if (getValue(allFrames[index + 1]) != currentValue)
                {
                    currentValue = getValue(allFrames[index + 1]);
                    index++;
                }

            while (index < allFrames.Count - 1 && getValue(allFrames[index]) == currentValue)
            {
                index++;
            }
            return index;
        }

        int GetNextFrameMatching(List<LightSequenceData> sequenceData)
        {
            if (allFrames == null)
                return 0;
            var index = FrameIndex;

            if (index >= sequenceData.Count)
                index = sequenceData.Count() - 1;

            LightSequenceData currentValue = sequenceData[index];
            if (index >= sequenceData.Count)
                index = sequenceData.Count - 1;

            if (index < sequenceData.Count - 2)
                if (!sequenceData[index + 1].Matches(currentValue))
                {
                    currentValue = sequenceData[index + 1];
                    index++;
                }

            while (index < sequenceData.Count - 1 && sequenceData[index].Matches(currentValue))
            {
                index++;
            }
            return index;
        }

        private void btnPreviousDifference_Click(object sender, RoutedEventArgs e)
        {
            ISelectableVisualizer? selectedVisualizer = visualizerSelector.SelectedVisualizer;
            if (selectedVisualizer == null)
                return;
            ObsFramePropertyAttribute frameAttribute = GetObsFrameAttributeFromVisualizer(selectedVisualizer);

            if (frameAttribute != ObsFramePropertyAttribute.None)
            {
                switch (frameAttribute)
                {
                    case ObsFramePropertyAttribute.X:
                        FrameIndex = GetPreviousFrameMatching(x => x.GetX());
                        break;
                    case ObsFramePropertyAttribute.Y:
                        FrameIndex = GetPreviousFrameMatching(y => y.GetY());
                        break;
                    case ObsFramePropertyAttribute.Scale:
                        FrameIndex = GetPreviousFrameMatching(scale => scale.GetScale());
                        break;
                    case ObsFramePropertyAttribute.Rotation:
                        FrameIndex = GetPreviousFrameMatching(rotation => rotation.GetRotation());
                        break;
                    case ObsFramePropertyAttribute.Opacity:
                        FrameIndex = GetPreviousFrameMatching(opacity => opacity.GetOpacity());
                        break;
                }

            }
            else
            {
                LightKind lightKind = GetLightKindFromVisualizer(selectedVisualizer);
                List<LightSequenceData>? sequenceData = null;
                switch (lightKind)
                {
                    case LightKind.Left:
                        sequenceData = GetLightFromId(BluetoothLights.Left_ID)?.SequenceData;
                        break;
                    case LightKind.Center:
                        sequenceData = GetLightFromId(BluetoothLights.Center_ID)?.SequenceData;
                        break;
                    case LightKind.Right:
                        sequenceData = GetLightFromId(BluetoothLights.Right_ID)?.SequenceData;
                        break;
                }

                if (sequenceData != null)
                    FrameIndex = GetPreviousFrameMatching(sequenceData);
            }
        }

        private void btnNextDifference_Click(object sender, RoutedEventArgs e)
        {
            ISelectableVisualizer? selectedVisualizer = visualizerSelector.SelectedVisualizer;
            if (selectedVisualizer == null)
                return;
            ObsFramePropertyAttribute frameAttribute = GetObsFrameAttributeFromVisualizer(selectedVisualizer);

            if (frameAttribute != ObsFramePropertyAttribute.None)
            {
                switch (frameAttribute)
                {
                    case ObsFramePropertyAttribute.X:
                        FrameIndex = GetNextFrameMatching(x => x.GetX());
                        break;
                    case ObsFramePropertyAttribute.Y:
                        FrameIndex = GetNextFrameMatching(y => y.GetY());
                        break;
                    case ObsFramePropertyAttribute.Scale:
                        FrameIndex = GetNextFrameMatching(scale => scale.GetScale());
                        break;
                    case ObsFramePropertyAttribute.Rotation:
                        FrameIndex = GetNextFrameMatching(rotation => rotation.GetRotation());
                        break;
                    case ObsFramePropertyAttribute.Opacity:
                        FrameIndex = GetNextFrameMatching(opacity => opacity.GetOpacity());
                        break;
                }

            }
            else
            {
                LightKind lightKind = GetLightKindFromVisualizer(selectedVisualizer);
                List<LightSequenceData>? sequenceData = null;
                switch (lightKind)
                {
                    case LightKind.Left:
                        sequenceData = GetLightFromId(BluetoothLights.Left_ID)?.SequenceData;
                        break;
                    case LightKind.Center:
                        sequenceData = GetLightFromId(BluetoothLights.Center_ID)?.SequenceData;
                        break;
                    case LightKind.Right:
                        sequenceData = GetLightFromId(BluetoothLights.Right_ID)?.SequenceData;
                        break;
                }

                if (sequenceData != null)
                    FrameIndex = GetNextFrameMatching(sequenceData);
            }
        }

        void ApplyPlayHeadToSelectionForLights(List<LightSequenceData> sequenceData)
        {
            if (!SelectionExists)
                return;
            if (allFrames == null)
                return;
            if (FrameIndex < 0 || FrameIndex > sequenceData.Count)
                return;
            int index = SelectionLeft;
            int endIndex = SelectionRight;
            if (endIndex >= sequenceData.Count)
                endIndex = sequenceData.Count - 1;

            var startingValue = sequenceData[FrameIndex];

            while (index < endIndex)
            {
                var lightData = sequenceData[index];
                lightData.SetFrom(startingValue);
                index++;
            }
        }

        void ApplyPlayHeadToSelection(Action<ObsTransformEdit> setValue)
        {
            if (!SelectionExists)
                return;
            if (allFrames == null)
                return;
            if (FrameIndex < 0 || FrameIndex > allFrames.Count)
                return;
            int index = SelectionLeft;
            int endIndex = SelectionRight;
            if (endIndex >= allFrames.Count)
                endIndex = allFrames.Count - 1;

            while (index < endIndex)
            {
                ObsTransformEdit obsTransformEdit = allFrames[index];
                setValue(obsTransformEdit);
                index++;
            }
        }

        private void btnApplyPlayheadValueToSelection_Click(object sender, RoutedEventArgs e)
        {
            ISelectableVisualizer? selectedVisualizer = visualizerSelector.SelectedVisualizer;
            if (selectedVisualizer == null)
                return;
            ObsFramePropertyAttribute frameAttribute = GetObsFrameAttributeFromVisualizer(selectedVisualizer);
            ObsTransformEdit? transformEdit = GetCurrentObsTransformEdit();
            if (transformEdit == null)
                return;

            if (frameAttribute != ObsFramePropertyAttribute.None)
            {
                switch (frameAttribute)
                {
                    case ObsFramePropertyAttribute.X:
                        ApplyPlayHeadToSelection(x => x.Origin = new CommonCore.Point2d(transformEdit.Origin.X, x.Origin.Y));
                        break;

                    case ObsFramePropertyAttribute.Y:
                        ApplyPlayHeadToSelection(x => x.Origin = new CommonCore.Point2d(x.Origin.X, transformEdit.Origin.Y));
                        break;
                    
                    case ObsFramePropertyAttribute.Scale:
                        ApplyPlayHeadToSelection(x => x.Scale = transformEdit.Scale);
                        break;

                    case ObsFramePropertyAttribute.Rotation:
                        ApplyPlayHeadToSelection(x => x.Rotation = transformEdit.Rotation);
                        break;

                    case ObsFramePropertyAttribute.Opacity:
                        ApplyPlayHeadToSelection(x => x.Opacity = transformEdit.Opacity);
                        break;
                }

            }
            else
            {
                LightKind lightKind = GetLightKindFromVisualizer(selectedVisualizer);
                List<LightSequenceData>? sequenceData = null;
                switch (lightKind)
                {
                    case LightKind.Left:
                        sequenceData = GetLightFromId(BluetoothLights.Left_ID)?.SequenceData;
                        break;
                    case LightKind.Center:
                        sequenceData = GetLightFromId(BluetoothLights.Center_ID)?.SequenceData;
                        break;
                    case LightKind.Right:
                        sequenceData = GetLightFromId(BluetoothLights.Right_ID)?.SequenceData;
                        break;
                }

                if (sequenceData != null)
                    ApplyPlayHeadToSelectionForLights(sequenceData);
            }

            UpdateEverythingOnTimeline();
        }

        private void btnLinearInterpolateAcrossSelection_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
