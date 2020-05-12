using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class CharacterDto
	{
		public string name { get; set; }
		public string features { get; set; }
		public string playingNow { get; set; }
		public string playerShortcut { get; set; }
		public string race { get; set; }
		public string heShe { get; set; }
		public string hisHer { get; set; }
		public string himHer { get; set; }
		public string class1 { get; set; }
		public string subclass1 { get; set; }
		public string level1 { get; set; }
		public string class2 { get; set; }
		public string subclass2 { get; set; }
		public string level2 { get; set; }
		public int playerId { get; set; }
		public int hitPoints { get; set; }
		public int maxHitPoints { get; set; }
		public int baseArmorClass { get; set; }
		public decimal goldPieces { get; set; }
		public int baseStrength { get; set; }
		public int baseDexterity { get; set; }
		public int baseConstitution { get; set; }
		public int baseIntelligence { get; set; }
		public int baseWisdom { get; set; }
		public int baseCharisma { get; set; }
		public int proficiencyBonus { get; set; }
		public int walking { get; set; }
		public string proficientSkills { get; set; }
		public string halfProficiency { get; set; }
		public string emoticon { get; set; }
		public string doubleProficiency { get; set; }
		public string savingThrowProficiency { get; set; }
		public string spellCastingAbility { get; set; }
		public int initiative { get; set; }
		public string rollInitiative { get; set; }
		public int hueShift { get; set; }
		public string dieBackColor { get; set; }
		public string dieFontColor { get; set; }
		public int headshotIndex { get; set; }
		public string alignment { get; set; }
		public int leftMostPriority { get; set; }
		public string weaponProficiency { get; set; }
		public string weapons { get; set; }
		public string ammunition { get; set; }
		public string spells { get; set; }
		public CharacterDto()
		{

		}
	}
}
