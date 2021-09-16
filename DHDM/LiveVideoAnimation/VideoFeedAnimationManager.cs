using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DHDM
{
	public static class VideoFeedAnimationManager
	{
		static bool existingAnimationIsRunning;

		static VideoFeedAnimationManager()
		{
			
		}

		static void StopExistingAnimation()
		{
			// TODO: Restore defaultX, defaultY, videoWidth and videoHeight.
		}
		static void StartLiveAnimation(string sceneName, VideoAnimationBinding binding)
		{
			if (existingAnimationIsRunning)
			{
				StopExistingAnimation();
			}

			string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string movementFile = Path.Combine(location, "LiveVideoAnimation\\Data", binding.MovementFileName + ".movement");
			if (File.Exists(movementFile))
			{
				// TODO: Load up the movement file, and get the animation going!
				// We are close!!!
			}

			// TODO: Track which video feed we are running in case we can an abort while we are running.

			existingAnimationIsRunning = true;
		}

		private static void ObsManager_SceneChanged(object sender, string sceneName)
		{
			VideoAnimationBinding binding = AllVideoBindings.Get(sceneName);

			if (binding != null)
				StartLiveAnimation(sceneName, binding);
		}

		public static void Initialize()
		{
			ObsManager.SceneChanged += ObsManager_SceneChanged;
		}
	}
}
