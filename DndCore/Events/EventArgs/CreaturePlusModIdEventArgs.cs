using System;
using System.Linq;

namespace DndCore
{
	public class CreaturePlusModIdEventArgs : EventArgs
	{
		public CreaturePlusModIdEventArgs(CreaturePlusModId creaturePlusModId)
		{
			CreaturePlusModId = creaturePlusModId;
		}
		public CreaturePlusModId CreaturePlusModId { get; set; }
	}
}
