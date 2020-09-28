using System;

namespace DndCore
{
	public class SetObsSourceVisibilityEventArgs : EventArgs
	{
		public string SourceName { get; set; }
		public string SceneName { get; set; }
		public bool Visible { get; set; }
		public double DelaySeconds { get; set; }

		public SetObsSourceVisibilityEventArgs(string sourceName, string sceneName, bool visible, double delaySeconds)
		{
			SourceName = sourceName;
			SceneName = sceneName;
			Visible = visible;
			DelaySeconds = delaySeconds;
		}

		public SetObsSourceVisibilityEventArgs()
		{

		}
	}
}
