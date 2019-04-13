using System;
using System.Linq;

namespace DndCore
{
	public class DiceRoll
	{
		public DiceRollKind Kind { get; set; }
		public string DamageDice { get; set; }
		public DiceRoll(DiceRollKind kind, string damageDice)
		{
			DamageDice = damageDice;
			Kind = kind;
		}
	}
}
