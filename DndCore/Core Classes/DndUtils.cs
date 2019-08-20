using System;
using System.Linq;

namespace DndCore
{
	public static class DndUtils
	{
		public static Skills ToSkill(string skillStr)
		{
			switch (skillStr.ToLower())
			{
				case "acrobatics": return Skills.acrobatics;
				case "animal handling":
				case "animalhandling": return Skills.animalHandling;
				case "arcana": return Skills.arcana;
				case "athletics": return Skills.athletics;
				case "deception": return Skills.deception;
				case "history": return Skills.history;
				case "insight": return Skills.insight;
				case "intimidation": return Skills.intimidation;
				case "investigation": return Skills.investigation;
				case "medicine": return Skills.medicine;
				case "nature": return Skills.nature;
				case "perception": return Skills.perception;
				case "performance": return Skills.performance;
				case "persuasion": return Skills.persuasion;
				case "religion": return Skills.religion;
				case "slightofhand":
				case "slight of hand"
				: return Skills.slightOfHand;
				case "stealth": return Skills.stealth;
				case "survival": return Skills.survival;
				case "strength": return Skills.strength;
				case "dexterity": return Skills.dexterity;
				case "constitution": return Skills.constitution;
				case "intelligence": return Skills.intelligence;
				case "wisdom": return Skills.wisdom;
				case "charisma": return Skills.charisma;
			}
			return Skills.none;
		}

		public static Ability ToAbility(string abilityStr)
		{
			switch (abilityStr.ToLower())
			{
				case "charisma": return Ability.Charisma;
				case "constitution": return Ability.Constitution;
				case "dexterity": return Ability.Dexterity;
				case "intelligence": return Ability.Intelligence;
				case "strength": return Ability.Strength;
				case "wisdom": return Ability.Wisdom;
			}
			return Ability.None;
		}
		public static DamageType ToDamage(string damageStr)
		{
			switch (damageStr.ToLower())
			{
				case "acid": return DamageType.Acid;
				case "bludgeoning": return DamageType.Bludgeoning;
				case "cold": return DamageType.Cold;
				case "fire": return DamageType.Fire;
				case "force": return DamageType.Force;
				case "lightning": return DamageType.Lightning;
				case "necrotic": return DamageType.Necrotic;
				case "piercing": return DamageType.Piercing;
				case "poison": return DamageType.Poison;
				case "psychic": return DamageType.Psychic;
				case "radiant": return DamageType.Radiant;
				case "slashing": return DamageType.Slashing;
				case "thunder": return DamageType.Thunder;
				case "condition": return DamageType.Condition;
				case "none": return DamageType.None;
			}
			return DamageType.None;
		}
		
		public static string ToSkillDisplayString(Skills skill)
		{
			switch (skill)
			{
				case Skills.none:
					return "none";
				case Skills.acrobatics:
					return "Acrobatics";
				case Skills.animalHandling:
					return "Animal Handling";
				case Skills.arcana:
					return "Arcana";
				case Skills.athletics:
					return "Athletics";
				case Skills.deception:
					return "Deception";
				case Skills.history:
					return "History";
				case Skills.insight:
					return "Insight";
				case Skills.intimidation:
					return "Intimidation";
				case Skills.investigation:
					return "Investigation";
				case Skills.medicine:
					return "Medicine";
				case Skills.nature:
					return "Nature";
				case Skills.perception:
					return "Perception";
				case Skills.performance:
					return "Performance";
				case Skills.persuasion:
					return "Persuasion";
				case Skills.religion:
					return "Religion";
				case Skills.slightOfHand:
					return "Slight of Hand";
				case Skills.stealth:
					return "Stealth";
				case Skills.survival:
					return "Survival";
				case Skills.strength:
					return "Strength";
				case Skills.dexterity:
					return "Dexterity";
				case Skills.constitution:
					return "Constitution";
				case Skills.intelligence:
					return "Intelligence";
				case Skills.wisdom:
					return "Wisdom";
				case Skills.charisma:
					return "Charisma";
			}
			return "?";
		}
		public static string ToArticlePlusSkillDisplayString(Skills skill)
		{
			switch (skill)
			{
				case Skills.acrobatics:
				case Skills.animalHandling:
				case Skills.arcana:
				case Skills.athletics:
				case Skills.insight:
				case Skills.intimidation:
				case Skills.investigation:
				case Skills.intelligence:
					return "an " + ToSkillDisplayString(skill);
			}
			return "a " + ToSkillDisplayString(skill);
		}

		static string ToAbilityDisplayString(Ability ability)
		{
			switch (ability)
			{
				case Ability.None:
					return "None";
				case Ability.Strength:
					return "Strength";
				case Ability.Dexterity:
					return "Dexterity";
				case Ability.Constitution:
					return "Constitution";
				case Ability.Intelligence:
					return "Intelligence";
				case Ability.Wisdom:
					return "Wisdom";
				case Ability.Charisma:
					return "Charisma";
			}
			return "?";
		}
		public static string ToArticlePlusAbilityDisplayString(Ability ability)
		{
			if (ability == Ability.Intelligence)
				return "an " + ToAbilityDisplayString(ability);
			return "a " + ToAbilityDisplayString(ability);
		}
	}
}
