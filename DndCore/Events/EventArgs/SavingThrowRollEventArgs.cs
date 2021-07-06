using System;

namespace DndCore
{
	public class SavingThrowRollEventArgs : EventArgs
	{
		public string DamageDieStr { get; private set; }
		public Conditions Condition { get; private set; }
		public Ability Ability { get; private set; }
		public string SpellGuid { get; private set; }
		
		public SavingThrowRollEventArgs(string damageDieStr, Conditions condition, Ability ability, string spellGuid)
		{
			SpellGuid = spellGuid;
			DamageDieStr = damageDieStr;
			Condition = condition;
			Ability = ability;
		}
		public SavingThrowRollEventArgs()
		{

		}
	}
}
