namespace DndCore
{
	public class DamageConditions
	{
		public DamageConditions(Conditions conditions, ComparisonFilter comparisonFilter, int escapeDC, int concurrentTargets)
		{
			ConcurrentTargets = concurrentTargets;
			EscapeDC = escapeDC;
			ComparisonFilter = comparisonFilter;
			Conditions = conditions;
		}

		public Conditions Conditions { get; set; }
		public ComparisonFilter ComparisonFilter { get; set; }
		public int EscapeDC { get; set; }
		public int ConcurrentTargets { get; set; }
	}
}
