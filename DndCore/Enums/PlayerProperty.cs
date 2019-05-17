using System;
using System.Linq;
using System.ComponentModel;

namespace DndCore.Enums
{
	[TypeConverter("DndCore.EnumDescriptionTypeConverter")]
	public enum PlayerProperty
	{
		None,
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
