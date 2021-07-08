using System;

namespace DndCore
{
	public class SavingThrowRollEventArgs : EventArgs
	{
		public string DamageDieStr { get; private set; }
		public Conditions Condition { get; private set; }
		public Ability Ability { get; private set; }
		public CastedSpell CastedSpell { get; private set; }
		
		public SavingThrowRollEventArgs(string damageDieStr, Conditions condition, Ability ability, CastedSpell spellGuid)
		{
			CastedSpell = spellGuid;
			DamageDieStr = damageDieStr;
			Condition = condition;
			Ability = ability;
		}

		public SavingThrowRollEventArgs()
		{

		}
	}
}
