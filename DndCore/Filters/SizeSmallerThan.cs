using System;
using System.Linq;
using DndCore.Enums;

namespace DndCore.Filters
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
