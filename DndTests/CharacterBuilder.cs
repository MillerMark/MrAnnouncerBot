using DndCore;
using System;

namespace DndTests
{
	public static class CharacterBuilder
	{
		public static Character BuildTestElf(string name = "")
		{
			Character elf = new Character();
			elf.kind = CreatureKinds.Humanoid;
			
			if (string.IsNullOrEmpty(name))
				elf.name = "Taragon";
			else
				elf.name = name;

			elf.race = "Wood Elf Barbarian";
			elf.AddClass("Barbarian", 1);
			elf.alignmentStr = "Chaotic Good";
			elf.baseArmorClass = 12;
			GenerateRandomAttributes(elf);
			elf.remainingHitDice = "1 d10";
			elf.inspiration = "";

			elf.initiative = 2;
			elf.baseWalkingSpeed = 30;
			elf.HitPoints = 47;
			elf.tempHitPoints = 0;
			elf.maxHitPoints = 55;
			elf.proficiencyBonus = 2;
			elf.savingThrowProficiency = Ability.intelligence | Ability.charisma;
			elf.proficientSkills = Skills.acrobatics | Skills.deception | Skills.sleightOfHand;
			elf.deathSaveLife1 = true;
			//elf.deathSaveLife2 = true;
			//elf.deathSaveLife3 = true;
			elf.deathSaveDeath1 = true;
			elf.deathSaveDeath2 = true;
			//elf.deathSaveDeath3 = true;

			return elf;
		}
		public static void GenerateRandomAttributes(Character character)
		{
			character.baseCharisma = Die.getAbilityScore();
			character.baseConstitution = Die.getAbilityScore();
			character.baseDexterity = Die.getAbilityScore();
			character.baseWisdom = Die.getAbilityScore();
			character.baseIntelligence = Die.getAbilityScore();
			character.baseStrength = Die.getAbilityScore();
			character.experiencePoints = 1234;
			character.goldPieces = 4321;
			character.weight = 144;
			character.load = 166;
		}

		public static Character BuildTestBarbarian(string name = "")
		{
			Character barbarian = new Character();
			barbarian.kind = CreatureKinds.Humanoid;

			if (string.IsNullOrEmpty(name))
				barbarian.name = "Ava";
			else
				barbarian.name = name;
			barbarian.race = "Dragonborn";
			barbarian.AddClass("Barbarian", 1);
			barbarian.alignmentStr = "Chaotic Evil";
			barbarian.baseArmorClass = 14;
			GenerateRandomAttributes(barbarian);
			barbarian.remainingHitDice = "1 d10";
			barbarian.inspiration = "";

			barbarian.initiative = 2;
			barbarian.baseWalkingSpeed = 30;
			barbarian.HitPoints = 127;
			barbarian.tempHitPoints = 3;
			barbarian.maxHitPoints = 127;
			barbarian.proficiencyBonus = 2;
			barbarian.savingThrowProficiency = Ability.strength | Ability.dexterity;
			barbarian.proficientSkills = Skills.acrobatics | Skills.intimidation | Skills.athletics;
			barbarian.deathSaveLife1 = true;
			barbarian.deathSaveLife2 = true;
			//elf.deathSaveLife3 = true;
			barbarian.deathSaveDeath1 = true;
			barbarian.deathSaveDeath2 = true;
			//elf.deathSaveDeath3 = true;

			return barbarian;
		}

		public static Character BuildTestWizard(string name = "")
		{
			Character wizard = new Character();
			wizard.kind = CreatureKinds.Humanoid;
			if (string.IsNullOrEmpty(name))
				wizard.name = "Morkin";
			else
				wizard.name = name;
			
			wizard.race = "Human";
			wizard.AddClass("Wizard", 1);
			wizard.alignmentStr = "Chaotic Neutral";
			wizard.baseArmorClass = 10;
			GenerateRandomAttributes(wizard);
			wizard.remainingHitDice = "1 d8";
			wizard.inspiration = "";

			wizard.initiative = 2;
			wizard.baseWalkingSpeed = 30;
			wizard.HitPoints = 33;
			wizard.tempHitPoints = 0;
			wizard.maxHitPoints = 127;
			wizard.proficiencyBonus = 2;
			wizard.savingThrowProficiency = Ability.intelligence | Ability.charisma;
			wizard.proficientSkills = Skills.arcana | Skills.sleightOfHand | Skills.deception;
			wizard.Equip(Weapon.buildShortSword());
			wizard.Pack(Weapon.buildBlowgun());
			wizard.Pack(Ammunition.buildBlowgunNeedlePack());

			return wizard;
		}
		public static Character BuildTestDruid(string name = "")
		{
			Character druid = new Character();
			druid.kind = CreatureKinds.Humanoid;
			if (string.IsNullOrEmpty(name))
				druid.name = "Kylee";
			else
				druid.name = name;
			
			druid.race = "Wood Elf";
			druid.AddClass("Druid", 1);
			druid.alignmentStr = "Lawful Good";
			druid.baseArmorClass = 10;
			GenerateRandomAttributes(druid);
			druid.remainingHitDice = "1 d8";
			druid.inspiration = "";

			druid.initiative = 2;
			druid.baseWalkingSpeed = 30;
			druid.HitPoints = 27;
			druid.tempHitPoints = 0;
			druid.maxHitPoints = 44;
			druid.proficiencyBonus = 2;
			druid.savingThrowProficiency = Ability.wisdom | Ability.dexterity;
			druid.proficientSkills = Skills.animalHandling | Skills.nature | Skills.medicine;

			return druid;
		}
	}
}
