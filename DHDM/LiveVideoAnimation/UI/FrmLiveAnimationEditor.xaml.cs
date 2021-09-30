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
			InitializeComponent();
		}

		
		List<LiveFeedData> allFrames;
		string[] backFiles;
		string[] frontFiles;
		int frameIndex;
		bool settingInternally;
		LiveFeedAnimator liveFeedAnimator;
		void LoadAnimation(string selectedPath)
		{
			string movementFileName = System.IO.Path.GetFileName(selectedPath);
			LoadAllFrames(movementFileName);
			LoadAllImages(selectedPath);
			UpdateUI();
			frameIndex = 0;
			DrawActiveFrame();
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
			LiveFeedData liveFeedData = allFrames[frameIndex];
			liveFeedAnimator.ScreenAnchorLeft = liveFeedData.Origin.X;
			liveFeedAnimator.ScreenAnchorTop = liveFeedData.Origin.Y;
			ObsControl.ObsManager.SizeAndPositionItem(liveFeedAnimator, (float)liveFeedData.Scale, liveFeedData.Opacity, liveFeedData.Rotation);
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
			allFrames = new List<LiveFeedData>();
			foreach (LiveFeedSequence liveFeedSequence in liveFeedAnimator.LiveFeedSequences)
			{
				const double frameRate = 29.97;  // fps
				const double secondsPerFrame = 1 / frameRate;
				int frameCount = (int)(liveFeedSequence.Duration / secondsPerFrame);
				for (int i = 0; i < frameCount; i++)
					allFrames.Add(liveFeedSequence.Clone());
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
	}
}
