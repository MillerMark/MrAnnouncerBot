using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public static class AllSpells
	{

		static AllSpells()
		{
			LoadData();
		}

		public static void LoadData()
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

		public static List<Spell> GetAll(string spellName, int spellSlotLevel = 0, int spellCasterLevel = 0, int spellcastingAbilityModifier = int.MinValue)
		{
			List<Spell> result = new List<Spell>();
			
			List<SpellDto> spells = Spells.Where(x => string.Compare(x.name, spellName, true) == 0).ToList();
			if (spells != null)
				foreach (SpellDto spellDto in spells)
				{
					result.Add(Spell.FromDto(spellDto, spellSlotLevel, spellCasterLevel, spellcastingAbilityModifier));
				}

			return result;
		}
		public static Spell Get(string spellName, Character character, int spellSlotLevel = 0)
		{
			return Get(spellName, spellSlotLevel, character.level, character.GetSpellcastingAbilityModifier());
		}
		public static List<Spell> GetAll(string spellName, Character character, int spellSlotLevel = 0)
		{
			return GetAll(spellName, spellSlotLevel, character.level, character.GetSpellcastingAbilityModifier());
		}
	}
}
