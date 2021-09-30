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
		static LiveFeedAnimator liveFeedAnimator;
		static System.Timers.Timer animationEditorTimer = new System.Timers.Timer();

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
			if (liveFeedAnimator != null)
				liveFeedAnimator.StopSoon();
		}

		static void LoadLiveAnimation(string movementFile, VideoAnimationBinding binding, VideoFeed videoFeed, DateTime startTime)
		{
			if (!File.Exists(movementFile))
				return;
			string movementInstructions = File.ReadAllText(movementFile);
			List<LiveFeedFrame> liveFeedFrames = JsonConvert.DeserializeObject<List<LiveFeedFrame>>(movementInstructions);
			liveFeedAnimator = new LiveFeedAnimator(videoFeed.videoAnchorHorizontal, videoFeed.videoAnchorVertical, videoFeed.videoWidth, videoFeed.videoHeight, videoFeed.sceneName, videoFeed.sourceName, liveFeedFrames);
			liveFeedAnimator.AnimationComplete += LiveFeedAnimator_AnimationComplete;
			liveFeedAnimator.Start(startTime);
		}

		private static void LiveFeedAnimator_AnimationComplete(object sender, EventArgs e)
		{
			if (sender == liveFeedAnimator && liveFeedAnimator != null)
				liveFeedAnimator = null;
		}

		static void StartLiveAnimation(string sceneName, VideoAnimationBinding binding, DateTime startTime)
		{
			string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string movementFile = Path.Combine(location, "LiveVideoAnimation\\Data", binding.MovementFileName + ".movement");
			VideoFeed videoFeed = AllVideoFeeds.Get(binding.SourceName);
			LoadLiveAnimation(movementFile, binding, videoFeed, startTime);

			// TODO: Track which video feed we are running in case we abort while we are running.

			existingAnimationIsRunning = true;
		}

		private static void ObsManager_SceneChanged(object sender, string sceneName)
		{
			DateTime startTime = DateTime.Now;
			StopExistingAnimation();

			VideoAnimationBinding binding = AllVideoBindings.Get(sceneName);

			if (binding != null)
				StartLiveAnimation(sceneName, binding, startTime);

			if (sceneName == STR_AnimationEditor)
			{
				animationEditorTimer.Start();
			}
			else
			{
				HubtasticBaseStation.ShowImageFront(null);
				HubtasticBaseStation.ShowImageBack(null);
			}
		}

		private static void AnimationEditorTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			animationEditorTimer.Stop();

			// image_source
			HubtasticBaseStation.ShowImageFront(@"Editor/FrontTest.png");
			HubtasticBaseStation.ShowImageBack(@"Editor/BackTest.png");
		}

		public static void Initialize()
		{
			ObsManager.SceneChanged += ObsManager_SceneChanged;
		}
	}
}
