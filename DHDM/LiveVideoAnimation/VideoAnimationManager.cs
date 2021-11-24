using Newtonsoft.Json;
using OBSWebsocketDotNet.Types;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Imaging;
using ObsControl;
using Windows.UI.Composition.Scenes;

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
					liveFeedAnimator.FrameAnimator.StopSoon();
		}

		public static LiveFeedAnimator LoadLiveAnimation(string movementFile, VideoAnimationBinding binding, VideoFeed[] videoFeeds)
		{
			if (!File.Exists(movementFile))
				return null;
			string movementInstructions = File.ReadAllText(movementFile);
			List<ObsTransform> liveFeedFrames = JsonConvert.DeserializeObject<List<ObsTransform>>(movementInstructions);
			LiveFeedAnimator liveFeedAnimator = new LiveFeedAnimator(videoFeeds, liveFeedFrames);
			liveFeedAnimator.FrameAnimator.StartTimeOffset = binding.StartTimeOffset;
			liveFeedAnimator.FrameAnimator.TimeStretchFactor = binding.TimeStretchFactor;

			return liveFeedAnimator;
		}

		private static void LiveFeedAnimator_AnimationComplete(object sender, EventArgs e)
		{
			if (sender is LiveFeedAnimator liveFeedAnimator)
			{
				liveFeedAnimator.FrameAnimator.AnimationComplete -= LiveFeedAnimator_AnimationComplete;
				if (liveFeedAnimators != null)
					liveFeedAnimators.Remove(liveFeedAnimator);
			}
		}

		static void CleanUpLiveFeedAnimators()
		{
			if (liveFeedAnimators == null || liveFeedAnimators.Count == 0)
				return;

			List<LiveFeedAnimator> copiedLiveFeedAnimators = new List<LiveFeedAnimator>(liveFeedAnimators);

			foreach (LiveFeedAnimator liveFeedAnimator in copiedLiveFeedAnimators)
				liveFeedAnimator.FrameAnimator.Stop();
		}

		static void StartLiveAnimation(string sceneName, List<VideoAnimationBinding> bindings, DateTime startTime)
		{
			CleanUpLiveFeedAnimators();
			liveFeedAnimators = new List<LiveFeedAnimator>();
			foreach (VideoAnimationBinding videoAnimationBinding in bindings)
			{
				string movementFile = GetFullPathToMovementFile(videoAnimationBinding.MovementFileName);
				VideoFeed[] videoFeeds = AllVideoFeeds.GetAll(videoAnimationBinding);

				LiveFeedAnimator liveFeedAnimator = LoadLiveAnimation(movementFile, videoAnimationBinding, videoFeeds);
				liveFeedAnimator.FrameAnimator.AnimationComplete += LiveFeedAnimator_AnimationComplete;
				liveFeedAnimator.FrameAnimator.Start(startTime);
				liveFeedAnimators.Add(liveFeedAnimator);
			}

			// TODO: Track which video feed we are running in case we abort while we are running.

			string fullPathToLightsFile = VideoAnimationManager.GetFullPathToLightsFile(sceneName);
			if (System.IO.File.Exists(fullPathToLightsFile))
			{
				string lightsJson = System.IO.File.ReadAllText(fullPathToLightsFile);
				LightingSequence lightingSequence = JsonConvert.DeserializeObject<LightingSequence>(lightsJson);
				// TODO: We need a LightAnimator that is a lot like a LiveFeedAnimator
			}

			existingAnimationIsRunning = true;
		}

		public static string GetFullPathToMovementFile(string movementFileName)
		{
			return GetFullPathToDataFile(movementFileName, ".movement");
		}

		public static string GetFullPathToLightsFile(string lightsFileName)
		{
			return GetFullPathToDataFile(lightsFileName, ".lights");
		}

		private static string GetFullPathToDataFile(string movementFileName, string extension)
		{
			string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			try
			{
				return Path.Combine(location, "LiveVideoAnimation\\Data", movementFileName + extension);
			}
			catch
			{
				return null;
			}
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
