using System;
using System.Linq;

namespace DndCore
{
	public class ArcaneTrickster : BaseRow
	{
		public int Level { get; set; }
		public int CantripsKnown { get; set; }
		public int SpellsKnown { get; set; }
		public int Slot1Spells { get; set; }
		public int Slot2Spells { get; set; }
		public int Slot3Spells { get; set; }
		public int Slot4Spells { get; set; }

		public ArcaneTrickster()
		{

		}
	}
}

