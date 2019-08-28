using System;
using System.Linq;

namespace DndCore
{
	public class CharacterDto
	{
		public string name { get; set; }
		public string playingNow { get; set; }
		public string raceClass { get; set; }
		public int level { get; set; }
		public int hitPoints { get; set; }
		public int maxHitPoints { get; set; }
		public int baseArmorClass { get; set; }
		public int goldPieces { get; set; }
		public int baseStrength { get; set; }
		public int baseDexterity { get; set; }
		public int baseConstitution { get; set; }
		public int baseIntelligence { get; set; }
		public int baseWisdom { get; set; }
		public int baseCharisma { get; set; }
		public int proficiencyBonus { get; set; }
		public int walking { get; set; }
		public string proficientSkills { get; set; }
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
		public CharacterDto()
		{

		}
	}
}
