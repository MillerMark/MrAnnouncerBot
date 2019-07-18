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
	}
}
