using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DndCore
{
	public static class AllSpells
	{
		public static void Invalidate()
		{
			spells = null;
		}
		static void LoadData()
		{
			Spells = CsvToSheetsHelper.Get<SpellDto>(Folders.InCoreData("DnD - Spells.csv"));
		}

		static List<SpellDto> spells;
		public static List<SpellDto> Spells
		{
			get
			{
				if (spells == null)
					LoadData();
				return spells;
			}
			private set
			{
				spells = value;
			}
		}

		public static Spell Get(string spellName, int spellSlotLevel = -1, int spellCasterLevel = 0, int spellcastingAbilityModifier = int.MinValue)
		{
			if (string.IsNullOrEmpty(spellName))
				return null;
			SpellDto spell = GetDto(spellName);
			return Spell.FromDto(spell, spellSlotLevel, spellCasterLevel, spellcastingAbilityModifier);
		}

		public static SpellDto GetDto(string spellName)
		{
			return Spells.FirstOrDefault(x => string.Compare(x.name, spellName, true) == 0);
		}

		public static List<Spell> GetAll(string spellName, int spellSlotLevel = -1, int spellCasterLevel = 0, int spellcastingAbilityModifier = int.MinValue)
		{
			spellName = spellName.Trim();
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
			return Get(spellName, spellSlotLevel, character.GetSpellcastingLevel(), character.GetSpellcastingAbilityModifier());
		}
		public static List<Spell> GetAll(string spellName, Character character, int spellSlotLevel = 0)
		{
			int spellLevel = 0;
			int spellcastingAbilityModifier = 0;
			if (character != null)
			{
				spellLevel = character.GetSpellcastingLevel();
				spellcastingAbilityModifier = character.GetSpellcastingAbilityModifier();
			}
			return GetAll(spellName, spellSlotLevel, spellLevel, spellcastingAbilityModifier);
		}

		public static TimeSpan GetDuration(string spellName)
		{
			Spell spell = Get(spellName);
			if (spell == null)
				return TimeSpan.Zero;
			return spell.Duration.GetTimeSpan();
		}
	}
}
