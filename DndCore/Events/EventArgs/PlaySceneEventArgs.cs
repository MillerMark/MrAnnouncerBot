using System;

namespace DndCore
{
	public class PlaySceneEventArgs : EventArgs
	{
		public PlaySceneEventArgs(string sceneName, double delay, double returnSec)
		{
			// TODO: Clean this up after all delays have been converted to seconds in the Spell event handlers.
			if (delay > 400)
				DelayMs = (int)Math.Round(delay);
			else
				DelayMs = (int)Math.Round(delay * 1000);
			ReturnMs = (int)Math.Round(returnSec * 1000);
			SceneName = sceneName;
		}

		public string SceneName { get; set; }
		public int DelayMs { get; set; }
		public int ReturnMs { get; set; }
	}
}
