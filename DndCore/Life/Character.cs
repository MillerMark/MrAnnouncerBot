using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DndCore
{
	public class Character : Creature
	{
		public event StateChangedEventHandler StateChanged;

		protected virtual void OnStateChanged(object sender, StateChangedEventArgs ea)
		{
			StateChanged?.Invoke(sender, ea);
		}

		void OnStateChanged(string key, object oldValue, object newValue)
		{
			OnStateChanged(this, new StateChangedEventArgs(key, oldValue, newValue));
		}

		public bool SpellCastingLock { get; set; }
		public int damageOffsetThisRoll = 0;
		public int advantageDiceThisRoll = 0;
		public int disadvantageDiceThisRoll = 0;
		public Ability checkingAbilities = Ability.none;
		public Ability savingAgainst = Ability.none;
		public Ability attackingAbility = Ability.none;
		public Skills checkingSkills = Skills.none;
		public string diceJustRolled = string.Empty;
		public AttackType attackingType = AttackType.None;
		public AttackKind attackingKind = AttackKind.Any;

		List<CharacterClass> classes = new List<CharacterClass>();

		public void ResetPlayerTurnBasedState()
		{
			checkingAbilities = Ability.none;
			savingAgainst = Ability.none;
			attackingAbility = Ability.none;
			checkingSkills = Skills.none;
			attackingType = AttackType.None;
			attackingKind = AttackKind.Any;
			ResetPlayerActionBasedState();
		}

		public void ResetPlayerActionBasedState()
		{
			diceJustRolled = string.Empty;
			damageOffsetThisRoll = 0;
			advantageDiceThisRoll = 0;
			disadvantageDiceThisRoll = 0;
		}

		public void StartTurn()
		{
			ResetPlayerTurnBasedState();
		}

		public void StartAction()
		{
			ResetPlayerActionBasedState();
		}

		public void EndAction()
		{
			ResetPlayerActionBasedState();
		}

		public void EndTurn()
		{
			ResetPlayerTurnBasedState();
		}

		public void AddClass(string name, int level)
		{
			classes.Add(new CharacterClass(name, level));
		}

		public string ClassLevelStr
		{
			get
			{
				string result = string.Empty;
				foreach (CharacterClass characterClass in classes)
				{
					if (result.Length > 0)
						result += " / ";
					result += characterClass.ToString();
				}
				return result;
			}
		}

		public string raceClass
		{
			get
			{
				return $"{race} {ClassLevelStr}";
			}
		}

		public bool playingNow { get; set; }
		public int playerID { get; set; }
		public int ShieldBonus { get; set; }
		public int leftMostPriority { get; set; }
		double _passivePerception = int.MinValue;
		public bool deathSaveDeath1 = false;
		public bool deathSaveDeath2 = false;
		public bool deathSaveDeath3 = false;
		public bool deathSaveLife1 = false;
		public bool deathSaveLife2 = false;
		public bool deathSaveLife3 = false;
		public int experiencePoints = 0;
		public string inspiration = "";
		public int level
		{
			get
			{
				int totalLevel = 0;
				foreach (CharacterClass playerClass in classes)
				{
					totalLevel += playerClass.Level;
				}
				return totalLevel;
			}
		}
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
		public Ability spellCastingAbility = Ability.none;
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
				return this.hasSavingThrowProficiency(Ability.charisma);
			}
		}

		bool hasSavingThrowProficiencyConstitution
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.constitution);
			}
		}
		bool hasSavingThrowProficiencyDexterity
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.dexterity);
			}

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
		bool hasSavingThrowProficiencyWisdom
		{
			get
			{
				return this.hasSavingThrowProficiency(Ability.wisdom);
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
				return this.getProficiencyBonusForSavingThrow(Ability.charisma) + this.charismaMod + this.tempSavingThrowModCharisma;
			}
		}

		public double savingThrowModConstitution
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.constitution) + this.constitutionMod + this.tempSavingThrowModConstitution;
			}
		}

		public double savingThrowModDexterity
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.dexterity) + this.dexterityMod + this.tempSavingThrowModDexterity;
			}
		}

		public double savingThrowModIntelligence
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.intelligence) + this.intelligenceMod + this.tempSavingThrowModIntelligence;
			}
		}

		public double savingThrowModStrength
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.strength) + this.strengthMod + this.tempSavingThrowModStrength;
			}
		}

		public double savingThrowModWisdom
		{
			get
			{
				return this.getProficiencyBonusForSavingThrow(Ability.wisdom) + this.wisdomMod + this.tempSavingThrowModWisdom;
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
		public Weapons weaponProficiency { get; set; }
		public List<CharacterClass> Classes { get => classes; set => classes = value; }
		public List<string> features { get; set; }

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

		public int GetSpellAttackModifier()
		{
			return (int)Math.Round(proficiencyBonus) + GetSpellcastingAbilityModifier();
		}

		public int GetSpellSaveDC()
		{
			return 8 + GetSpellAttackModifier();
		}

		public int GetSpellcastingAbilityModifier()
		{
			switch (spellCastingAbility)
			{
				case Ability.wisdom: return (int)Math.Floor(wisdomMod);
				case Ability.charisma: return (int)Math.Floor(charismaMod);
				case Ability.constitution: return (int)Math.Floor(constitutionMod);
				case Ability.dexterity: return (int)Math.Floor(dexterityMod);
				case Ability.intelligence: return (int)Math.Floor(intelligenceMod);
				case Ability.strength: return (int)Math.Floor(strengthMod);
			}
			return 0;
		}

		public static Character From(CharacterDto characterDto)
		{
			var character = new Character();
			character.playerID = PlayerID.FromName(characterDto.name);
			character.features = GetFeatures(characterDto.features);
			character.name = characterDto.name;
			character.playingNow = !string.IsNullOrWhiteSpace(characterDto.playingNow);
			character.race = characterDto.race;
			string class1 = characterDto.class1;
			int level1 = MathUtils.GetInt(characterDto.level1);
			if (!string.IsNullOrEmpty(class1) && level1 > 0)
				character.AddClass(class1, level1);

			string class2 = characterDto.class2;
			int level2 = MathUtils.GetInt(characterDto.level2);
			if (!string.IsNullOrEmpty(class2) && level2 > 0)  // Multi-class
				character.AddClass(class2, level2);

			character.hitPoints = characterDto.hitPoints;
			character.maxHitPoints = characterDto.maxHitPoints;
			character.baseArmorClass = characterDto.baseArmorClass;
			character.goldPieces = characterDto.goldPieces;
			character.baseStrength = characterDto.baseStrength;
			character.baseDexterity = characterDto.baseDexterity;
			character.baseConstitution = characterDto.baseConstitution;
			character.baseIntelligence = characterDto.baseIntelligence;
			character.baseWisdom = characterDto.baseWisdom;
			character.baseCharisma = characterDto.baseCharisma;
			character.proficiencyBonus = characterDto.proficiencyBonus;
			character.baseWalkingSpeed = characterDto.walking;
			character.proficientSkills = DndUtils.ToSkill(characterDto.proficientSkills);
			character.doubleProficiency = DndUtils.ToSkill(characterDto.doubleProficiency);
			character.savingThrowProficiency = DndUtils.ToAbility(characterDto.savingThrowProficiency);
			character.spellCastingAbility = DndUtils.ToAbility(characterDto.spellCastingAbility);
			character.initiative = characterDto.initiative;
			character.rollInitiative = DndUtils.ToVantage(characterDto.rollInitiative);
			character.hueShift = characterDto.hueShift;
			character.dieBackColor = characterDto.dieBackColor;
			character.dieFontColor = characterDto.dieFontColor;
			character.headshotIndex = characterDto.headshotIndex;
			character.alignment = characterDto.alignment;
			character.weaponProficiency = DndUtils.ToWeapon(characterDto.weaponProficiency);
			character.leftMostPriority = characterDto.leftMostPriority;
			return character;
		}

		private static List<string> GetFeatures(string featureListStr)
		{
			List<string> results = new List<string>();
			List<string> features = featureListStr.Split(';').ToList();
			foreach (string feature in features)
			{
				string trimmedFeature = feature.Trim();
				if (!string.IsNullOrEmpty(trimmedFeature))
					results.Add(trimmedFeature);
			}
			return results;
		}

		public int GetAbilityModifier(WeaponProperties weaponProperties, AttackType attackType)
		{
			double abilityModifier;
			double dexterityModifier = GetAbilityModifier(Ability.dexterity);
			double strengthModifier = GetAbilityModifier(Ability.strength);

			attackingType = attackType;

			if ((weaponProperties | WeaponProperties.Finesse) == WeaponProperties.Finesse)
			{
				abilityModifier = Math.Max(dexterityModifier, strengthModifier);

				if (dexterityModifier > strengthModifier)
					attackingAbility = Ability.dexterity;
				else
					attackingAbility = Ability.strength;

			}
			else if (attackType == AttackType.Range)
			{
				abilityModifier = dexterityModifier;
				attackingAbility = Ability.dexterity;
			}
			else
			{
				abilityModifier = strengthModifier;
				attackingAbility = Ability.strength;
			}

			return (int)Math.Round(abilityModifier);
		}

		public void SetAbilities(int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma)
		{
			baseCharisma = charisma;
			baseConstitution = constitution;
			baseDexterity = dexterity;
			baseWisdom = wisdom;
			baseIntelligence = intelligence;
			baseStrength = strength;
		}

		public bool IsProficientWith(Weapons weapon)
		{
			if (weapon == Weapons.None)
				return true;
			return (weaponProficiency & weapon) == weapon;
		}

		public bool IsProficientWith(string name)
		{
			Weapons weapon = DndUtils.ToWeapon(DndUtils.GetCleanItemName(name));
			return IsProficientWith(weapon);
		}

		public int GetLevel(string characterClassName)
		{
			CharacterClass characterClass = classes.FirstOrDefault(x => string.Compare(x.Name, characterClassName, true) == 0);
			if (characterClass == null)
				return 0;
			return characterClass.Level;
		}

		Dictionary<string, object> states = new Dictionary<string, object>();
		public void SetState(string key, object newValue)
		{
			if (states.ContainsKey(key))
			{
				if (states[key] == newValue)
					return;
				object oldState = states[key];
				states[key] = newValue;
				OnStateChanged(key, oldState, newValue);
			}
			else
			{
				states.Add(key, newValue);
				OnStateChanged(key, null, newValue);
			}
		}

		public object GetState(string key)
		{
			if (states.ContainsKey(key))
				return states[key];
			return null;
		}

		public void ClearStateVariables()
		{
			states.Clear();
		}

		public void RollDice(string diceStr)
		{
			diceJustRolled = diceStr;
			// TODO: Implement this.
		}

		public void GiveAdvantageThisRoll()
		{
			advantageDiceThisRoll++;
		}

		public void GiveDisadvantageThisRoll()
		{
			disadvantageDiceThisRoll++;
		}

		// TODO: Incorporate GetVantageThisRoll into die rolls.
		public VantageKind GetVantageThisRoll()
		{
			if (advantageDiceThisRoll > 0 && disadvantageDiceThisRoll > 0)
				return VantageKind.Normal;

			if (advantageDiceThisRoll > 0)
				return VantageKind.Advantage;

			if (disadvantageDiceThisRoll > 0)
				return VantageKind.Disadvantage;

			return VantageKind.Normal;
		}

		/// <summary>
		/// Returns an array of ints indexed by spell slot level of the number of spell slots available for this character.
		/// </summary>
		/// <returns></returns>
		public int[] GetSpellSlotLevels()
		{
			int[] results = new int[10];
			for (int i = 0; i < 10; i++)
			{
				results[i] = 0;
			}
			foreach (CharacterClass characterClass in Classes)
			{
				if (DndUtils.CanCastSpells(characterClass.Name))
				{
					results[1] = DndUtils.GetAvailableSpellSlots(characterClass, 1);
					results[2] = DndUtils.GetAvailableSpellSlots(characterClass, 2);
					results[3] = DndUtils.GetAvailableSpellSlots(characterClass, 3);
					results[4] = DndUtils.GetAvailableSpellSlots(characterClass, 4);
					results[5] = DndUtils.GetAvailableSpellSlots(characterClass, 5);
				}
			}
			return results;
		}

		public void ActivateFeature(string featureNameStr)
		{
			string foundFeature = features.FirstOrDefault(x => DndUtils.GetCleanItemName(x.ToLower()) == DndUtils.GetCleanItemName(featureNameStr).ToLower());
			if (string.IsNullOrWhiteSpace(foundFeature))
				return;

			List<string> parameters = Feature.GetParameters(foundFeature);
			string featureName = string.Empty;
			if (foundFeature.IndexOf("(") >= 0)
				featureName = foundFeature.EverythingBefore("(");
			else
				featureName = foundFeature;

			Feature feature = AllFeatures.Get(featureName);
			if (feature == null)
				return;

			string joinedParameters = string.Join(",", parameters);
			feature.Activate(joinedParameters, this);
		}
	}
}