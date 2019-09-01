using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllSpells
	{

		static AllSpells()
		{
			Spells = CsvData.Get<SpellDto>(Folders.InCoreData("DnD - Spells.csv"));
		}

		public static List<SpellDto> Spells { get; private set; }

		public static Spell Get(string spellName, int spellSlotLevel = 0, int spellCasterLevel = 0, int spellcastingAbilityModifier = int.MinValue)
		{
			SpellDto spell = Spells.FirstOrDefault(x => string.Compare(x.name, spellName, true) == 0);
			if (spell != null)
				return Spell.FromDto(spell, spellSlotLevel, spellCasterLevel, spellcastingAbilityModifier);
			return null;
		}
		public static Spell Get(string spellName, Character character, int spellSlotLevel = 0)
		{
			return Get(spellName, spellSlotLevel, character.level, character.GetSpellcastingAbilityModifier());
		}
	}
}
