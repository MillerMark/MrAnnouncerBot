using System;
using System.Linq;
using Newtonsoft.Json;

namespace DndCore
{
	public class ActiveSpellData
	{
		public string name;
		public int spellSlotLevel;
		public int spellLevel;
		public int playerLevel;
		public string description;
		public string castingTimeStr;
		public string rangeStr;
		public string componentsStr;
		public string durationStr;
		public SchoolOfMagic schoolOfMagic;
		public bool requiresConcentration;
		public bool morePowerfulAtHigherLevels;
		public bool powerComesFromCasterLevel;
		
		public ActiveSpellData()
		{

		}

		public ActiveSpellData(string name, int spellSlotLevel, int spellLevel, int playerLevel, string castingTimeStr, string rangeStr, string componentsStr, string durationStr, SchoolOfMagic schoolOfMagic, bool requiresConcentration, bool morePowerfulAtHigherLevels, bool powerComesFromCasterLevel)
		{
			this.name = name;
			this.spellSlotLevel = spellSlotLevel;
			this.spellLevel = spellLevel;
			this.playerLevel = playerLevel;
			this.description = "";
			this.castingTimeStr = castingTimeStr;
			this.rangeStr = rangeStr;
			this.componentsStr = componentsStr;
			this.durationStr = durationStr;
			this.schoolOfMagic = schoolOfMagic;
			this.requiresConcentration = requiresConcentration;
			this.morePowerfulAtHigherLevels = morePowerfulAtHigherLevels;
			this.powerComesFromCasterLevel = powerComesFromCasterLevel;
		}

		static string GetSpellDescription(CastedSpell castedSpell)
		{
			if (castedSpell.Spell.Description.IndexOf('{') < 0)
				return castedSpell.Spell.Description;

			string result = Expressions.GetStr("$\"" + EmphasizeCalculatedProperties(castedSpell) + "\"", castedSpell.SpellCaster, castedSpell.Target, castedSpell);
			return result;
		}

		private static string EmphasizeCalculatedProperties(CastedSpell castedSpell)
		{
			return castedSpell.Spell.Description
				.Replace("{spell_DieStrRaw}", "«{spell_DieStrRaw}»")
				.Replace("{spell_DieStr}", "«{spell_DieStr}»")
				.Replace("{spell_AmmoCount_word}", "«{spell_AmmoCount_word}»")
				.Replace("{spell_AmmoCount_Word}", "«{spell_AmmoCount_Word}»")
				.Replace("{spell_DoubleAmmoCount}", "«{spell_DoubleAmmoCount}»")
				.Replace("{spell_AmmoCount}", "«{spell_AmmoCount}»")
				;
		}

		public static ActiveSpellData FromCastedSpell(CastedSpell castedSpell)
		{
			Spell spell = castedSpell.Spell;

			bool castingPowerComesFromPlayerLevel = !string.IsNullOrEmpty(spell.BonusThreshold) && spell.BonusThreshold.StartsWith("c");

			ActiveSpellData activeSpellData = new ActiveSpellData(spell.Name,
				castedSpell.SpellSlotLevel, castedSpell.Level, castedSpell.SpellCaster.Level,
				spell.CastingTimeStr, spell.RangeStr, spell.ComponentsStr,
				spell.DurationStr, spell.SchoolOfMagic, spell.RequiresConcentration,
				spell.MorePowerfulWhenCastAtHigherLevels, castingPowerComesFromPlayerLevel);
			activeSpellData.description = GetSpellDescription(castedSpell);
			return activeSpellData;
		}
		
	}
}