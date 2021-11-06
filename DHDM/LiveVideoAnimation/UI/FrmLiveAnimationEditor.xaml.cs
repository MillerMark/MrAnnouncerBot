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
			}
			finally
			{
				initializing = false;
			}
		}


		List<ObsTransformEdit> allFrames
		{
			get
			{
				return ActiveMovement.ObsTransformEdits;
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
		AnimatorWithTransforms ActiveMovement;
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
			LoadAllImages(System.IO.Path.Combine(STR_EditorPath, SelectedSceneName));

			relativePathFront = GetRelativePathBaseName(frontFiles[frameIndex]);
			relativePathBack = GetRelativePathBaseName(backFiles[frameIndex]);
			if (digitCount == 0)
			{
				int lastIndex = backFiles.Length - 1;
				digitCount = lastIndex.ToString().Length;
			}
			sldFrameIndex.Maximum = backFiles.Length - 1;
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
			if (frameIndex < 0 || frameIndex >= backFiles.Length)
			{
				HubtasticBaseStation.ShowImageBack(null);
				HubtasticBaseStation.ShowImageFront(null);
				return;
			}

			HubtasticBaseStation.ShowImageBack(GetRelativePath(backFiles[frameIndex]));
			HubtasticBaseStation.ShowImageFront(GetRelativePath(frontFiles[frameIndex]));

			foreach (AnimatorWithTransforms animatorWithTransform in liveFeedAnimators)
			{
				DrawFrameSource(animatorWithTransform);
			}

		}

		private void DrawFrameSource(AnimatorWithTransforms animatorWithTransform)
		{
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

		private void UpdateUI()
		{
			if (backFiles != null)
				tbTotalFrameCount.Text = backFiles.Length.ToString();
		}

		void LoadAllImages(string selectedPath)
		{
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
			int lastIndex = backFiles.Length - 1;
			int endFrame = Math.Min(lastIndex, frameIndex + extraFramesCount);

			HubtasticBaseStation.PreloadImageBack(relativePathBack, startFrame, endFrame, digitCount);
			HubtasticBaseStation.PreloadImageFront(relativePathFront, startFrame, endFrame, digitCount);
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
				UpdateValuePreviews(liveFeedEdit);
			}
			finally
			{
				changingInternally = false;
			}
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

				if (backFiles != null && value >= backFiles.Length)
					value = backFiles.Length - 1;

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
		public string ActiveMovementFileName { get; set; }


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
		string relativePathFront;
		string relativePathBack;
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
			if (backFiles.Length == 0)
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

		private void sldFrameIndex_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			FrameIndex = (int)sldFrameIndex.Value;
			changingInternally = true;
			try
			{
				tbxFrameNumber.Text = FrameIndex.ToString();
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

			string fullPathToMovementFile = VideoAnimationManager.GetFullPathToMovementFile(movementFileName);
			string serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(liveFeedAnimator.LiveFeedSequences, Newtonsoft.Json.Formatting.Indented);
			System.IO.File.WriteAllText(fullPathToMovementFile, serializedObject);
		}

		private void LoadAllFrames(List<VideoAnimationBinding> allBindings)
		{
			liveFeedAnimators = new List<AnimatorWithTransforms>();
			ActiveMovement = null;
			foreach (VideoAnimationBinding videoAnimationBinding in allBindings)
			{
				AnimatorWithTransforms animatorWithTransforms = new AnimatorWithTransforms();
				animatorWithTransforms.MovementFileName = videoAnimationBinding.MovementFileName;
				animatorWithTransforms.LiveFeedAnimator = GetLiveFeedAnimator(videoAnimationBinding.MovementFileName);
				if (ActiveMovement == null)
					ActiveMovement = animatorWithTransforms;
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
			}
		}
	}
}
