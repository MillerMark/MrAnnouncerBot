using System;
using System.Linq;

namespace DndCore
{
	public class Paladin : BaseRow
	{
		public int Level { get; set; }
		public int ProficiencyBonus { get; set; }
		public string Features { get; set; }
		public int Slot1Spells { get; set; }
		public int Slot2Spells { get; set; }
		public int Slot3Spells { get; set; }
		public int Slot4Spells { get; set; }
		public int Slot5Spells { get; set; }

		public Paladin()
		{

		}
	}
}

