using System;
using System.Linq;

namespace DndCore
{
	public class LevelChangedEventArgs : EventArgs
	{
		public int OldLevel { get; private set; }
		public int NewLevel { get; private set; }
		public LevelChangedEventArgs(int oldLevel, int newLevel)
		{
			OldLevel = oldLevel;
			NewLevel = newLevel;
		}
	}
}