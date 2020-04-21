using System;
using DndCore;

namespace DndTests
{
	public static class PlayerHelper
	{

		public static Character GetPlayerAtLevel(string className, int spellCasterLevel)
		{
			Character character = new Character();
			character.AddClass(className, spellCasterLevel);
			character.playingNow = true;
			return character;
		}

		public static Character GetPaladin(int level, int baseCharisma)
		{
			Character player = GetPlayerAtLevel("Paladin", level);
			player.spellCastingAbility = Ability.charisma;
			player.baseCharisma = baseCharisma;
			return player;
		}

		public static Character GetSorcerer(int level, int baseCharisma, int proficiencyBonus, params string[] knownSpells)
		{
			Character player = GetPlayerAtLevel("Sorcerer", level);
			player.proficiencyBonus = proficiencyBonus;
			player.spellCastingAbility = Ability.charisma;
			player.baseCharisma = baseCharisma;
			foreach (string spellName in knownSpells)
			{
				player.AddSpell(spellName);
			}
			
			return player;
		}
	}
}
