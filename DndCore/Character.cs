using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DndCore
{
	public class Character : Creature
	{
		public int playerID { get; set; }
		double _passivePerception = int.MinValue;
		public bool deathSaveDeath1 = false;
		public bool deathSaveDeath2 = false;
		public bool deathSaveDeath3 = false;
		public bool deathSaveLife1 = false;
		public bool deathSaveLife2 = false;
		public bool deathSaveLife3 = false;
		public int experiencePoints = 0;
		public string inspiration = "";
		public int level;
		public int headshotIndex;
		public int hueShift = 0;
		public string dieBackColor = "#ffffff";
		public string dieFontColor = "#000000";
		public double load = 0;
		public double proficiencyBonus = 0;
		public Skills proficientSkills = 0;
		public Skills doubleProficiency = 0;
		public string remainingHitDice = string.Empty;
		public Ability savingThrowProficiency = 0;
		public Ability spellCastingAbility = Ability.None;
		public double tempAcrobaticsMod = 0;
		public double tempAnimalHandlingMod = 0;
		public double tempArcanaMod = 0;
		public double tempAthleticsMod = 0;
		public double tempDeceptionMod = 0;
		public double tempHistoryMod = 0;
		public double tempInsightMod = 0;
		public double tempIntimidationMod = 0;
		public double tempInvestigationMod = 0;
		public double tempMedicineMod = 0;
		public double tempNatureMod = 0;
		public double tempPerceptionMod = 0;
		public double tempPerformanceMod = 0;
		public double tempPersuasionMod = 0;
		public double tempReligionMod = 0;
		public double tempSavingThrowModCharisma = 0;
		public double tempSavingThrowModConstitution = 0;
		public double tempSavingThrowModDexterity = 0;
		public double tempSavingThrowModIntelligence = 0;
		public double tempSavingThrowModStrength = 0;
		public double tempSavingThrowModWisdom = 0;
		public double tempSlightOfHandMod = 0;
		public double tempStealthMod = 0;
		public double tempSurvivalMod = 0;
		public string totalHitDice = string.Empty;
		public double weight = 0;
		public VantageKind rollInitiative = VantageKind.Normal;

		public double charismaMod
		{
			get
			{
				return this.getModFromAbility(this.Charisma);
			}
		}
		public double constitutionMod
		{
			get
			{
				return this.getModFromAbility(this.Constitution);
			}
		}
		public double dexterityMod
		{
			get
			{
				return this.getModFromAbility(this.Dexterity);
			}
		}
		bool hasSavingThrowProficiencyCharisma
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.Charisma);
			}
		}

		bool hasSavingThrowProficiencyConstitution
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.Constitution);
			}
		}
		bool hasSavingThrowProficiencyDexterity
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.Dexterity);
			}

		}

		bool hasSavingThrowProficiencyIntelligence
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.Intelligence);
			}
		}
		bool hasSavingThrowProficiencyStrength
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.Strength); ;
			}
		}
		bool hasSavingThrowProficiencyWisdom
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.Wisdom);
			}
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
		public double intelligenceMod
		{
			get
			{
				return this.getModFromAbility(this.Intelligence);
			}
		}

		public double PassivePerception
		{
			get
			{
				if (this._passivePerception == int.MinValue)
					this._passivePerception = 10 + this.wisdomMod + this.getProficiencyBonusForSkill(Skills.perception);
				return this._passivePerception;
			}
		}

		public double savingThrowModCharisma
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.Charisma) + this.charismaMod + this.tempSavingThrowModCharisma;
			}
		}

		public double savingThrowModConstitution
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.Constitution) + this.constitutionMod + this.tempSavingThrowModConstitution;
			}
		}

		public double savingThrowModDexterity
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.Dexterity) + this.dexterityMod + this.tempSavingThrowModDexterity;
			}
		}

		public double savingThrowModIntelligence
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.Intelligence) + this.intelligenceMod + this.tempSavingThrowModIntelligence;
			}
		}

		public double savingThrowModStrength
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.Strength) + this.strengthMod + this.tempSavingThrowModStrength;
			}
		}

		public double savingThrowModWisdom
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.Wisdom) + this.wisdomMod + this.tempSavingThrowModWisdom;
			}
		}

		public double skillModAcrobatics
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.acrobatics) + this.dexterityMod + this.tempAcrobaticsMod;
			}
		}

		public double skillModAnimalHandling
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.animalHandling) + this.wisdomMod + this.tempAnimalHandlingMod;
			}
		}

		public double skillModArcana
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.arcana) + this.intelligenceMod + this.tempArcanaMod;
			}
		}

		public double skillModAthletics
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.athletics) + this.strengthMod + this.tempAthleticsMod;

			}
		}

		public double skillModDeception
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.deception) + this.charismaMod + this.tempDeceptionMod;

			}
		}

		public double skillModHistory
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.history) + this.intelligenceMod + this.tempHistoryMod;

			}
		}

		public double skillModInsight
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.insight) + this.wisdomMod + this.tempInsightMod;
			}
		}

		public double skillModIntimidation
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.intimidation) + this.charismaMod + this.tempIntimidationMod;

			}
		}

		public double skillModInvestigation
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.investigation) + this.intelligenceMod + this.tempInvestigationMod;
			}
		}

		public double skillModMedicine
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.medicine) + this.wisdomMod + this.tempMedicineMod;
			}
		}


		public double skillModNature
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.nature) + this.intelligenceMod + this.tempNatureMod;
			}
		}


		public double skillModPerception
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.perception) + this.wisdomMod + this.tempPerceptionMod;
			}
		}


		public double skillModPerformance
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.performance) + this.charismaMod + this.tempPerformanceMod;
			}
		}

		public double skillModPersuasion
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.persuasion) + this.charismaMod + this.tempPersuasionMod;
			}
		}


		public double skillModReligion
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.religion) + this.intelligenceMod + this.tempReligionMod;
			}
		}


		public double skillModSlightOfHand
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.slightOfHand) + this.dexterityMod + this.tempSlightOfHandMod;
			}
		}

		public double skillModStealth
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.stealth) + this.dexterityMod + this.tempStealthMod;
			}
		}

		public double skillModSurvival
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.survival) + this.wisdomMod + this.tempSurvivalMod;
			}
		}
		public double strengthMod
		{
			get
			{
				return this.getModFromAbility(this.Strength);
			}
		}
		public double wisdomMod
		{
			get
			{
				return this.getModFromAbility(this.Wisdom);
			}
		}

		public Vector WorldPosition { get; private set; }

		public void ApplyModPermanently(Mod mod, string description)
		{
			// TODO: Implement this!
		}

		public void ApplyModTemporarily(Mod mod, string description)
		{
			// TODO: Implement this!
		}

		int getModFromAbility(double abilityScore)
		{
			return (int)Math.Floor((abilityScore - 10) / 2);
		}

		double getProficiencyBonusForSavingThrow(Ability savingThrow)
		{
			if (this.hasSavingThrowProficiency(savingThrow))
				return this.proficiencyBonus;
			return 0;
		}

		double getProficiencyBonusMultiplier(Skills skill)
		{
			if (hasDoubleProficiencyBonusForSkill(skill))
				return 2;
			return 1;
		}

		double getProficiencyBonusForSkill(Skills skill)
		{
			if (hasProficiencyBonusForSkill(skill))
			{
				return proficiencyBonus * getProficiencyBonusMultiplier(skill);
			}
			return 0;
		}
		public Vector GetRoomCoordinates()
		{
			return Vector.zero;
		}

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
			return hasDoubleProficiencyBonusForSkill(skill) || (proficientSkills & skill) == skill;
		}

		bool hasDoubleProficiencyBonusForSkill(Skills skill)
		{
			return (doubleProficiency & skill) == skill;
		}

		bool hasSavingThrowProficiency(Ability ability)
		{
			return (savingThrowProficiency & ability) == ability;
		}
		public void SetWorldPosition(Vector worldPosition)
		{
			WorldPosition = worldPosition;
		}

		public void InflictDamage(double deltaDamage)
		{
			double damageToSubtract = deltaDamage;
			if (tempHitPoints > 0)
				if (damageToSubtract > tempHitPoints)
				{
					damageToSubtract -= tempHitPoints;
					tempHitPoints = 0;
				}
				else
				{
					tempHitPoints -= damageToSubtract;
					damageToSubtract = 0;
				}
			if (damageToSubtract > hitPoints)
				hitPoints = 0;
			else
				hitPoints -= damageToSubtract;
		}

		public void ChangeHealth(double damageHealthAmount)
		{
			if (damageHealthAmount < 0)
				InflictDamage(-damageHealthAmount);
			else
				Heal(damageHealthAmount);
		}

		public void Heal(double deltaHealth)
		{
			if (deltaHealth <= 0)
				return;

			if (hitPoints >= maxHitPoints)
				return;

			double maxToHeal = maxHitPoints - hitPoints;
			if (deltaHealth > maxToHeal)
				hitPoints = maxHitPoints;
			else
				hitPoints += deltaHealth;
		}

		public string ToJson()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}
		
		public int GetSpellcastingAbilityModifier()
		{
			switch (spellCastingAbility)
			{
				case Ability.Wisdom: return (int)Math.Floor(wisdomMod);
				case Ability.Charisma: return (int)Math.Floor(charismaMod);
				case Ability.Constitution: return (int)Math.Floor(constitutionMod);
				case Ability.Dexterity: return (int)Math.Floor(dexterityMod);
				case Ability.Intelligence: return (int)Math.Floor(intelligenceMod);
				case Ability.Strength: return (int)Math.Floor(strengthMod);
			}
			return 0;
		}
	}
}