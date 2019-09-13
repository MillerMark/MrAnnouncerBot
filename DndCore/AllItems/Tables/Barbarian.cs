using System;
using System.Linq;

namespace DndCore
{
	public class Barbarian : BaseRow
	{
		public int Level { get; set; }
		public int ProficiencyBonus { get; set; }
		public string Features { get; set; }
		public int Rages { get; set; }
		public int RageDamage { get; set; }
		public Barbarian()
		{

		}
	}
}

