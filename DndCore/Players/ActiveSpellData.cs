using System;
using System.Linq;

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

		public ActiveSpellData()
		{

		}

		public ActiveSpellData(string name, int spellSlotLevel, int spellLevel, int playerLevel, string castingTimeStr, string rangeStr, string componentsStr, string durationStr, SchoolOfMagic schoolOfMagic, bool requiresConcentration)
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
		}

		static string GetSpellDescription(CastedSpell castedSpell)
		{
			if (castedSpell.Spell.Description.IndexOf('{') < 0)
				return castedSpell.Spell.Description;

			string result = Expressions.GetStr("$\"" + EmphasizeCalculatedProperties(castedSpell) + "\"", castedSpell.SpellCaster, castedSpell.TargetCreature, castedSpell);
			return result;
		}

		private static string EmphasizeCalculatedProperties(CastedSpell castedSpell)
		{
			return castedSpell.Spell.Description
				.Replace("{spell_DieStrRaw}", "«{spell_DieStrRaw}»")
				.Replace("{spell_DieStr}", "«{spell_DieStr}»")
				.Replace("{spell_AmmoCount_word}", "«{spell_AmmoCount_word}»")
				.Replace("{spell_AmmoCount_Word}", "«{spell_AmmoCount_Word}»")
				.Replace("{spell_AmmoCount}", "«{spell_AmmoCount}»")
				;
		}

		public static ActiveSpellData FromCastedSpell(CastedSpell castedSpell)
		{
			ActiveSpellData activeSpellData = new ActiveSpellData(castedSpell.Spell.Name,
				castedSpell.SpellSlotLevel, castedSpell.Level, castedSpell.SpellCaster.level,
				castedSpell.Spell.CastingTimeStr, castedSpell.Spell.RangeStr, castedSpell.Spell.ComponentsStr,
				castedSpell.Spell.DurationStr, castedSpell.Spell.SchoolOfMagic, castedSpell.Spell.RequiresConcentration);
			activeSpellData.description = GetSpellDescription(castedSpell);
			return activeSpellData;
		}
		
	}
}