using Newtonsoft.Json;
using OBSWebsocketDotNet.Types;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Imaging;
using ObsControl;

namespace DHDM
{
	public static class VideoAnimationManager
	{
		const string STR_AnimationEditor = "Animation Editor";
		static bool existingAnimationIsRunning;
		static List<LiveFeedAnimator> liveFeedAnimators;
		static System.Timers.Timer animationEditorTimer = new System.Timers.Timer();
		static FrmLiveAnimationEditor frmLiveAnimationEditor;

		static VideoAnimationManager()
		{
			animationEditorTimer.Interval = 1;
			animationEditorTimer.Elapsed += AnimationEditorTimer_Elapsed;
		}

		static void StopExistingAnimation()
		{
			if (!existingAnimationIsRunning)
				return;

			existingAnimationIsRunning = false;
			// TODO: Restore defaultX, defaultY, videoWidth and videoHeight.
			if (liveFeedAnimators != null)
				foreach (LiveFeedAnimator liveFeedAnimator in liveFeedAnimators)
					liveFeedAnimator.StopSoon();
		}

		public static List<LiveFeedAnimator> LoadLiveAnimation(string movementFile, VideoAnimationBinding binding, VideoFeed[] videoFeeds)
		{
			if (!File.Exists(movementFile))
				return null;
			string movementInstructions = File.ReadAllText(movementFile);
			List<ObsTransform> liveFeedFrames = JsonConvert.DeserializeObject<List<ObsTransform>>(movementInstructions);
			liveFeedAnimators = new List<LiveFeedAnimator>();
			LiveFeedAnimator liveFeedAnimator = new LiveFeedAnimator(videoFeeds, liveFeedFrames);
			liveFeedAnimators.Add(liveFeedAnimator);
			liveFeedAnimator.StartTimeOffset = binding.StartTimeOffset;
			liveFeedAnimator.TimeStretchFactor = binding.TimeStretchFactor;
			return liveFeedAnimators;
		}

		private static void LiveFeedAnimator_AnimationComplete(object sender, EventArgs e)
		{
			if (sender == liveFeedAnimators && liveFeedAnimators != null)
				liveFeedAnimators = null;
		}

		static void StartLiveAnimation(string sceneName, List<VideoAnimationBinding> bindings, DateTime startTime)
		{
			foreach (VideoAnimationBinding videoAnimationBinding in bindings)
			{
				string movementFile = GetFullPathToMovementFile(videoAnimationBinding.MovementFileName);
				VideoFeed[] videoFeeds = AllVideoFeeds.GetAll(videoAnimationBinding);

				List<LiveFeedAnimator> LiveFeedAnimators = LoadLiveAnimation(movementFile, videoAnimationBinding, videoFeeds);
				foreach (LiveFeedAnimator liveFeedAnimator in LiveFeedAnimators)
				{
					liveFeedAnimator.AnimationComplete += LiveFeedAnimator_AnimationComplete;
					liveFeedAnimator.Start(startTime);
				}
			}

			// TODO: Track which video feed we are running in case we abort while we are running.

			existingAnimationIsRunning = true;
		}

		public static string GetFullPathToMovementFile(string movementFileName)
		{
			string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			return Path.Combine(location, "LiveVideoAnimation\\Data", movementFileName + ".movement");
		}

		private static void ObsManager_SceneChanged(object sender, string sceneName)
		{
			DateTime startTime = DateTime.Now;
			StopExistingAnimation();

			List<VideoAnimationBinding> bindings = AllVideoBindings.GetAll(sceneName);

			if (bindings != null)
				StartLiveAnimation(sceneName, bindings, startTime);

			if (sceneName == STR_AnimationEditor)
			{
				animationEditorTimer.Start();
			}
			else
			{
				HubtasticBaseStation.PreloadImageBack(null, 0, 0, 0);
				HubtasticBaseStation.PreloadImageFront(null, 0, 0, 0);
				HubtasticBaseStation.ShowImageFront(null);
				HubtasticBaseStation.ShowImageBack(null);
			}
		}

		private static void AnimationEditorTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			animationEditorTimer.Stop();

			System.Windows.Application.Current.Dispatcher.Invoke(() => 
			{
				if (frmLiveAnimationEditor == null)
					frmLiveAnimationEditor = new FrmLiveAnimationEditor();

				frmLiveAnimationEditor.Show();
			});

			// image_source
			//HubtasticBaseStation.ShowImageFront(@"Editor/FrontTest.png");
			//HubtasticBaseStation.ShowImageBack(@"Editor/BackTest.png");
		}

		public static void Initialize()
		{
			ObsManager.SceneChanged += ObsManager_SceneChanged;
		}
		public static void ClosingEditor()
		{
			frmLiveAnimationEditor = null;
		}
	}
}
