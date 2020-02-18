using System;

namespace DndCore
{
	public class ShortcutEventArgs : EventArgs
	{

		public ShortcutEventArgs(PlayerActionShortcut shortcut, int delayMs = 0)
		{
			DelayMs = delayMs;
			Shortcut = shortcut;
		}

		public PlayerActionShortcut Shortcut { get; private set; }
		public int DelayMs { get; set; }
	}
}

