using System;
using System.Linq;

namespace DndCore
{
	public class DiceRoll
	{
		public DiceRollType Type { get; set; }
		public DiceRollKind Kind { get; set; }
		public string DamageDice { get; set; }
		public double Modifier { get; set; }
		public double HiddenThreshold { get; set; }
		public bool IsMagic { get; set; }

		public DiceRoll(DiceRollKind kind, string damageDice)
		{
			DamageDice = damageDice;
			Kind = kind;
		}
	}
}
