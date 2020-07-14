using System;
using System.Linq;

namespace DndCore
{
	public class VantageMod
	{
		public DiceRollType RollType { get; set; }
		public Skills Detail { get; set; }
		public int Offset { get; set; }
		public string DieLabel { get; set; }

		public VantageMod(DiceRollType rollType, Skills detail, string dieLabel, int offset)
		{
			Set(rollType, detail, dieLabel, offset);
		}

		public void Set(DiceRollType rollType, Skills detail, string dieLabel, int offset)
		{
			RollType = rollType;
			Detail = detail;
			DieLabel = dieLabel;
			Offset = offset;
		}
	}
}