using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHDM
{

	enum ExhaustionLevels
	{
		level1DisadvantageOnAbilityChecks = 1,
		level2SpeedHalved = 2,
		level3DisadvantageOnAttackRollsAndSavingThrows = 3,
		level4HitPointMaximumHalved = 4,
		level5SpeedReducedToZero = 5,
		level6Death = 6
	}

	/*
	Exhaustion
	Some Special Abilities and environmental hazards, such as starvation and the long-­term Effects of freezing or scorching temperatures, can lead to a Special condition called exhaustion. Exhaustion is measured in six levels. An effect can give a creature one or more levels of exhaustion, as specified in the effect’s description.

	Exhaustion Effects
	Level	Effect
	1	Disadvantage on Ability Checks
	2	Speed halved
	3	Disadvantage on Attack rolls and Saving Throws
	4	Hit point maximum halved
	5	Speed reduced to 0
	6	Death */

	enum Skills
	{
		acrobatics = 1,
		animalHandling = 2,
		arcana = 4,
		athletics = 8,
		deception = 16,
		history = 32,
		insight = 64,
		intimidation = 128,
		investigation = 256,
		medicine = 512,
		nature = 1024,
		perception = 2048,
		performance = 4096,
		persuasion = 8192,
		religion = 16384,
		slightOfHand = 32768,
		stealth = 65536,
		survival = 131072
	}

	class Character
	{
		public List<Item> equipment = new List<Item>();
		public List<CurseBlessingDisease> cursesAndBlessings = new List<CurseBlessingDisease>();
		public string name;
		public int level;
		public Conditions conditions = Conditions.none;
		public int onTurnActions = 1;
		public int offTurnActions = 0;
		public double inspiration = 0;
		public int experiencePoints = 0;
		string raceClass;
		string alignment;

		double armorClass;
		double initiative;
		double speed;
		double hitPoints;
		double tempHitPoints;
		double maxHitPoints;
		double goldPieces;
		double load;
		double weight;
		double proficiencyBonus;
		int savingThrowProficiency;
		string remainingHitDice;
		string totalHitDice;
		bool deathSaveLife1;
		bool deathSaveLife2;
		bool deathSaveLife3;
		bool deathSaveDeath1;
		bool deathSaveDeath2;
		bool deathSaveDeath3;
		int proficientSkills;

		double tempSavingThrowModStrength = 0;
		double tempSavingThrowModDexterity = 0;
		double tempSavingThrowModConstitution = 0;
		double tempSavingThrowModIntelligence = 0;
		double tempSavingThrowModWisdom = 0;
		double tempSavingThrowModCharisma = 0;

		double tempAcrobaticsMod = 0;
		double tempAnimalHandlingMod = 0;
		double tempArcanaMod = 0;
		double tempAthleticsMod = 0;
		double tempDeceptionMod = 0;
		double tempHistoryMod = 0;
		double tempInsightMod = 0;
		double tempIntimidationMod = 0;
		double tempInvestigationMod = 0;
		double tempMedicineMod = 0;
		double tempNatureMod = 0;
		double tempPerceptionMod = 0;
		double tempPerformanceMod = 0;
		double tempPersuasionMod = 0;
		double tempReligionMod = 0;
		double tempSlightOfHandMod = 0;
		double tempStealthMod = 0;
		double tempSurvivalMod = 0;

		/* 
			savingThrowModStrength
			savingThrowModDexterity
			savingThrowModConstitution
			savingThrowModIntelligence
			savingThrowModWisdom
			savingThrowModCharisma
		*/

		bool hasProficiencyBonusForSkill(Skills skill)
		{
			return (proficientSkills & (int)skill) == (int)skill;
		}

		bool hasSkillProficiencyAcrobatics
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.acrobatics);
			}
		}

		bool hasSkillProficiencyAnimalHandling
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.animalHandling);
			}
		}

		bool hasSkillProficiencyArcana
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.arcana);
			}
		}

		bool hasSkillProficiencyAthletics
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.athletics);
			}
		}

		bool hasSkillProficiencyDeception
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.deception);
			}
		}


		bool hasSkillProficiencyHistory
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.history);
			}
		}


		bool hasSkillProficiencyInsight
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.insight);
			}
		}

		bool hasSkillProficiencyIntimidation
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.intimidation);
			}
		}

		bool hasSkillProficiencyInvestigation
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.investigation);
			}
		}

		bool hasSkillProficiencyMedicine
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.medicine);
			}
		}

		bool hasSkillProficiencyNature
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.nature);
			}
		}

		bool hasSkillProficiencyPerception
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.perception);
			}
		}

		bool hasSkillProficiencyPerformance
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.performance);
			}
		}

		bool hasSkillProficiencyPersuasion
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.persuasion);
			}
		}

		bool hasSkillProficiencyReligion
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.religion);
			}
		}

		bool hasSkillProficiencySlightOfHand
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.slightOfHand);
			}
		}

		bool hasSkillProficiencyStealth
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.stealth);
			}
		}

		bool hasSkillProficiencySurvival
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.survival);
			}
		}

		double skillModAcrobatics
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.acrobatics) + this.dexterityMod + this.tempAcrobaticsMod;
			}
		}

		double skillModAnimalHandling
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.animalHandling) + this.wisdomMod + this.tempAnimalHandlingMod;
			}
		}

		double skillModArcana
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.arcana) + this.intelligenceMod + this.tempArcanaMod;
			}
		}

		double skillModAthletics
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.athletics) + this.strengthMod + this.tempAthleticsMod;

			}
		}

		double skillModDeception
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.deception) + this.charismaMod + this.tempDeceptionMod;

			}
		}

		double skillModHistory
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.history) + this.intelligenceMod + this.tempHistoryMod;

			}
		}

		double skillModInsight
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.insight) + this.wisdomMod + this.tempInsightMod;
			}
		}

		double skillModIntimidation
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.intimidation) + this.charismaMod + this.tempIntimidationMod;

			}
		}

		double skillModInvestigation
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.investigation) + this.intelligenceMod + this.tempInvestigationMod;
			}
		}

		double skillModMedicine
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.medicine) + this.wisdomMod + this.tempMedicineMod;
			}
		}


		double skillModNature
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.nature) + this.intelligenceMod + this.tempNatureMod;
			}
		}


		double skillModPerception
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.perception) + this.wisdomMod + this.tempPerceptionMod;
			}
		}


		double skillModPerformance
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.performance) + this.charismaMod + this.tempPerformanceMod;
			}
		}


		double skillModPersuasion
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.persuasion) + this.charismaMod + this.tempPersuasionMod;
			}
		}


		double skillModReligion
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.religion) + this.intelligenceMod + this.tempReligionMod;
			}
		}


		double skillModSlightOfHand
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.slightOfHand) + this.dexterityMod + this.tempSlightOfHandMod;
			}
		}

		double skillModStealth
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.stealth) + this.dexterityMod + this.tempStealthMod;
			}
		}

		double skillModSurvival
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.survival) + this.wisdomMod + this.tempSurvivalMod;
			}
		}

		double savingThrowModStrength
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.strength) + this.strengthMod + this.tempSavingThrowModStrength;
			}
		}

		double savingThrowModDexterity
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.dexterity) + this.dexterityMod + this.tempSavingThrowModDexterity;
			}
		}

		double savingThrowModConstitution
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.constitution) + this.constitutionMod + this.tempSavingThrowModConstitution;
			}
		}

		double savingThrowModWisdom
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.wisdom) + this.wisdomMod + this.tempSavingThrowModWisdom;
			}
		}

		double savingThrowModCharisma
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.charisma) + this.charismaMod + this.tempSavingThrowModCharisma;
			}
		}

		double savingThrowModIntelligence
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.intelligence) + this.intelligenceMod + this.tempSavingThrowModIntelligence;
			}
		}

		bool hasSavingThrowProficiency(Ability ability)
		{
			return (savingThrowProficiency & (int)ability) == (int)ability;
		}

		bool hasSavingThrowProficiencyIntelligence
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.intelligence);
			}
		}
		bool hasSavingThrowProficiencyStrength
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.strength); ;
			}
		}
		bool hasSavingThrowProficiencyDexterity
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.dexterity);
			}

		}

		bool hasSavingThrowProficiencyConstitution
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.constitution);
			}
		}
		bool hasSavingThrowProficiencyWisdom
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.wisdom);
			}
		}
		bool hasSavingThrowProficiencyCharisma
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.charisma);
			}
		}

		double getProficiencyBonusForSkill(Skills skill)
		{
			if (hasProficiencyBonusForSkill(skill))
				return this.proficiencyBonus;
			return 0;
		}

		double getProficiencyBonusForSavingThrow(Ability savingThrow)
		{
			if (this.hasSavingThrowProficiency(savingThrow))
				return this.proficiencyBonus;
			return 0;
		}

		private double _passivePerception = int.MinValue;

		double passivePerception
		{
			get
			{
				if (this._passivePerception == int.MinValue)
					this._passivePerception = 10 + this._wisdomMod + this.getProficiencyBonusForSkill(Skills.perception);
				return this._passivePerception;
			}
		}

		private double _wisdom;
		private double _wisdomMod;

		double wisdom
		{
			get
			{
				return this._wisdom;
			}
			set
			{
				this._wisdom = value;
				this._wisdomMod = this.getModFromAbility(this._wisdom);
			}
		}


		private double _charisma;
		private double _charismaMod;

		double charisma
		{
			get
			{
				return this._charisma;
			}
			set
			{
				this._charisma = value;
				this._charismaMod = this.getModFromAbility(this._charisma);
			}
		}

		private double _intelligence;
		private double _intelligenceMod;

		double intelligence
		{
			get
			{
				return this._intelligence;
			}
			set
			{
				this._intelligence = value;
				this._intelligenceMod = this.getModFromAbility(this._intelligence);
			}
		}

		private double _strength;
		private double strengthMod;

		double strength
		{
			get
			{
				return this._strength;
			}
			set
			{
				this._strength = value;
				this.strengthMod = this.getModFromAbility(this._strength);
			}
		}

		private double _dexterity;
		private double dexterityMod;

		double dexterity
		{
			get
			{
				return this._dexterity;
			}
			set
			{
				this._dexterity = value;
				this.dexterityMod = this.getModFromAbility(this._dexterity);
			}
		}


		private double _constitution;
		private double constitutionMod;

		double constitution
		{
			get
			{
				return this._constitution;
			}
			set
			{
				this._constitution = value;
				this.constitutionMod = this.getModFromAbility(this._constitution);
			}
		}

		int getModFromAbility(double abilityScore)
		{
			return Math.floor((abilityScore - 10) / 2);
		}

		static int rollDie(int sides)
		{
			return Random.intBetween(1, sides);
		}

		static int getAbilityScore()
		{
			return Character.rollDie(6) + Character.rollDie(6) + Character.rollDie(6);
		}

		void pack(Item item)
		{
			this.equipment.Add(item);
		}

		void unpack(Item item, int count = 1)
		{
			int index = this.equipment.IndexOf(item);
			if (index >= 0)
			{
				Item thisItem = this.equipment[index];
				if (thisItem.count > 0)
					if (count == int.MaxValue)
					{
						thisItem.count = 0;
					}
					else
					{
						thisItem.count -= count;
					}

				if (thisItem.count <= 0)
				{
					this.equipment.RemoveAt(index);
				}
			}
		}

		void equip(Item item)
		{
			this.equipment.Add(item);
			item.equipped = true;
		}

		// TS:

		//static Character newTestElf()
		//{
		//	let elf: Character = new Character();
		//	elf.name = 'Taragon';
		//	elf.raceClass = 'Wood Elf Barbarian';
		//	elf.alignment = 'Chaotic Good';
		//	elf.armorClass = 12;
		//	Character.generateRandomAttributes(elf);
		//	elf.remainingHitDice = '1 d10';
		//	elf.level = 1;
		//	elf.inspiration = 0;

		//	elf.initiative = 2;
		//	elf.speed = 30;
		//	elf.hitPoints = 47;
		//	elf.tempHitPoints = 0;
		//	elf.maxHitPoints = 55;
		//	elf.proficiencyBonus = 2;
		//	elf.savingThrowProficiency = Ability.intelligence + Ability.charisma;
		//	elf.proficientSkills = Skills.acrobatics + Skills.deception + Skills.slightOfHand;
		//	elf.deathSaveLife1 = true;
		//	//elf.deathSaveLife2 = true;
		//	//elf.deathSaveLife3 = true;
		//	elf.deathSaveDeath1 = true;
		//	elf.deathSaveDeath2 = true;
		//	//elf.deathSaveDeath3 = true;

		//	return elf;
		//}
		//private static generateRandomAttributes(character: Character)
		//{
		//	character.charisma = Character.getAbilityScore();
		//	character.constitution = Character.getAbilityScore();
		//	character.dexterity = Character.getAbilityScore();
		//	character.wisdom = Character.getAbilityScore();
		//	character.intelligence = Character.getAbilityScore();
		//	character.strength = Character.getAbilityScore();
		//	character.experiencePoints = Random.intMaxDigitCount(6);
		//	character.goldPieces = Random.intMaxDigitCount(6);
		//	character.weight = Random.intBetween(80, 275);
		//	character.load = Random.intBetween(15, 88);
		//}

		//static newTestBarbarian() : Character {
		//    let barbarian: Character = new Character();
		//barbarian.name = 'Ava';
		//    barbarian.raceClass = 'Dragonborn Barbarian';
		//    barbarian.alignment = 'Chaotic Evil';
		//    barbarian.armorClass = 14;
		//    Character.generateRandomAttributes(barbarian);
		//    barbarian.remainingHitDice = '1 d10';
		//    barbarian.level = 1;
		//    barbarian.inspiration = 0;

		//    barbarian.initiative = 2;
		//    barbarian.speed = 30;
		//    barbarian.hitPoints = 127;
		//    barbarian.tempHitPoints = 3;
		//    barbarian.maxHitPoints = 127;
		//    barbarian.proficiencyBonus = 2;
		//    barbarian.savingThrowProficiency = Ability.strength + Ability.dexterity;
		//    barbarian.proficientSkills = Skills.acrobatics + Skills.intimidation + Skills.athletics;
		//    barbarian.deathSaveLife1 = true;
		//    barbarian.deathSaveLife2 = true;
		//    //elf.deathSaveLife3 = true;
		//    barbarian.deathSaveDeath1 = true;
		//    barbarian.deathSaveDeath2 = true;
		//    //elf.deathSaveDeath3 = true;

		//    return barbarian;
		//  }

		//  static newTestDruid() : Character {
		//    let druid: Character = new Character();
		//druid.name = 'Kylee';
		//    druid.raceClass = 'Wood Elf Druid';
		//    druid.alignment = 'Lawful Good';
		//    druid.armorClass = 10;
		//    Character.generateRandomAttributes(druid);
		//    druid.remainingHitDice = '1 d8';
		//    druid.level = 1;
		//    druid.inspiration = 0;

		//    druid.initiative = 2;
		//    druid.speed = 30;
		//    druid.hitPoints = 27;
		//    druid.tempHitPoints = 0;
		//    druid.maxHitPoints = 44;
		//    druid.proficiencyBonus = 2;
		//    druid.savingThrowProficiency = Ability.wisdom + Ability.dexterity;
		//    druid.proficientSkills = Skills.animalHandling + Skills.nature + Skills.medicine;
		//    //barbarian.deathSaveLife1 = true;
		//    //barbarian.deathSaveLife2 = true;
		//    //elf.deathSaveLife3 = true;
		//    //barbarian.deathSaveDeath1 = true;
		//    //barbarian.deathSaveDeath2 = true;
		//    //elf.deathSaveDeath3 = true;

		//    return druid;
		//  }

		//  static newTestWizard() : Character {
		//    let wizard: Character = new Character();
		//wizard.name = 'Morkin';
		//    wizard.raceClass = 'Human Wizard';
		//    wizard.alignment = 'Chaotic Neutral';
		//    wizard.armorClass = 10;
		//    Character.generateRandomAttributes(wizard);
		//    wizard.remainingHitDice = '1 d8';
		//    wizard.level = 1;
		//    wizard.inspiration = 0;

		//    wizard.initiative = 2;
		//    wizard.speed = 30;
		//    wizard.hitPoints = 33;
		//    wizard.tempHitPoints = 0;
		//    wizard.maxHitPoints = 127;
		//    wizard.proficiencyBonus = 2;
		//    wizard.savingThrowProficiency = Ability.intelligence + Ability.charisma;
		//    wizard.proficientSkills = Skills.arcana + Skills.slightOfHand + Skills.deception;
		//    wizard.equip(Weapon.buildShortSword());
		//    wizard.pack(Weapon.buildBlowgun());
		//    wizard.pack(Ammunition.buildBlowgunNeedlePack());

		//    return wizard;
		//  }
	}