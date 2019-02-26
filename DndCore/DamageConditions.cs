namespace DndCore
{
	public class DamageConditions
	{
		public DamageConditions(Conditions conditions, ComparisonFilterOption comparisonFilterOption, CreatureSize sizeFilter, int escapeDC, int concurrentTargets)
		{
			ConcurrentTargets = concurrentTargets;
			EscapeDC = escapeDC;
			CreatureSizeFilter = sizeFilter;
			Conditions = conditions;
		}

		public Conditions Conditions { get; set; }
		public CreatureSize CreatureSizeFilter { get; set; }
		public int EscapeDC { get; set; }
		public int ConcurrentTargets { get; set; }
	}
}
