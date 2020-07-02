using System;

namespace DndCore
{
	public class MagicEventArgs : EventArgs
	{
		public MagicEventArgs(Magic magic)
		{
			Magic = magic;
		}

		public Magic Magic { get; set; }
	}
}
