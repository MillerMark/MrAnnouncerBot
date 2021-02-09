using System;
using System.Linq;

namespace DndCore
{
	public class QueueShortcutEventArgs : EventArgs
	{
		public QueueShortcutEventArgs(Creature player, string shortcutName, bool rollImmediately)
		{
			Player = player;
			RollImmediately = rollImmediately;
			ShortcutName = shortcutName;
		}

		public string ShortcutName { get; set; }
		public bool RollImmediately { get; set; }
		public Creature Player { get; set; }
	}
}
