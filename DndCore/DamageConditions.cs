using DndCore;

namespace DndCore
{
	public class DamageConditions
	{
		public DamageConditions(Conditions conditions, CreatureSize creatureSizeFilter, int escapeDC, int concurrentTargets)
		{
			ConcurrentTargets = concurrentTargets;
			EscapeDC = escapeDC;
			CreatureSizeFilter = creatureSizeFilter;
			Conditions = conditions;
		}

		public int ConcurrentTargets { get; set; }

		public Conditions Conditions { get; set; }
		public CreatureSize CreatureSizeFilter { get; set; }
		public int EscapeDC { get; set; }
	}
}
