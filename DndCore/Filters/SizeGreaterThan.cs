using System;
using System.Linq;

namespace DndCore
{
	public class SizeGreaterThan : ComparisonFilter
	{
		public SizeGreaterThan(CreatureSize creatureSize)
		{
			CreatureSize = creatureSize;
		}

		public override bool IsTrue(Creature creature)
		{
			return creature.creatureSize > CreatureSize;
		}

		public CreatureSize CreatureSize { get; set; }
	}
}
