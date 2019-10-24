using System;
using System.Linq;

namespace DndCore
{
	public class Rechargeable
	{
		public string DisplayName;
		public string VarName;
		public int MaxValue;
		public int ChargesUsed;
		public DndTimeSpan Cycle;

		public Rechargeable()
		{

		}

		public Rechargeable(string displayName, string varName, int maxValue, string cycle)
		{
			DisplayName = displayName;
			VarName = varName;
			MaxValue = maxValue;
			Cycle = DndTimeSpan.FromDurationStr(cycle);
		}
	}
}