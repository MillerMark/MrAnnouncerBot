using System;
using System.Linq;

namespace DndCore
{
	public class CreaturesChoice : ComparisonFilter
	{
		public CreaturesChoice(int maxTargets = int.MaxValue)
		{
			MaxTargets = maxTargets;
		}

		public int MaxTargets { get; set; }

		public override bool IsTrue(Creature creature)
		{
			if (MaxTargets == int.MaxValue)
				return true;

			// TODO: Solve this interactively, or through UI for the DM to select targets?
			return false;
		}
	}
}
