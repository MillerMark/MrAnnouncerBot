using System;
using System.Linq;

namespace DndCore
{
	public abstract class ComparisonFilter
	{
		public abstract bool IsTrue(Creature creature);
	}
}
