using System;

namespace DndCore
{
	public class PlaySceneEventArgs : EventArgs
	{
		public PlaySceneEventArgs(string sceneName, int delayMs)
		{
			DelayMs = delayMs;
			SceneName = sceneName;
		}

		public string SceneName { get; set; }
		public int DelayMs { get; set; }
	}
}
