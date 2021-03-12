using System;

namespace DndCore
{
	public class ObsSceneFilterEventArgs : EventArgs
	{
		public string SourceName { get; set; }
		public string FilterName { get; set; }
		public bool FilterEnabled { get; set; }
		public int DelayMs { get; set; }

		public ObsSceneFilterEventArgs(string sourceName, string filterName, bool filterEnabled, int delayMs)
		{
			DelayMs = delayMs;
			SourceName = sourceName;
			FilterName = filterName;
			FilterEnabled = filterEnabled;
		}
	}
}
