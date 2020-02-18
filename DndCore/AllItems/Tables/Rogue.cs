using System;
using System.Linq;

namespace DndCore
{
	public class Rogue : BaseRow
	{
		public int Level { get; set; }
		public int ProficiencyBonus { get; set; }
		public string SneakAttack { get; set; }
		public string Features { get; set; }
		public Rogue()
		{

		}
	}
}

