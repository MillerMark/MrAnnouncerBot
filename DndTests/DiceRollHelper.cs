using System;
using DndCore;

namespace DndTests
{
	public static class DiceRollHelper
	{

		public static DiceRoll GetSpellFrom(string spellName, Character player, int spellSlotLevel = -1)
		{
			
			PlayerActionShortcut shortcut = PlayerActionShortcut.FromSpell(spellName, player, spellSlotLevel);
			return DiceRoll.GetFrom(shortcut);
		}
	}
}
