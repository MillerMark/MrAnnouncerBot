using System;

namespace DndCore
{
	public class LiveFeedEventArgs : EventArgs
	{
		public Creature Player { get; set; }
		public double targetScale { get; set; }
		public double TimeMs { get; set; }
		public LiveFeedEventArgs(Creature player, double targetSize, double timeMs)
		{
			Player = player;
			targetScale = targetSize;
			TimeMs = timeMs;
		}
		public LiveFeedEventArgs()
		{

		}
	}
}
