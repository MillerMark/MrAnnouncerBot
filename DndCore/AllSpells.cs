using System;
using System.Collections.Generic;
using System.Linq;

namespace DndCore
{
	public static class AllSpells
	{
		static List<SpellDto> spells;
		static AllSpells()
		{
			spells = CsvData.Get<SpellDto>("Data/dnd spells - all spells.csv");
		}
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
		public static List<SpellDto> Spells { get => spells; private set => spells = value; }
	}
}
