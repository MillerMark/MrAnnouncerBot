using System;
using System.Linq;

namespace DndCore
{
	public class IndividualRoll
	{
		public int value { get; set; }
		public int numSides { get; set; }
		public int modifier { get; set; }
		public string type { get; set; }
		public DieCountsAs dieCountsAs { get; set; }
		public int creatureId { get; set; }
		public DamageType damageType { get; set; }
		public IndividualRoll()
		{

		}
	}
}
