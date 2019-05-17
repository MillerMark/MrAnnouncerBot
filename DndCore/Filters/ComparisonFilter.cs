using System;
using System.Linq;

namespace DndCore.Filters
{
	public abstract class ComparisonFilter
	{
		public abstract bool IsTrue(Creature creature);
	}
}
