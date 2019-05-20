using System;
using System.Linq;

namespace DndCore
{
	public class SizeSmallerThan : ComparisonFilter
	{
		public SizeSmallerThan(CreatureSize creatureSize)
		{
			CreatureSize = creatureSize;
		}

		public CreatureSize CreatureSize { get; set; }

		public override bool IsTrue(Creature creature)
		{
			return creature.creatureSize < CreatureSize;
		}
	}
}
