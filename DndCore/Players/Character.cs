using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DndCore
{
	public class Character : Creature
	{
		const string STR_RechargeableMaxSuffix = "_max";
		double _passivePerception = int.MinValue;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public List<SpellGroup> SpellData { get; private set; }

		[JsonIgnore]
		public string emoticon { get; set; }

		[JsonIgnore]
		public int HighestSpellSlot
		{
			get
			{
				int[] spellSlotLevels = GetSpellSlotLevels();
				for (int i = 0; i < spellSlotLevels.Length; i++)
				{
					if (spellSlotLevels[i] == 0)
						return i - 1;
				}
				return -1;
			}
		}
		

		[JsonIgnore]
		public List<Rechargeable> rechargeables = new List<Rechargeable>();

		[JsonIgnore]
		public string additionalDiceThisRoll = string.Empty;
		[JsonIgnore]
		public int advantageDiceThisRoll = 0;
		[JsonIgnore]
		public Ability attackingAbility = Ability.none;
		[JsonIgnore]
		public double attackingAbilityModifier = 0; // TODO: Set for spells?
		[JsonIgnore]
		public AttackKind attackingKind = AttackKind.Any;
		[JsonIgnore]
		public AttackType attackingType = AttackType.None;
		[JsonIgnore]
		public int attackOffsetThisRoll = 0;
		[JsonIgnore]
		bool casting;
		[JsonIgnore]
		public Ability checkingAbilities = Ability.none;
		[JsonIgnore]
		public Skills checkingSkills = Skills.none;

		[JsonIgnore]
		public Vector WorldPosition { get; private set; }

		[JsonIgnore]
		public CastedSpell concentratedSpell;

		[JsonIgnore]
		public List<AssignedFeature> features { get; set; }

		[JsonIgnore]
		public int damageOffsetThisRoll = 0;

		public ActiveSpellData spellActivelyCasting;
		public ActiveSpellData spellPreviouslyCasting;

		public bool forceShowSpell = false;

		public bool deathSaveDeath1 = false;
		public bool deathSaveDeath2 = false;
		public bool deathSaveDeath3 = false;
		public bool deathSaveLife1 = false;
		public bool deathSaveLife2 = false;
		public bool deathSaveLife3 = false;
		public string diceWeAreRolling = string.Empty;
		public string dieBackColor = "#ffffff";
		public string dieFontColor = "#000000";
		public string dieRollEffectsThisRoll = string.Empty;
		public string dieRollMessageThisRoll = string.Empty;
		public int disadvantageDiceThisRoll = 0;
		public Skills doubleProficiency = 0;
		int enemyAdvantage;
		public int ActionsPerTurn;

		bool evaluatingExpression;
		public int experiencePoints = 0;
		public int headshotIndex;

		[JsonIgnore]
		public bool hitWasCritical = false;  // TODO: Implement this + test cases.

		[JsonIgnore]
		public int _attackNum = 0;

		public int hueShift = 0;
		public string inspiration = string.Empty;
		public double load = 0;
		public double proficiencyBonus = 0;
		public Skills proficientSkills = 0;

		[JsonIgnore]
		RecalcOptions queuedRecalcOptions = RecalcOptions.None;

		[JsonIgnore]
		bool reapplyingActiveFeatures;

		public string remainingHitDice = string.Empty;

		public VantageKind rollInitiative = VantageKind.Normal;

		public Ability savingAgainst = Ability.none;
		public Ability savingThrowProficiency = 0;
		public Ability spellCastingAbility = Ability.none;

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public string SpellCastingAbilityStr
		{
			get
			{
				return DndUtils.ToAbilityDisplayString(spellCastingAbility);
			}
		}
		

		[JsonIgnore]
		CastedSpell spellToCast;

		[JsonIgnore]
		Dictionary<string, object> states = new Dictionary<string, object>();

		[JsonIgnore]
		public Creature targetedCreature;

		public int targetedCreatureHitPoints = 0;  // TODO: Implement this + test cases.
		public bool targetThisRollIsCreature = false;
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
		public double tempSleightOfHandMod = 0;
		public double tempStealthMod = 0;
		public double tempSurvivalMod = 0;
		public string totalHitDice = string.Empty;
		public string trailingEffectsThisRoll = string.Empty;
		public bool usesMagicThisRoll = false;
		public double weight = 0;

		[JsonIgnore]
		public List<CarriedAmmunition> CarriedAmmunition { get; private set; } = new List<CarriedAmmunition>();

		[JsonIgnore]
		public List<CarriedWeapon> CarriedWeapons { get; private set; } = new List<CarriedWeapon>();

		
		//// TODO: Set this to true when casting and false when no longer casting.
		//public bool Casting
		//{
		//	get { return casting; }
		//	set
		//	{
		//		bool oldValue = casting;
		//		bool newValue = value;

		//		if (oldValue == newValue)
		//			return;

		//		casting = value;
		//		OnStateChanged("Casting", oldValue, newValue);
		//	}
		//}

		public double charismaMod
		{
			get
			{
				return this.getModFromAbility(this.Charisma);
			}
		}
		public List<CharacterClass> Classes { get; set; } = new List<CharacterClass>();

		public string ClassLevelStr
		{
			get
			{
				string result = string.Empty;
				foreach (CharacterClass characterClass in Classes)
				{
					if (result.Length > 0)
						result += " / ";
					result += characterClass.ToString();
				}
				return result;
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


		public int EnemyAdvantage
		{
			get { return enemyAdvantage; }
			set
			{
				enemyAdvantage = value;
				if (enemyAdvantage < -1)
					enemyAdvantage = -1;

				if (enemyAdvantage > 1)
					enemyAdvantage = 1;
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

		bool hasSkillProficiencySleightOfHand
		{
			get
			{
				return hasProficiencyBonusForSkill(Skills.sleightOfHand);
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

		[JsonIgnore]
		public List<KnownSpell> KnownSpells { get; private set; } = new List<KnownSpell>();

		public int leftMostPriority { get; set; }
		public int level
		{
			get
			{
				int totalLevel = 0;
				foreach (CharacterClass playerClass in Classes)
				{
					totalLevel += playerClass.Level;
				}
				return totalLevel;
			}
		}
		public bool OneHanded { get; set; }  // TODO: Implement this + test cases.

		public double PassivePerception
		{
			get
			{
				if (this._passivePerception == int.MinValue)
					this._passivePerception = 10 + this.wisdomMod + this.getProficiencyBonusForSkill(Skills.perception);
				return this._passivePerception;
			}
		}
		public int playerID { get; set; }

		public bool playingNow { get; set; }

		public string raceClass
		{
			get
			{
				return $"{race} {ClassLevelStr}";
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
		public int ShieldBonus { get; set; }

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


		public double skillModSleightOfHand
		{
			get
			{
				return this.getProficiencyBonusForSkill(Skills.sleightOfHand) + this.dexterityMod + this.tempSleightOfHandMod;
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

		public bool SpellCastingLock { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public int SpellSaveDC
		{
			get
			{
				return GetSpellSaveDC();
			}
		}
		public double strengthMod
		{
			get
			{
				return this.getModFromAbility(this.Strength);
			}
		}
		public bool TwoHanded { get; set; }  // TODO: Implement this + test cases.
		public bool WeaponIsHeavy { get; set; }  // TODO: Implement this + test cases.
		public Weapons weaponProficiency { get; set; }
		public int WeaponsInHand { get; set; }  // TODO: Implement this (0, 1 or 2) + test cases.
		public double wisdomMod
		{
			get
			{
				return this.getModFromAbility(this.Wisdom);
			}
		}

		public static Character From(CharacterDto characterDto)
		{
			var character = new Character();
			character.playerID = PlayerID.FromName(characterDto.name);
			character.features = GetFeatures(characterDto.features, character);
			character.name = characterDto.name;
			character.playingNow = !string.IsNullOrWhiteSpace(characterDto.playingNow);
			character.race = characterDto.race;
			character.heShe = characterDto.heShe;
			character.hisHer = characterDto.hisHer;
			character.himHer = characterDto.himHer;
			character.HeShe = characterDto.heShe.InitialCap();
			character.HisHer = characterDto.hisHer.InitialCap();
			string class1 = characterDto.class1;
			int level1 = MathUtils.GetInt(characterDto.level1);
			SubClass subClass1 = DndUtils.ToSubClass(characterDto.subclass1);
			if (!string.IsNullOrEmpty(class1) && level1 > 0)
				character.AddClass(class1, level1).SubClass = subClass1;

			string class2 = characterDto.class2;
			SubClass subClass2 = DndUtils.ToSubClass(characterDto.subclass2);
			int level2 = MathUtils.GetInt(characterDto.level2);
			if (!string.IsNullOrEmpty(class2) && level2 > 0)  // Multi-class
				character.AddClass(class2, level2).SubClass = subClass2;

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
			character.emoticon = characterDto.emoticon;
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
			character.AddWeaponsFrom(characterDto.weapons);
			character.AddAmmunitionFrom(characterDto.ammunition);
			character.AddSpellsFrom(characterDto.spells);
			character.weaponProficiency = DndUtils.ToWeapon(characterDto.weaponProficiency);
			character.leftMostPriority = characterDto.leftMostPriority;
			return character;
		}

		static List<AssignedFeature> GetFeatures(string featureListStr, Character player)
		{
			List<AssignedFeature> results = new List<AssignedFeature>();
			List<string> features = featureListStr.Split(';').ToList();
			foreach (string feature in features)
			{
				string trimmedFeature = feature.Trim();
				if (!string.IsNullOrEmpty(trimmedFeature))
					results.Add(AssignedFeature.From(trimmedFeature, player));
			}
			return results;
		}

		public void ActivateFeature(string featureNameStr)
		{
			AssignedFeature foundFeature = features.FirstOrDefault(x => DndUtils.GetCleanItemName(x.Feature.Name.ToLower()) == DndUtils.GetCleanItemName(featureNameStr).ToLower());
			if (foundFeature == null)
				return;

			foundFeature.Activate();
		}

		void AddAmmunition(string ammunitionStr)
		{
			if (string.IsNullOrWhiteSpace(ammunitionStr))
				return;
			int count = 0;
			string ammunitionName = ammunitionStr;
			if (ammunitionStr.Has("["))
			{
				if (ammunitionName.Has("["))
					ammunitionName = ammunitionName.EverythingBefore("[");

				int.TryParse(ammunitionStr.EverythingBetween("[", "]").Trim(), out count);
			}

			CarriedAmmunition carriedAmmunition = new CarriedAmmunition();
			carriedAmmunition.Name = ammunitionName;
			carriedAmmunition.Count = count;
			CarriedAmmunition.Add(carriedAmmunition);
		}
		void AddAmmunitionFrom(string ammunitionStr)
		{
			if (string.IsNullOrWhiteSpace(ammunitionStr))
				return;
			string[] ammunitionStrs = ammunitionStr.Split(';');
			foreach (var ammunition in ammunitionStrs)
			{
				AddAmmunition(ammunition.Trim());
			}
		}

		public CharacterClass AddClass(string name, int level)
		{
			CharacterClass characterClass = new CharacterClass(name, level);
			characterClass.LevelChanged += CharacterClass_LevelChanged;
			Classes.Add(characterClass);
			return characterClass;
		}

		public void AddDice(string diceStr)
		{
			additionalDiceThisRoll = diceStr;
		}

		public void AddDieRollEffects(string dieRollEffects)
		{
			if (!string.IsNullOrWhiteSpace(dieRollEffectsThisRoll))
				dieRollEffectsThisRoll += ";";
			dieRollEffectsThisRoll += dieRollEffects;
		}

		public void AddDieRollMessage(string dieRollMessage)
		{
			dieRollMessageThisRoll = dieRollMessage;
		}

		public void AddSpell(string spellStr)
		{
			if (string.IsNullOrWhiteSpace(spellStr))
				return;
			int totalCharges = int.MaxValue;
			DndTimeSpan chargeResetSpan = DndTimeSpan.Zero;
			string spellName = spellStr;
			string itemName = string.Empty;
			DndTimeSpan rechargesAt = DndTimeSpan.FromSeconds(0);  // midnight;
			if (spellName.Has("("))
			{
				spellName = spellName.EverythingBefore("(");
				if (HasSpell(spellName))
					return;

				string parameterStr = spellStr.EverythingBetween("(", ")");
				var parameters = parameterStr.Split(',');

				for (int i = 0; i < parameters.Length; i++)
				{
					var parameter = parameters[i].Trim();
					if (string.IsNullOrWhiteSpace(parameter))
						continue;

					if (i == 0)
						itemName = parameter.EverythingBetween("\"", "\"");
					else if (i == 1)
					{
						var chargeDetails = parameter.Split('/');
						if (chargeDetails.Length == 2)
						{
							int.TryParse(chargeDetails[0], out totalCharges);
							string durationStr = chargeDetails[1];
							chargeResetSpan = DndTimeSpan.FromDurationStr(durationStr);
							if (durationStr == "dawn")
								rechargesAt = DndTimeSpan.FromHours(6);  // 6:00 am
						}
					}
				}
			}
			else if (HasSpell(spellName))
				return;
			KnownSpell knownSpell = new KnownSpell();
			knownSpell.SpellName = spellName;
			knownSpell.RechargesAt = rechargesAt.GetTimeSpan();
			knownSpell.TotalCharges = totalCharges;
			knownSpell.ChargesRemaining = totalCharges;
			knownSpell.ResetSpan = chargeResetSpan;
			knownSpell.ItemName = itemName;
			knownSpell.Player = this;
			KnownSpells.Add(knownSpell);
		}

		private bool HasSpell(string spellName)
		{
			return KnownSpells.FirstOrDefault(x => x.SpellName == spellName) != null;
		}

		public void AddSpellsFrom(string spellsStr)
		{
			if (string.IsNullOrWhiteSpace(spellsStr))
				return;

			string[] spellsStrs = spellsStr.Split(';');
			foreach (var spell in spellsStrs)
			{
				AddSpell(spell.Trim());
			}
		}

		public void RemoveSpell(string name)
		{
			KnownSpell foundSpell = KnownSpells.FirstOrDefault(x => x.SpellName == name);
			if (foundSpell == null)
				return;
			KnownSpells.Remove(foundSpell);
		}

		public void AddTrailingEffects(string trailingEffects)
		{
			if (!string.IsNullOrWhiteSpace(trailingEffectsThisRoll))
				trailingEffectsThisRoll += ";";
			trailingEffectsThisRoll += trailingEffects;
		}

		void AddWeapon(string weaponStr)
		{
			if (string.IsNullOrWhiteSpace(weaponStr))
				return;

			string weaponName = weaponStr;
			string parametersStr = string.Empty;
			int count = 1;

			if (weaponStr.Has("("))
			{
				parametersStr = weaponStr.EverythingBetween("(", ")").Trim();
				weaponName = weaponStr.EverythingBefore("(").Trim();
			}

			if (weaponStr.Has("["))
			{
				if (weaponName.Has("["))
					weaponName = weaponName.EverythingBefore("[");

				int.TryParse(weaponStr.EverythingBetween("[", "]").Trim(), out count);
			}

			CarriedWeapon carriedWeapon = new CarriedWeapon();

			if (parametersStr.HasSomething())
			{
				string[] parameters = parametersStr.Split(',');
				for (int i = 0; i < parameters.Length; i++)
				{
					var parameter = parameters[i].Trim();
					switch (i)
					{
						case 0:
							carriedWeapon.Name = parameter.EverythingBetween("\"", "\"");
							break;
						case 1:
							carriedWeapon.HitDamageBonus = MathUtils.GetInt(parameter);
							break;
						case 2:
							carriedWeapon.WeaponHue = parameter;
							break;
						case 3:
							carriedWeapon.Hue1 = parameter;
							break;
						case 4:
							carriedWeapon.Hue2 = parameter;
							break;
						case 5:
							carriedWeapon.Hue3 = parameter;
							break;
					}
				}
			}

			Weapon weapon = AllWeapons.Get(weaponName);
			carriedWeapon.Weapon = weapon;
			carriedWeapon.Count = count;
			CarriedWeapons.Add(carriedWeapon);
		}

		void AddWeaponsFrom(string weaponsStr)
		{
			if (string.IsNullOrWhiteSpace(weaponsStr))
				return;
			string[] weaponStrs = weaponsStr.Split(';');
			foreach (var weapon in weaponStrs)
			{
				AddWeapon(weapon.Trim());
			}
		}

		public void ApplyModPermanently(Mod mod, string description)
		{
			// TODO: Implement this!
		}

		public void ApplyModTemporarily(Mod mod, string description)
		{
			// TODO: Implement this!
		}

		public void BreakConcentration()
		{
			if (concentratedSpell == null)
				return;

			OnSpellDispelled(this, new CastedSpellEventArgs(Game, concentratedSpell));
			concentratedSpell = null;
		}
		public CastedSpell Cast(Spell spell, Creature targetCreature = null)
		{
			spellToCast = null;

			if (Game != null)
				spellToCast = Game.Cast(this, spell, targetCreature);

			return spellToCast;
		}

		public void CastingSpellRequiringConcentration(CastedSpell spell)
		{
			BreakConcentration();
			concentratedSpell = spell;
		}

		public void CheckConcentration(CastedSpell castedSpell)
		{
			if (castedSpell.Spell.RequiresConcentration)
				CastingSpellRequiringConcentration(castedSpell);
		}

		public void ShowPlayerCasting(CastedSpell castedSpell)
		{
			forceShowSpell = true;
			spellPreviouslyCasting = null;
			spellActivelyCasting = ActiveSpellData.FromCastedSpell(castedSpell);
			OnStateChanged(this, new StateChangedEventArgs("spellActivelyCasting", null, null));
		}

		public void ClearAllCasting()
		{
			if (spellPreviouslyCasting == null && spellActivelyCasting == null)
				return;
			spellPreviouslyCasting = null;
			spellActivelyCasting = null;
			OnStateChanged(this, new StateChangedEventArgs("spellActivelyCasting", null, null));
		}

		public void ChangeHealth(double damageHealthAmount)
		{
			if (damageHealthAmount < 0)
				InflictDamage(-damageHealthAmount);
			else
				Heal(damageHealthAmount);
		}

		void CharacterClass_LevelChanged(object sender, LevelChangedEventArgs ea)
		{
			if (sender is CharacterClass characterClass)
				OnStateChanged($"Level[{characterClass.Name}]", ea.OldLevel, ea.NewLevel);
		}

		public void ClearStateVariables()
		{
			states.Clear();
		}

		public void CompletingExpressionEvaluation()
		{
			evaluatingExpression = false;
			Recalculate(queuedRecalcOptions);
			queuedRecalcOptions = RecalcOptions.None;
		}

		public void DeactivateFeature(string featureNameStr)
		{
			AssignedFeature foundFeature = features.FirstOrDefault(x => DndUtils.GetCleanItemName(x.Feature.Name.ToLower()) == DndUtils.GetCleanItemName(featureNameStr).ToLower());
			if (foundFeature == null)
				return;

			foundFeature.Deactivate();
		}

		public void Dispel(CastedSpell castedSpell)
		{
			if (Game != null)
				Game.Dispel(castedSpell);
			if (concentratedSpell?.Spell?.Name == castedSpell?.Spell?.Name)
				concentratedSpell = null;
		}

		public void EndAction()
		{
			ResetPlayerActionBasedState();
		}

		public void EndTurn()
		{
			if (Game != null)
				Game.EndingTurnFor(this);
			EndTurnResetState();
		}

		public override void EndTurnResetState()
		{
			ResetPlayerActionBasedState();
			ResetPlayerRollBasedState();
		}

		public List<CastedSpell> GetActiveSpells()
		{
			return Game.GetActiveSpells(this);
		}

		public int GetAmmunitionCount(string str)
		{
			CarriedAmmunition ammo = CarriedAmmunition.FirstOrDefault(x => x.Name == str);
			if (ammo == null)
				return 0;
			return ammo.Count;
		}

		public int GetAttackingAbilityModifier(WeaponProperties weaponProperties, AttackType attackType)
		{
			double dexterityModifier = GetAbilityModifier(Ability.dexterity);
			double strengthModifier = GetAbilityModifier(Ability.strength);

			attackingType = attackType;

			if ((weaponProperties & WeaponProperties.Finesse) == WeaponProperties.Finesse)
			{
				attackingAbilityModifier = Math.Max(dexterityModifier, strengthModifier);

				if (dexterityModifier > strengthModifier)
					attackingAbility = Ability.dexterity;
				else
					attackingAbility = Ability.strength;

			}
			else if (attackType == AttackType.Range)
			{
				attackingAbilityModifier = dexterityModifier;
				attackingAbility = Ability.dexterity;
			}
			else
			{
				attackingAbilityModifier = strengthModifier;
				attackingAbility = Ability.strength;
			}

			return (int)Math.Round(attackingAbilityModifier);
		}

		public AssignedFeature GetFeature(string name)
		{
			return features.FirstOrDefault(x => DndUtils.GetCleanItemName(x.Feature.Name.ToLower()) ==
			DndUtils.GetCleanItemName(name.ToLower()));
		}

		public int GetLevel(string characterClassName)
		{
			CharacterClass characterClass = Classes.FirstOrDefault(x => string.Compare(x.Name, characterClassName, true) == 0);
			if (characterClass == null)
				return 0;
			return characterClass.Level;
		}

		public int GetLevel(Class dndClass)
		{
			CharacterClass characterClass = Classes.FirstOrDefault(x => x.Class == dndClass);
			if (characterClass == null)
				return 0;
			return characterClass.Level;
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

		double getProficiencyBonusForSkill(Skills skill)
		{
			if (hasProficiencyBonusForSkill(skill))
			{
				return proficiencyBonus * getProficiencyBonusMultiplier(skill);
			}
			return 0;
		}

		double getProficiencyBonusMultiplier(Skills skill)
		{
			if (hasDoubleProficiencyBonusForSkill(skill))
				return 2;
			return 1;
		}
		public Vector GetRoomCoordinates()
		{
			return Vector.zero;
		}

		/// <summary>
		/// Returns the first action shortcut for this player that starts with the specified name.
		/// </summary>
		/// <param name="shortcutName"></param>
		/// <returns></returns>
		public PlayerActionShortcut GetShortcut(string shortcutName)
		{
			List<PlayerActionShortcut> shortcuts = AllActionShortcuts.Get(playerID, shortcutName);
			if (shortcuts != null && shortcuts.Count > 0)
				return shortcuts[0];
			return null;
		}

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public string SpellAttackBonusStr
		{
			get
			{
				string bonusStr;
				int bonus = SpellAttackBonus;
				if (bonus > 0)
					bonusStr = "+" + bonus.ToString();
				else
					bonusStr = bonus.ToString();
				return bonusStr;
			}
		}
		
		[JsonIgnore]
		public int SpellAttackBonus
		{
			get
			{
				return (int)Math.Round(proficiencyBonus) + GetSpellcastingAbilityModifier();
			}
		}

		[JsonIgnore]
		public int SpellcastingAbilityModifier
		{
			get
			{
				return GetSpellcastingAbilityModifier();
			}
			set
			{
			}
		}

		[JsonIgnore]
		public string SpellcastingAbilityModifierStr
		{
			get
			{
				int spellcastingAbilityModifier = GetSpellcastingAbilityModifier();
				if (spellcastingAbilityModifier >= 0)
					return "+" + spellcastingAbilityModifier.ToString();
				return spellcastingAbilityModifier.ToString();
			}
			set
			{
			}
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

		public int GetSpellSaveDC()
		{
			return 8 + SpellAttackBonus;
		}

		/// <summary>
		/// Returns an array of ints indexed by spell slot level of the number of spell slots available for this character.
		/// </summary>
		/// <returns></returns>
		public int[] GetSpellSlotLevels()
		{
			int[] results = new int[10];
			for (int i = 0; i < 10; i++)
				results[i] = 0;

			foreach (CharacterClass characterClass in Classes)
			{
				AddAvailableSpellSlots(results, characterClass.Name, characterClass.Level);
				if (characterClass.SubClass != SubClass.None)
					AddAvailableSpellSlots(results, Enum.GetName(typeof(SubClass), characterClass.SubClass), characterClass.Level);
			}
			return results;
		}

		private static void AddAvailableSpellSlots(int[] results, string name, int level)
		{
			if (DndUtils.CanCastSpells(name))
			{
				// BUG: If a character multi-classes in two classes that can both cast spells, should we keep the spell slots separate?
				results[1] += DndUtils.GetAvailableSpellSlots(name, level, 1);
				results[2] += DndUtils.GetAvailableSpellSlots(name, level, 2);
				results[3] += DndUtils.GetAvailableSpellSlots(name, level, 3);
				results[4] += DndUtils.GetAvailableSpellSlots(name, level, 4);
				results[5] += DndUtils.GetAvailableSpellSlots(name, level, 5);
				results[6] += DndUtils.GetAvailableSpellSlots(name, level, 6);
				results[7] += DndUtils.GetAvailableSpellSlots(name, level, 7);
				results[8] += DndUtils.GetAvailableSpellSlots(name, level, 8);
				results[9] += DndUtils.GetAvailableSpellSlots(name, level, 9);
			}
		}

		public object GetState(string key)
		{
			if (states.ContainsKey(key))
				return states[key];
			Rechargeable rechargeable = rechargeables.FirstOrDefault(x => x.VarName == key);
			if (rechargeable != null)
				return rechargeable.ChargesUsed;

			rechargeable = rechargeables.FirstOrDefault(x => x.VarName + STR_RechargeableMaxSuffix == key);
			if (rechargeable != null)
				return rechargeable.TotalCharges;

			return null;
		}

		int GetIntState(string key)
		{
			object result = GetState(key);
			if (result == null)
				return 0;
			return (int)result;
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

		public void GiveAdvantageThisRoll()
		{
			advantageDiceThisRoll++;
		}

		public void GiveDisadvantageThisRoll()
		{
			disadvantageDiceThisRoll++;
		}

		bool hasDoubleProficiencyBonusForSkill(Skills skill)
		{
			return (doubleProficiency & skill) == skill;
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

		bool hasSavingThrowProficiency(Ability ability)
		{
			return (savingThrowProficiency & ability) == ability;
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

		public bool HoldsState(string key)
		{
			return states.ContainsKey(key);
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
		public void JustCastSpell(string spellName)
		{
			Spell spellJustCast = AllSpells.Get(spellName);
			if (spellJustCast == null)
				return;

			// HACK: Need a CastedSpell for the event. TimeSpellWasCast & SpellSlotLevel will not be correct.
			CastedSpell castedSpell = new CastedSpell(spellJustCast, this);
			Expressions.BeginUpdate();
			try
			{
				foreach (AssignedFeature assignedFeature in features)
				{
					assignedFeature.SpellJustCast(this, castedSpell);
				}
			}
			finally
			{
				Expressions.EndUpdate(this);
			}
		}

		public void JustSwungWeapon()
		{
			Expressions.BeginUpdate();
			try
			{
				foreach (AssignedFeature assignedFeature in features)
				{
					assignedFeature.WeaponJustSwung(this);
				}
			}
			finally
			{
				Expressions.EndUpdate(this);
			}
		}

		public void PlayerStartsTurn()
		{
			Expressions.BeginUpdate();
			try
			{
				foreach (AssignedFeature assignedFeature in features)
				{
					assignedFeature.PlayerStartsTurn(this);
				}
			}
			finally
			{
				Expressions.EndUpdate(this);
			}
		}

		protected virtual void OnRollDiceRequest(object sender, RollDiceEventArgs ea)
		{
			RollDiceRequest?.Invoke(sender, ea);
		}

		protected virtual void OnSpellDispelled(object sender, CastedSpellEventArgs ea)
		{
			SpellDispelled?.Invoke(sender, ea);
		}

		protected virtual void OnStateChanged(object sender, StateChangedEventArgs ea)
		{
			StateChanged?.Invoke(sender, ea);
		}

		void OnStateChanged(string key, object oldValue, object newValue, bool isRechargeable = false)
		{
			OnStateChanged(this, new StateChangedEventArgs(key, oldValue, newValue, isRechargeable));
		}

		public void AboutToCompleteCast()
		{
			
		}

		public void CompleteCast()
		{
			CastedSpell spellToComplete = spellToCast;
			if (spellToComplete == null && !Game.PlayerIsCastingSpell(concentratedSpell, playerID))
				spellToComplete = concentratedSpell;
			if (spellToComplete == null)
				return;

			if (Game != null)
				Game.CompleteCast(this, spellToComplete);

			spellToCast = null;
		}

		public override void ReadyRollDice(DiceRollType rollType, string diceStr, int hiddenThreshold = int.MinValue)
		{
			CompleteCast();

			if (Game != null)
				Game.SetHiddenThreshold(this, hiddenThreshold, rollType);
			diceWeAreRolling = diceStr;
		}

		void ReapplyActiveFeatures(bool forceApply = false, AssignedFeature ignored = null)
		{
			if (reapplyingActiveFeatures)
				return;
			queuedRecalcOptions = RecalcOptions.None;
			reapplyingActiveFeatures = true;
			Expressions.BeginUpdate();
			try
			{
				foreach (AssignedFeature assignedFeature in features)
				{
					if (assignedFeature == ignored)
						continue;
					if (!assignedFeature.ActivatesConditionally())
						continue;
					if (assignedFeature.ShouldActivateNow())
						assignedFeature.Activate(forceApply);
					else
						assignedFeature.Deactivate(forceApply);
				}
			}
			finally
			{
				Expressions.EndUpdate(this);
				reapplyingActiveFeatures = false;
				if (queuedRecalcOptions != RecalcOptions.None)
					Recalculate(queuedRecalcOptions);
			}
		}
		public void Recalculate(RecalcOptions recalcOptions)
		{
			//if (recalcOptions == RecalcOptions.None)
			//	return;

			if (reapplyingActiveFeatures || evaluatingExpression)
			{
				queuedRecalcOptions |= recalcOptions;
				return;
			}

			if ((recalcOptions & RecalcOptions.TurnBasedState) == RecalcOptions.TurnBasedState)
			{
				StartTurnResetState();
			}
			else if ((recalcOptions & RecalcOptions.ActionBasedState) == RecalcOptions.ActionBasedState)
			{
				ResetPlayerActionBasedState();
			}

			if ((recalcOptions & RecalcOptions.Resistance) == RecalcOptions.Resistance)
			{
				ResetPlayerResistance();
			}

			ReapplyActiveFeatures(false);
		}

		public void RemoveStateVar(string varName)
		{
			if (states.ContainsKey(varName))
				states.Remove(varName);
		}

		public void ResetPlayerActionBasedState()
		{
			attackingAbility = Ability.none;
			attackingAbilityModifier = 0;
			attackingType = AttackType.None;
			attackingKind = AttackKind.Any;
			diceWeAreRolling = string.Empty;
			targetThisRollIsCreature = false;
			damageOffsetThisRoll = 0;
			attackOffsetThisRoll = 0;
			advantageDiceThisRoll = 0;
			disadvantageDiceThisRoll = 0;
		}

		void ResetPlayerResistance()
		{
			// TODO: implement this.
			//damageResistance
		}

		// TODO: Call after a successful roll.
		public void ResetPlayerRollBasedState()
		{
			lastRollWasSuccessful = false;
			usesMagicThisRoll = false;
			targetedCreatureHitPoints = 0;
			hitWasCritical = false;
			additionalDiceThisRoll = string.Empty;
			trailingEffectsThisRoll = string.Empty;
			dieRollEffectsThisRoll = string.Empty;
			dieRollMessageThisRoll = string.Empty;
		}

		public void RollDiceNow()
		{
			OnRollDiceRequest(this, new RollDiceEventArgs(diceWeAreRolling));
		}

		public void RollIsComplete(bool hitWasCritical, List<Creature> targetedCreatures = null)
		{
			this.hitWasCritical = hitWasCritical;
			if (targetedCreatures != null)
			{
				targetedCreatureHitPoints = -1;
				foreach (var creature in targetedCreatures)
				{
					if (targetedCreatureHitPoints == -1)
						targetedCreatureHitPoints = (int)Math.Round(creature.hitPoints);

					if (creature.hitPoints <= 0)
						targetedCreatureHitPoints = 0;
				}
			}

			Expressions.BeginUpdate();
			try
			{
				foreach (AssignedFeature assignedFeature in features)
				{
					assignedFeature.RollIsComplete(this);
				}
			}
			finally
			{
				Expressions.EndUpdate(this);
			}
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
				Rechargeable rechargeable = rechargeables.FirstOrDefault(x => x.VarName == key);
				if (rechargeable != null)
				{
					int newIntValue = (int)newValue;
					if (newIntValue == rechargeable.ChargesUsed)
						return;
					int oldValue = rechargeable.ChargesUsed;
					rechargeable.ChargesUsed = newIntValue;
					OnStateChanged(key, oldValue, newValue, true);
					return;
				}

				rechargeable = rechargeables.FirstOrDefault(x => x.VarName == key + STR_RechargeableMaxSuffix);
				if (rechargeable != null)
				{
					int newIntValue = (int)newValue;
					if (newIntValue == rechargeable.TotalCharges)
						return;
					int oldValue = rechargeable.TotalCharges;
					rechargeable.TotalCharges = newIntValue;
					OnStateChanged(key + STR_RechargeableMaxSuffix, oldValue, newValue, false); // max value is not considered a rechargeable, as the max value does not change and has no corresponding UI key.
					return;
				}

				states.Add(key, newValue);
				OnStateChanged(key, null, newValue);
			}
		}
		public void SetWorldPosition(Vector worldPosition)
		{
			WorldPosition = worldPosition;
		}
		public bool SpellIsActive(string spellName)
		{
			return GetActiveSpells().FirstOrDefault(x => x.Spell.Name == spellName) != null;
		}

		public void StartAction()
		{
			ResetPlayerActionBasedState();
		}

		public void StartingExpressionEvaluation()
		{
			evaluatingExpression = true;
		}

		public void TestStartTurn()
		{
			StartTurnResetState();
			PlayerStartsTurn();
			if (Game != null)
				Game.StartingTurnFor(this);
		}

		public override void StartTurnResetState()
		{
			forceShowSpell = false;
			ActionsPerTurn = 1;
			enemyAdvantage = 0;
			_attackNum = 0;
			WeaponsInHand = 0;
			OneHanded = false;
			TwoHanded = false;
			WeaponIsHeavy = false;
			checkingAbilities = Ability.none;
			savingAgainst = Ability.none;
			checkingSkills = Skills.none;
			ResetPlayerActionBasedState();
			ResetPlayerRollBasedState();
		}

		public override void Target(Creature target)
		{
			targetedCreature = target;
			base.Target(target);
		}

		public string ToJson()
		{
			BuildSpellGroupData();
			return JsonConvert.SerializeObject(this);
		}

		public override void Use(PlayerActionShortcut playerActionShortcut)
		{
			_attackNum++;
			ResetPlayerActionBasedState();
			if (playerActionShortcut != null)
			{
				attackingAbilityModifier = playerActionShortcut.AttackingAbilityModifier;
				attackingAbility = playerActionShortcut.AttackingAbility;
				attackingType = playerActionShortcut.AttackingType;
				playerActionShortcut.ExecuteCommands(this);
			}

			ReapplyActiveFeatures();
		}

		public int SpellSlots1
		{
			get
			{
				return GetIntState("SpellSlots1");
			}
			set
			{
				SetState("SpellSlots1", value);
			}
		}

		public void AddRechargeable(string displayName, string varName, int maxValue, string cycle)
		{
			if (maxValue == 0)
				return;
			rechargeables.Add(new Rechargeable(displayName, varName, maxValue, cycle));
		}

		public void AddSpellSlots()
		{
			int[] spellSlotLevels = GetSpellSlotLevels();
			AddRechargeable("Spell Slots 1", "SpellSlots1", spellSlotLevels[1], "long rest");
			AddRechargeable("Spell Slots 2", "SpellSlots2", spellSlotLevels[2], "long rest");
			AddRechargeable("Spell Slots 3", "SpellSlots3", spellSlotLevels[3], "long rest");
			AddRechargeable("Spell Slots 4", "SpellSlots4", spellSlotLevels[4], "long rest");
			AddRechargeable("Spell Slots 5", "SpellSlots5", spellSlotLevels[5], "long rest");
			AddRechargeable("Spell Slots 6", "SpellSlots6", spellSlotLevels[6], "long rest");
			AddRechargeable("Spell Slots 7", "SpellSlots7", spellSlotLevels[7], "long rest");
			AddRechargeable("Spell Slots 8", "SpellSlots8", spellSlotLevels[8], "long rest");
			AddRechargeable("Spell Slots 9", "SpellSlots9", spellSlotLevels[9], "long rest");
		}

		public void UseSpellSlot(int spellSlotLevel)
		{
			string key = DndUtils.GetSpellSlotLevelKey(spellSlotLevel);
			SetState(key, GetIntState(key) + 1);
		}

		public void RechargeAfterShortRest()
		{
			RechargeAfterHours(2);
		}

		public void RechargeAfterLongRest()
		{
			RechargeAfterHours(8);
		}

		private void RechargeAfterHours(int hours)
		{
			List<string> keys = new List<string>();
			bool stateChanged = false;
			foreach (Rechargeable rechargeable in rechargeables)
			{
				if (rechargeable.Cycle.GetTimeSpan().TotalHours <= hours)
					if (rechargeable.ChargesUsed != 0)
					{
						keys.Add(rechargeable.VarName);
						rechargeable.ChargesUsed = 0;
						stateChanged = true;
					}
			}

			if (stateChanged)
				OnStateChanged(this, new StateChangedEventArgs(string.Join(",", keys), 1, 0, true));
		}

		public bool HasRemainingSpellSlotCharges(int spellSlotLevel)
		{
			string key = DndUtils.GetSpellSlotLevelKey(spellSlotLevel);
			return GetIntState(key) < GetIntState(key + STR_RechargeableMaxSuffix);
		}

		public void SetTimeBasedEvents()
		{
			foreach (KnownSpell knownSpell in KnownSpells)
			{
				if (knownSpell.TotalCharges > 0 && knownSpell.TotalCharges != int.MaxValue)
				{
					if (knownSpell.ResetSpan.GetTimeSpan().TotalDays == 1)
					{
						DndAlarm dndAlarm = Game.Clock.CreateDailyAlarm("Recharge:" + knownSpell.ItemName, knownSpell.RechargesAt.Hours, knownSpell.RechargesAt.Minutes, knownSpell.RechargesAt.Seconds, this);
						dndAlarm.AlarmFired += DndAlarm_RechargeItem;
						AddRechargeable(knownSpell.ItemName, DndUtils.ToVarName(knownSpell.ItemName), knownSpell.TotalCharges, "1 day");
					}
				}
			}
		}

		private void DndAlarm_RechargeItem(object sender, DndTimeEventArgs ea)
		{
			if (ea.Alarm.Name.StartsWith("Recharge:"))
			{
				string itemName = ea.Alarm.Name.EverythingAfter("Recharge:");
				KnownSpell item = KnownSpells.FirstOrDefault(x => x.ItemName == itemName);
				if (item != null)
				{
					item.ChargesRemaining = item.TotalCharges;
				}
				OnRequestMessageToDungeonMaster($"{firstName}'s {item.ItemName} recharged ({item.TotalCharges}).");
			}
		}

		public string firstName
		{
			get
			{
				return DndUtils.GetFirstName(name);
			}
		}
		public string heShe { get; set; }
		public string hisHer { get; set; }
		public string himHer { get; set; }
		public string HisHer { get; set; }
		public string HeShe { get; set; }

		[JsonIgnore]
		public bool lastRollWasSuccessful { get; set; }

		public void SetRemainingChargesOnItem(string itemName, int value)
		{
			string varName = DndUtils.ToVarName(itemName);
			Rechargeable rechargeable = rechargeables.FirstOrDefault(x => x.VarName == varName);
			int oldChargesRemaining = rechargeable.ChargesRemaining;
			if (rechargeable != null && oldChargesRemaining != value)
			{
				int oldChargesUsed = rechargeable.ChargesUsed;
				rechargeable.SetRemainingCharges(value);
				OnStateChanged(this, new StateChangedEventArgs(rechargeable.VarName, oldChargesUsed, rechargeable.ChargesUsed, true));
			}
		}

		public int GetRemainingChargesOnItem(string itemName)
		{
			string varName = DndUtils.ToVarName(itemName);
			Rechargeable rechargeable = rechargeables.FirstOrDefault(x => x.VarName == varName);
			if (rechargeable != null)
				return rechargeable.ChargesRemaining;
			return 0;
		}

		protected virtual void OnRequestMessageToDungeonMaster(string message)
		{
			RequestMessageToDungeonMaster?.Invoke(this, new MessageEventArgs(message));
		}
		public int GetSpellcastingLevel()
		{
			return level;
			//foreach (CharacterClass characterClass in Classes)
			//{
			//	switch (characterClass.Class)
			//	{
			//		case Class.None:
			//			return 0;
			//		case Class.Artificer:
			//		case Class.Bard:
			//		case Class.BloodHunter:
			//		case Class.Druid:
			//		case Class.Cleric:
			//			return characterClass.Level;

			//		case Class.Paladin:
			//		case Class.Ranger:
			//			return characterClass.Level / 2;

			//		case Class.Barbarian:
			//			return 0;
			//		case Class.Fighter:

			//			break;
			//		case Class.Monk:

			//			// TODO: Check this logic against D&D rules:
			//			if (characterClass.SubClass == SubClass.WayOfTheFourElements)
			//				return characterClass.Level / 2;
			//			else
			//				return characterClass.Level;

			//		case Class.Rogue:
			//			if (characterClass.SubClass == SubClass.ArcaneTrickster)
			//			break;
			//		case Class.Sorcerer:

			//			break;
			//		case Class.Warlock:

			//			break;
			//		case Class.Wizard:
			//			
			//			break;
			//	}
			//}
			// return 0;
		}

		public void StartGame()
		{
			Expressions.BeginUpdate();
			try
			{
				foreach (AssignedFeature assignedFeature in features)
					assignedFeature.StartGame(this);
			}
			finally
			{
				Expressions.EndUpdate(null); // We are about to activate always-on features. No need to do this twice.
			}

			ActivateAlwaysOnFeatures();
			ActivateConditionallySatisfiedFeatures();
		}
		void BuildSpellGroupData()
		{
			SortedDictionary<int, SpellGroup> spellGroupsByLevel = new SortedDictionary<int, SpellGroup>();
			int[] spellSlotLevels = GetSpellSlotLevels();

			string GetSpellGroupName(int level)
			{
				switch (level)
				{
					case 0:
						return "Cantrips: ";
					case 1:
						return "1st: ";
					case 2:
						return "2nd: ";
					case 3:
						return "3rd: ";
				}
				return $"{level}th: ";
			}
			SpellGroup GetSpellGroupByLevel(int level)
			{
				if (!spellGroupsByLevel.ContainsKey(level))
				{
					
					SpellGroup newSpellGroup = new SpellGroup();
					newSpellGroup.Name = GetSpellGroupName(level);
					if (level > 0)
					{
						newSpellGroup.TotalCharges = spellSlotLevels[level];
						newSpellGroup.ChargesUsed = newSpellGroup.TotalCharges - GetRemainingChargesOnItem(DndUtils.GetSpellSlotLevelKey(level));
					}

					spellGroupsByLevel.Add(level, newSpellGroup);

				}

				return spellGroupsByLevel[level];
			}

			foreach (KnownSpell knownSpell in KnownSpells)
			{
				Spell spell = AllSpells.Get(knownSpell.SpellName);

				if (spell != null)
				{
					SpellGroup spellGroup = GetSpellGroupByLevel(spell.Level);
					Spell matchingSpell = AllSpells.Get(knownSpell.SpellName);
					bool requiresConcentration = false;
					bool morePowerfulAtHigherLevels = false;
					if (matchingSpell != null)
					{
						requiresConcentration = matchingSpell.RequiresConcentration;
						morePowerfulAtHigherLevels = matchingSpell.MorePowerfulWhenCastAtHigherLevels;
					}

					bool isConcentratingNow = concentratedSpell != null && concentratedSpell.Spell.Name == knownSpell.SpellName;
					
					bool powerComesFromItem = knownSpell.CanBeRecharged();

					spellGroup.SpellDataItems.Add(new SpellDataItem(knownSpell.SpellName, requiresConcentration, morePowerfulAtHigherLevels, isConcentratingNow, powerComesFromItem));
				}

			}

			for (int i = 1; i <= HighestSpellSlot; i++)
			{
				GetSpellGroupByLevel(i); // Ensures we have groups even for spell levels that have no spells (e.g., if a character only has first and third-level spells, this ensures we'll generate a group of second-level spell slots).
			}

			SpellData = new List<SpellGroup>();

			foreach (int key in spellGroupsByLevel.Keys)
			{
				SpellData.Add(spellGroupsByLevel[key]);
			}
		}

		public void ActivateConditionallySatisfiedFeatures()
		{
			Expressions.BeginUpdate();
			try
			{
				foreach (AssignedFeature assignedFeature in features)
				{
					assignedFeature.ActivateIfConditionallySatisfied();
				}
			}
			finally
			{
				Expressions.EndUpdate(this);
			}
		}

		public void ActivateAlwaysOnFeatures()
		{
			Expressions.BeginUpdate();
			try
			{
				foreach (AssignedFeature assignedFeature in features)
				{
					assignedFeature.ActivateIfAlwaysOn();
				}
			}
			finally
			{
				Expressions.EndUpdate(this);
			}
		}
		string GetValueStr(object value)
		{
			if (value == null)
				return "null";
			return value.ToString();
		}

		public List<string> GetStateReport()
		{
			List<string> report = new List<string>();
			foreach (KeyValuePair<string, object> pair in states)
			{
				report.Add($"{pair.Key} = {GetValueStr(pair.Value)}");
			}

			foreach (Rechargeable rechargeable in rechargeables)
			{
				report.Add($"{rechargeable.DisplayName} ({rechargeable.VarName}): {rechargeable.ChargesUsed}/{rechargeable.TotalCharges}");
			}

			return report;
		}
		public void TestEvaluateAllExpressions()
		{
			foreach (AssignedFeature assignedFeature in features)
			{
				assignedFeature.TestEvaluateAllExpressions(this);
			}

			foreach (KnownSpell knownSpell in KnownSpells)
			{
				knownSpell.TestEvaluateAllExpressions(this);
			}
		}
		public void PrepareForSerialization()
		{
			BuildSpellGroupData();
		}

		StateChangedEventArgs stateChangedEventArgs;

		void SetField(string fieldName, ref double field, double newValue, bool isRechargeable = false)
		{
			if (field == newValue)
				return;
			stateChangedEventArgs.Add(fieldName, field, newValue, isRechargeable);
			field = newValue;
		}
		void SetField(string fieldName, ref int field, int newValue, bool isRechargeable = false)
		{
			if (field == newValue)
				return;
			stateChangedEventArgs.Add(fieldName, field, newValue, isRechargeable);
			field = newValue;
		}
		void SetField(string fieldName, ref bool field, bool newValue, bool isRechargeable = false)
		{
			if (field == newValue)
				return;
			stateChangedEventArgs.Add(fieldName, field, newValue, isRechargeable);
			field = newValue;
		}
		void SetField(string fieldName, ref string field, string newValue, bool isRechargeable = false)
		{
			if (field == newValue)
				return;
			stateChangedEventArgs.Add(fieldName, field, newValue, isRechargeable);
			field = newValue;
		}
		public void CopyUIChangeableAttributesFrom(Character character)
		{
			
			stateChangedEventArgs = new StateChangedEventArgs();
			SetField("baseArmorClass",   ref baseArmorClass,                   character.baseArmorClass);
			SetField("baseCharisma",     ref baseCharisma,                   character.baseCharisma);
			SetField("baseConstitution", ref baseConstitution,                   character.baseConstitution);
			SetField("baseDexterity",    ref baseDexterity,                   character.baseDexterity);
			SetField("baseIntelligence", ref baseIntelligence,                   character.baseIntelligence);
			SetField("baseStrength",     ref baseStrength,                   character.baseStrength);
			SetField("alignment",        ref alignment,                   character.alignment);
			SetField("deathSaveDeath1",  ref deathSaveDeath1,                   character.deathSaveDeath1);
			SetField("deathSaveDeath2",  ref deathSaveDeath2,                   character.deathSaveDeath2);
			SetField("deathSaveDeath3",  ref deathSaveDeath3,                   character.deathSaveDeath3);
			SetField("deathSaveLife1",   ref deathSaveLife1,                   character.deathSaveLife1);
			SetField("deathSaveLife2",   ref deathSaveLife2,                   character.deathSaveLife2);
			SetField("deathSaveLife3",   ref deathSaveLife3,                   character.deathSaveLife3);
			SetField("experiencePoints", ref experiencePoints,                   character.experiencePoints);
			SetField("goldPieces",       ref goldPieces,                   character.goldPieces);
			SetField("hitPoints",        ref hitPoints,                   character.hitPoints);
			SetField("initiative",       ref initiative,                   character.initiative);
			SetField("inspiration",      ref inspiration,                   character.inspiration);
			SetField("load",             ref load,                   character.load);
			SetField("name",             ref name,                   character.name);
			SetField("proficiencyBonus", ref proficiencyBonus,                   character.proficiencyBonus);
			SetField("baseWalkingSpeed", ref baseWalkingSpeed,                   character.baseWalkingSpeed);
			SetField("tempHitPoints",    ref tempHitPoints,                   character.tempHitPoints);
			SetField("totalHitDice",     ref totalHitDice,                   character.totalHitDice);
			SetField("weight",					 ref weight,                   character.weight);

			if (stateChangedEventArgs.ChangeList.Count > 0)
			{
				OnStateChanged(this, stateChangedEventArgs);
				stateChangedEventArgs = null;
			}
		}

		public void RollHasStopped()
		{
			if (spellActivelyCasting != null)
			{
				spellPreviouslyCasting = spellActivelyCasting; // So we can show the spell for a longer time before we hide it.
				spellActivelyCasting = null;
				OnStateChanged(this, new StateChangedEventArgs("spellActivelyCasting", null, null));
			}
		}

		public void ClearPreviouslyCastingSpell()
		{
			if (spellPreviouslyCasting == null)
				return;
			spellPreviouslyCasting = null;
			OnStateChanged(this, new StateChangedEventArgs("spellPreviouslyCasting", null, null));
		}

		private static EventCategory CreateFeatureEvents(Character player)
		{
			Type type = typeof(Feature);
			EventCategory eventCategory = new EventCategory("Features", type);
			EventType eventType = EventType.FeatureEvents;

			foreach (AssignedFeature assignedFeature in player.features)
			{
				eventCategory.Add(new EventGroup(assignedFeature.Feature, assignedFeature.Feature.Name, eventType, DndEventManager.knownEvents[type]));
			}

			return eventCategory;
		}

		private static EventCategory CreateSpellEvents(Character player)
		{
			Type type = typeof(Spell);
			EventCategory eventCategory = new EventCategory("Spells", type);
			EventType eventType = EventType.SpellEvents;

			foreach (KnownSpell knownSpell in player.KnownSpells)
			{
				Spell spell = AllSpells.Get(knownSpell.SpellName);
				eventCategory.Add(new EventGroup(spell, knownSpell.SpellName, eventType, DndEventManager.knownEvents[type]));
			}

			return eventCategory;
		}

		public void RebuildAllEvents()
		{
			eventCategories.Clear();
			AddEvents(CreateFeatureEvents(this));
			AddEvents(CreateSpellEvents(this));
		}

		[JsonIgnore]
		public List<EventCategory> eventCategories = new List<EventCategory>();
		public void AddEvents(EventCategory eventCategory)
		{
			eventCategories.Add(eventCategory);
		}

		public IEnumerable GetEventGroup(Type type)
		{
			foreach (EventCategory eventCategory in eventCategories)
			{
				if (eventCategory.Type == type)
					return eventCategory.Groups;
			}
			return null;
		}
		public KnownSpell GetMatchingSpell(string spellName)
		{
			return KnownSpells.FirstOrDefault(x => x.SpellName == spellName);
		}

		public event CastedSpellEventHandler SpellDispelled;
		public event RollDiceEventHandler RollDiceRequest;
		public event StateChangedEventHandler StateChanged;
		public event MessageEventHandler RequestMessageToDungeonMaster;
	}
}