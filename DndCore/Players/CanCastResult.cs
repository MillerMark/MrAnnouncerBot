using System;
using System.Linq;

namespace DndCore
{
	public class CanCastResult
	{

		public CanCastResult()
		{

		}

		public CanCastResult(Vector location, int range)
		{
			Location = location;
			Range = range;
		}

		public int Range { get; set; }
		public Vector Location { get; set; }

		public bool At(Creature creature)
		{
			return DistanceTo(creature) <= Range;
		}

		public double DistanceTo(Creature creature)
		{
			return Location.DistanceTo(creature.Location);
		}
	}
}