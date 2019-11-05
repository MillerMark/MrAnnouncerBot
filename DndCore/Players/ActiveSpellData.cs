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

		public ActiveSpellData(string name, int spellSlotLevel, int spellLevel, int playerLevel, string description, string castingTimeStr, string rangeStr, string componentsStr, string durationStr, SchoolOfMagic schoolOfMagic, bool requiresConcentration)
		{
			this.name = name;
			this.spellSlotLevel = spellSlotLevel;
			this.spellLevel = spellLevel;
			this.playerLevel = playerLevel;
			this.description = description;
			this.castingTimeStr = castingTimeStr;
			this.rangeStr = rangeStr;
			this.componentsStr = componentsStr;
			this.durationStr = durationStr;
			this.schoolOfMagic = schoolOfMagic;
			this.requiresConcentration = requiresConcentration;
		}

		public static ActiveSpellData FromCastedSpell(CastedSpell castedSpell)
		{
			return new ActiveSpellData(castedSpell.Spell.Name, 
				castedSpell.SpellSlotLevel, castedSpell.Level, castedSpell.SpellCaster.level, 
				castedSpell.Spell.Description, 
				castedSpell.Spell.CastingTimeStr, castedSpell.Spell.RangeStr, castedSpell.Spell.ComponentsStr, 
				castedSpell.Spell.DurationStr, castedSpell.Spell.SchoolOfMagic, castedSpell.Spell.RequiresConcentration);
		}
	}
}