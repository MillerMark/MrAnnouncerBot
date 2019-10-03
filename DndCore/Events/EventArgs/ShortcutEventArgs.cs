using System;

namespace DndCore
{
	public class ShortcutEventArgs : EventArgs
	{

		public ShortcutEventArgs(PlayerActionShortcut shortcut)
		{
			Shortcut = shortcut;
		}

		public PlayerActionShortcut Shortcut { get; private set; }
	}
}

