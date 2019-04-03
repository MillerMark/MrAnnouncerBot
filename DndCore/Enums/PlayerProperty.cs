using System;
using System.ComponentModel;
using System.Linq;

namespace DndCore
{
	[TypeConverter("DndCore.EnumDescriptionTypeConverter")]
	public enum PlayerProperty
	{
		Alignment,
		ArmorClass,
		Charisma,
		Constitution,
		DeathSaves,
		Dexterity,
		ExperiencePoints,
		GoldPieces,
		HitDice,
		HitPoints,
		TempHitPoints,
		Initiative,
		Inspiration,
		Intelligence,
		Level,
		Load,
		[Description("Name/Headshot")]
		NameHeadshot,
		Perception,
		ProficiencyBonus,
		RaceClass,
		SavingCharisma,
		SavingConstitution,
		SavingDexterity,
		SavingIntelligence,
		SavingStrength,
		SavingWisdom,
		SkillsAcrobatics,
		SkillsAnimalHandling,
		SkillsArcana,
		SkillsAthletics,
		SkillsDeception,
		SkillsHistory,
		SkillsInsight,
		SkillsIntimidation,
		SkillsInvestigation,
		SkillsMedicine,
		SkillsNature,
		SkillsPerception,
		SkillsPerformance,
		SkillsPersuasion,
		SkillsReligion,
		SkillsSlightOfHand,
		SkillsStealth,
		SkillsSurvival,
		Speed,
		Strength,
		Weight,
		Wisdom
	}
}
