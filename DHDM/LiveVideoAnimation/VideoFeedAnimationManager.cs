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
	public static class VideoFeedAnimationManager
	{
		static bool existingAnimationIsRunning;
		static LiveFeedAnimator liveFeedAnimator;

		static VideoFeedAnimationManager()
		{

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
		}

		public static void Initialize()
		{
			ObsManager.SceneChanged += ObsManager_SceneChanged;
		}

		
	}
}
