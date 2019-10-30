using System;
using System.Linq;

namespace DndCore
{
	public class Rechargeable
	{
		public string DisplayName;
		public string VarName;
		public int TotalCharges;
		public int ChargesUsed;
		public DndTimeSpan Cycle;

		public Rechargeable()
		{

		}

		public int ChargesRemaining
		{
			get
			{
				return TotalCharges - ChargesUsed;
			}
		}
		public bool AddedToUI { get; set; }

		public void SetRemainingCharges(int numChargesRemaining)
		{
			ChargesUsed = TotalCharges - numChargesRemaining;
		}

		public Rechargeable(string displayName, string varName, int maxValue, string cycle)
		{
			DisplayName = displayName;
			VarName = varName;
			TotalCharges = maxValue;
			Cycle = DndTimeSpan.FromDurationStr(cycle);
		}
	}
}