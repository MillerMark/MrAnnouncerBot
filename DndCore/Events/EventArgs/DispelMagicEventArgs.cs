using System;
using System.Linq;

namespace DndCore
{
	public class DispelMagicEventArgs : EventArgs
	{
		public DispelMagicEventArgs(CreaturePlusModId recipient)
		{
			Recipient = recipient;
		}
		public CreaturePlusModId Recipient { get; set; }

	}
}
