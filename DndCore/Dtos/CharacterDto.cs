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
		static string GetFeatureList(List<AssignedFeature> features)
		{
			return string.Join(";", features.Select(x => x.Feature.Name).ToList());
			//List<string> features = featureListStr.Split(';').ToList();
			//foreach (string feature in features)
			//{
			//	string trimmedFeature = feature.Trim();
			//	if (!string.IsNullOrEmpty(trimmedFeature))
			//		results.Add(AssignedFeature.From(trimmedFeature, player));
			//}
			//return results;
		}
		//public static CharacterDto From(Character dto)
		//{
		//	CharacterDto result = new CharacterDto();
		//	result.name = dto.name;
		//	result.features = GetFeatureList(dto.features);
		//	result.playingNow = dto.playingNow;
		//	result.playerShortcut = dto.playerShortcut;
		//	result.race = dto.race;
		//	result.heShe = dto.heShe;
		//	result.hisHer = dto.hisHer;
		//	result.himHer = dto.himHer;
		//	result.class1 = dto.class1;
		//	result.subclass1 = dto.subclass1;
		//	result.level1 = dto.level1;
		//	result.class2 = dto.class2;
		//	result.subclass2 = dto.subclass2;
		//	result.level2 = dto.level2;
		//	result.playerId = dto.playerId;
		//	result.hitPoints = dto.hitPoints;
		//	result.maxHitPoints = dto.maxHitPoints;
		//	result.baseArmorClass = dto.baseArmorClass;
		//	result.goldPieces = dto.goldPieces;
		//	result.baseStrength = dto.baseStrength;
		//	result.baseDexterity = dto.baseDexterity;
		//	result.baseConstitution = dto.baseConstitution;
		//	result.baseIntelligence = dto.baseIntelligence;
		//	result.baseWisdom = dto.baseWisdom;
		//	result.baseCharisma = dto.baseCharisma;
		//	result.proficiencyBonus = dto.proficiencyBonus;
		//	result.walking = dto.walking;
		//	result.proficientSkills = dto.proficientSkills;
		//	result.halfProficiency = dto.halfProficiency;
		//	result.emoticon = dto.emoticon;
		//	result.doubleProficiency = dto.doubleProficiency;
		//	result.savingThrowProficiency = dto.savingThrowProficiency;
		//	result.spellCastingAbility = dto.spellCastingAbility;
		//	result.initiative = dto.initiative;
		//	result.rollInitiative = dto.rollInitiative;
		//	result.hueShift = dto.hueShift;
		//	result.dieBackColor = dto.dieBackColor;
		//	result.dieFontColor = dto.dieFontColor;
		//	result.headshotIndex = dto.headshotIndex;
		//	result.alignment = dto.alignment;
		//	result.leftMostPriority = dto.leftMostPriority;
		//	result.weaponProficiency = dto.weaponProficiency;
		//	result.weapons = dto.weapons;
		//	result.ammunition = dto.ammunition;
		//	result.spells = dto.spells;
		//	
		//}
	}
}
