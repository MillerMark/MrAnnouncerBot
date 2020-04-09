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
			return character;
		}

		public static Character GetLilCutiePaladin()
		{
			Character player = GetPlayerAtLevel("Paladin", 6);
			player.spellCastingAbility = Ability.charisma;
			player.baseCharisma = 19;
			return player;
		}
	}
}
