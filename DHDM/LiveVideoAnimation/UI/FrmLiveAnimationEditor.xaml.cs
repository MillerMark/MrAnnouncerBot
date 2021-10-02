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

		
		List<LiveFeedEdit> allFrames;
		string[] backFiles;
		string[] frontFiles;
		int frameIndex;
		bool settingInternally;
		LiveFeedAnimator liveFeedAnimator;
		int digitCount;
		void LoadAnimation(string selectedPath)
		{
			string movementFileName = System.IO.Path.GetFileName(selectedPath);
			LoadAllFrames(movementFileName);
			LoadAllImages(selectedPath);
			UpdateUI();
			frameIndex = 0;
			DrawActiveFrame();
			ClearEditorValues();
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
			LiveFeedEdit liveFeedEdit = allFrames[frameIndex];
			
			liveFeedAnimator.ScreenAnchorLeft = liveFeedEdit.GetX();
			liveFeedAnimator.ScreenAnchorTop = liveFeedEdit.GetY();

			float scale = (float)liveFeedEdit.Scale;
			double opacity = liveFeedEdit.Opacity;
			double rotation = liveFeedEdit.Rotation;
			if (liveFeedEdit.RotationOverride.HasValue)
				rotation = liveFeedEdit.RotationOverride.Value;
			if (liveFeedEdit.ScaleOverride.HasValue)
				scale = (float)liveFeedEdit.ScaleOverride.Value;
			if (liveFeedEdit.OpacityOverride.HasValue)
				opacity = liveFeedEdit.OpacityOverride.Value;

			ObsControl.ObsManager.SizeAndPositionItem(liveFeedAnimator, scale, opacity, rotation);
		}

		void Change(Attribute attribute, double value)
		{
			if (initializing || allFrames == null)
				return;
			LiveFeedEdit liveFeedEdit = allFrames[frameIndex];
			switch (attribute)
			{
				case Attribute.X:
					liveFeedEdit.DeltaX = (int)value;
					break;
				case Attribute.Y:
					liveFeedEdit.DeltaY = (int)value;
					break;
				case Attribute.Scale:
					liveFeedEdit.ScaleOverride = value;
					break;
				case Attribute.Rotation:
					liveFeedEdit.RotationOverride = value;
					break;
				case Attribute.Opacity:
					liveFeedEdit.OpacityOverride = value;
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

		private void LoadAllFrames(string movementFileName)
		{
			VideoAnimationBinding binding = AllVideoBindings.Get(movementFileName);
			string fullPathToMovementFile = VideoAnimationManager.GetFullPathToMovementFile(movementFileName);
			VideoFeed videoFeed = AllVideoFeeds.Get(binding.SourceName);
			liveFeedAnimator = VideoAnimationManager.LoadLiveAnimation(fullPathToMovementFile, binding, videoFeed);
			allFrames = new List<LiveFeedEdit>();
			foreach (LiveFeedSequence liveFeedSequence in liveFeedAnimator.LiveFeedSequences)
			{
				const double frameRate = 29.97;  // fps
				const double secondsPerFrame = 1 / frameRate;
				int frameCount = (int)(liveFeedSequence.Duration / secondsPerFrame);
				for (int i = 0; i < frameCount; i++)
					allFrames.Add(liveFeedSequence.CreateLiveFeedEdit());
			}
		}

		private void btnLoadAnimation_Click(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			folderBrowserDialog.SelectedPath = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\Editor";
			if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				LoadAnimation(folderBrowserDialog.SelectedPath);
			}
		}

		private void btnJumpToPreviousDelta_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btnJumpToNextDelta_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btnPreviousFrame_Click(object sender, RoutedEventArgs e)
		{
			FrameIndex--;
		}


		void PreloadAroundActiveFrame(int extraFramesCount)
		{
			int startFrame = Math.Max(0, frameIndex - extraFramesCount);
			int lastIndex = backFiles.Length - 1;
			int endFrame = Math.Min(lastIndex, frameIndex + extraFramesCount);

			// TODO: Calculate these once on animation load.
			string relativePathFront = GetRelativePathBaseName(frontFiles[frameIndex]);
			string relativePathBack = GetRelativePathBaseName(backFiles[frameIndex]);

			if (digitCount == 0)
			{
				digitCount = lastIndex.ToString().Length;
			}

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
		}
	}
}
