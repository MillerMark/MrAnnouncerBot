using System;

namespace DndCore
{
	public class PlaySceneEventArgs : EventArgs
	{

		public PlaySceneEventArgs(string sceneName)
		{
			SceneName = sceneName;
		}

		public string SceneName { get; set; }
	}
}
