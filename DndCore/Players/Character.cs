using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Reflection;
using GoogleHelper;

namespace DndCore
{
	[SheetName("DnD")]
	[TabName("Players")]
	public class Character : Creature
	{
		public override int IntId { get => playerID; }

		[JsonIgnore]
		List<CarriedItem> CarriedItems = new List<CarriedItem>();


		[Column]
		public string sourceName { get; set; }

		[Column]
		public string resistancesVulnerabilitiesImmunitiesStr { get; set; }

		// HACK: Don't be mad at this name property storing to the ancestor's field of the exact same name - we are doing this wrapper to support both serialization to Google Sheets as well as JSON to SignalR serialization to legacy TypeScript code.
		[Indexer]
		public new string name { get => base.name; set => base.name = value; }

		public int Index
		{
			get
			{
				List<Character> activePlayers = AllPlayers.GetActive();
				for (int i = 0; i < activePlayers.Count; i++)
				{
					if (activePlayers[i] == this)
						return i;
				}
				return -1;
			}
		}

		//string wildShapeCreatureName;
		Creature wildShape;
		[JsonIgnore]
		public Creature WildShape
		{
			get
			{
				return wildShape;
			}
			set
			{
				if (wildShape == value)
					return;
				//if (value == null)
				//	OnOriginalShapeRestoring();
				//else
				//	OnShapeChanging();
				wildShape = value;
			}
		}

		public bool Hidden { get; set; }

		[JsonIgnore]
		public string WildShapeCreatureKind
		{
			get
			{
				if (WildShape is Monster monster)
					return monster.Kind;
				return "";
			}
		}

		[JsonIgnore]
		public string ActiveWeaponName
		{
			get
			{
				if (ReadiedWeapon != null)
					if (!string.IsNullOrEmpty(ReadiedWeapon.Name))
						return ReadiedWeapon.Name;
					else
						return ReadiedWeapon.Weapon.Name;  // Weapon kind, like "Bow", "Dagger", etc.

				return string.Empty;
			}
		}


		CarriedWeapon readiedWeapon;
		[JsonIgnore]
		public CarriedWeapon ReadiedWeapon
		{
			get
			{
				return readiedWeapon;
			}
			set
			{
				if (readiedWeapon == value)
					return;

				readiedWeapon = value;
				if (readiedWeapon == null)
					ReadiedAmmunition = null;
				else
				{
					string ammunitionKind = readiedWeapon.Weapon.AmmunitionKind;
					if (!HasAmmunition(ammunitionKind))
						ReadiedAmmunition = null;
					else
					{
						List<CarriedAmmunition> allMatchingAmmunition = GetAllAmmunition(ammunitionKind);
						if (allMatchingAmmunition.Count > 1)
						{
							ReadiedAmmunition = ChooseAmmunition(ammunitionKind);
						}
						else
						{
							ReadiedAmmunition = allMatchingAmmunition[0];
						}
						if (ReadiedAmmunition != null)
						{
							usesMagicAmmunitionThisRoll = !string.IsNullOrEmpty(ReadiedAmmunition.DamageBonusStr);
						}
					}

				}
			}
		}



		[JsonIgnore]
		public CarriedAmmunition ReadiedAmmunition;

		[JsonIgnore]
		public string ReadiedAmmunitionEffectName
		{
			get
			{
				if (ReadiedAmmunition == null)
					return string.Empty;
				return ReadiedAmmunition.EffectName.Trim().Trim('"');
			}
		}
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
		public Ability checkingAbilities = Ability.none;

		[JsonIgnore]
		public Skills checkingSkills = Skills.none;

		[JsonIgnore]
		public List<AssignedFeature> features { get; set; } = new List<AssignedFeature>();

		[JsonIgnore]
		public bool IsActive { get; set; }

		public ActiveSpellData ActiveSpell
		{
			get
			{
				if (!IsActive)
					return null;
				if (spellPrepared != null)
					return spellPrepared;
				if (spellActivelyCasting != null)
					return spellActivelyCasting;
				return spellPreviouslyCasting;
			}
		}
		public bool deathSaveDeath1 = false;
		public bool deathSaveDeath2 = false;
		public bool deathSaveDeath3 = false;
		public bool deathSaveLife1 = false;
		public bool deathSaveLife2 = false;
		public bool deathSaveLife3 = false;

		[JsonIgnore]
		public string bubbleTextColor = "#000000";
		public Skills doubleProficiency = 0;
		int enemyAdvantage;
		public int ActionsPerTurn;
		public int experiencePoints = 0;
		public int headshotIndex;

		[JsonIgnore]
		public int _attackNum = 0;
		public string _inspiration = string.Empty;
		
		public string inspiration
		{
			get
			{
				if (string.IsNullOrEmpty(_inspiration))
					return "-";
				return _inspiration;
			}
			set => _inspiration = value;
		}
		
		public double load = 0;
		public double proficiencyBonus = 0;
		public Skills proficientSkills = 0;
		public Skills halfProficiency = 0;

		public string remainingHitDice = string.Empty;

		public VantageKind rollInitiative = VantageKind.Normal;

		public Ability savingAgainst = Ability.none;
		public Ability savingThrowProficiency = 0;

		public override int GetSpellcastingAbilityModifier()
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

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public string SpellCastingAbilityStr
		{
			get
			{
				return DndUtils.ToAbilityDisplayString(spellCastingAbility);
			}
		}

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
		public string SpellCastingStr
		{
			get
			{
				return DndUtils.ToSpellcastingAbilityDisplayString(spellCastingAbility, SpellcastingAbilityModifier);
			}
		}

		[JsonIgnore]
		public Creature targetedCreature;
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
		public double weight = 0;

		public List<CharacterClass> Classes { get; set; } = new List<CharacterClass>();

		[JsonIgnore]
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

		public string ClassStr
		{
			get
			{
				string result = string.Empty;
				foreach (CharacterClass characterClass in Classes)
				{
					if (result.Length > 0)
						result += " / ";
					result += characterClass.Name;
				}
				return result;
			}
		}

		public double wisdomMod
		{
			get
			{
				int mod = getModFromAbility(Wisdom);
				if (WildShape is Monster monster)
					return Math.Max(mod, monster.wisdomMod);

				return mod;
			}
		}


		public double intelligenceMod
		{
			get
			{
				int mod = getModFromAbility(Intelligence);
				if (WildShape is Monster monster)
					return Math.Max(mod, monster.intelligenceMod);

				return mod;
			}
		}
		public double constitutionMod
		{
			get
			{
				int mod = getModFromAbility(Constitution);
				if (WildShape is Monster monster)
					return Math.Max(mod, monster.constitutionMod);

				return mod;
			}
		}
		public double dexterityMod
		{
			get
			{
				int mod = getModFromAbility(Dexterity);
				if (WildShape is Monster monster)
					return Math.Max(mod, monster.dexterityMod);

				return mod;
			}
		}
		public double strengthMod
		{
			get
			{
				int mod = getModFromAbility(Strength);
				if (WildShape is Monster monster)
					return Math.Max(mod, monster.strengthMod);

				return mod;
			}
		}

		public double charismaMod
		{
			get
			{
				int mod = getModFromAbility(Charisma);
				if (WildShape is Monster monster)
					return Math.Max(mod, monster.charismaMod);

				return mod;
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

		[JsonIgnore]
		public int leftMostPriority { get; set; }

		
		public override int Level
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
				return _passivePerception = 10 + wisdomMod + getProficiencyBonusForSkill(Skills.perception);
			}
		}

		public bool playingNow { get; set; }

		public string raceClass
		{
			get
			{
				//return $"{race} {ClassLevelStr}";
				return $"{race} {ClassStr}";
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

		public override double GetSavingThrowModifier(Ability savingThrowAbility)
		{
			switch (savingThrowAbility)
			{
				case Ability.strength:
					return savingThrowModStrength;
				case Ability.dexterity:
					return savingThrowModDexterity;
				case Ability.constitution:
					return savingThrowModConstitution;
				case Ability.intelligence:
					return savingThrowModIntelligence;
				case Ability.wisdom:
					return savingThrowModWisdom;
				case Ability.charisma:
					return savingThrowModCharisma;
			}
			return 0;
		}
		public int ShieldBonus { get; set; }


		bool HasOverridingSkillMod(Skills skill, out int value)
		{
			value = 0;
			if (WildShape is Monster monster)
				if (monster.HasOverridingSkillMod(skill))
				{
					value = monster.GetOverridingSkillMod(skill);
					return true;
				}
			return false;
		}

		public double skillModAcrobatics
		{
			get
			{
				if (HasOverridingSkillMod(Skills.acrobatics, out int mod))
					return mod;
				return getProficiencyBonusForSkill(Skills.acrobatics) + dexterityMod + this.tempAcrobaticsMod;
			}
		}

		public double skillModAnimalHandling
		{
			get
			{
				if (HasOverridingSkillMod(Skills.animalHandling, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.animalHandling) + this.wisdomMod + this.tempAnimalHandlingMod;
			}
		}

		public double skillModArcana
		{
			get
			{
				if (HasOverridingSkillMod(Skills.arcana, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.arcana) + this.intelligenceMod + this.tempArcanaMod;
			}
		}

		public double skillModAthletics
		{
			get
			{
				if (HasOverridingSkillMod(Skills.athletics, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.athletics) + this.strengthMod + this.tempAthleticsMod;
			}
		}

		public double skillModDeception
		{
			get
			{
				if (HasOverridingSkillMod(Skills.deception, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.deception) + this.charismaMod + this.tempDeceptionMod;
			}
		}

		public double skillModHistory
		{
			get
			{
				if (HasOverridingSkillMod(Skills.history, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.history) + this.intelligenceMod + this.tempHistoryMod;
			}
		}

		public double skillModInsight
		{
			get
			{
				if (HasOverridingSkillMod(Skills.insight, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.insight) + this.wisdomMod + this.tempInsightMod;
			}
		}

		public double skillModIntimidation
		{
			get
			{
				if (HasOverridingSkillMod(Skills.intimidation, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.intimidation) + this.charismaMod + this.tempIntimidationMod;
			}
		}

		public double skillModInvestigation
		{
			get
			{
				if (HasOverridingSkillMod(Skills.investigation, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.investigation) + this.intelligenceMod + this.tempInvestigationMod;
			}
		}

		public double skillModMedicine
		{
			get
			{
				if (HasOverridingSkillMod(Skills.medicine, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.medicine) + this.wisdomMod + this.tempMedicineMod;
			}
		}


		public double skillModNature
		{
			get
			{
				if (HasOverridingSkillMod(Skills.nature, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.nature) + this.intelligenceMod + this.tempNatureMod;
			}
		}


		public double skillModPerception
		{
			get
			{
				if (HasOverridingSkillMod(Skills.perception, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.perception) + this.wisdomMod + this.tempPerceptionMod;
			}
		}


		public double skillModPerformance
		{
			get
			{
				if (HasOverridingSkillMod(Skills.performance, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.performance) + this.charismaMod + this.tempPerformanceMod;
			}
		}

		public double skillModPersuasion
		{
			get
			{
				if (HasOverridingSkillMod(Skills.persuasion, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.persuasion) + this.charismaMod + this.tempPersuasionMod;
			}
		}


		public double skillModReligion
		{
			get
			{
				if (HasOverridingSkillMod(Skills.religion, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.religion) + this.intelligenceMod + this.tempReligionMod;
			}
		}


		public double skillModSleightOfHand
		{
			get
			{
				if (HasOverridingSkillMod(Skills.sleightOfHand, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.sleightOfHand) + this.dexterityMod + this.tempSleightOfHandMod;
			}
		}

		public double skillModStealth
		{
			get
			{
				if (HasOverridingSkillMod(Skills.stealth, out int mod))
					return mod;
				return this.getProficiencyBonusForSkill(Skills.stealth) + this.dexterityMod + this.tempStealthMod;
			}
		}

		public double skillModSurvival
		{
			get
			{
				if (HasOverridingSkillMod(Skills.survival, out int mod))
					return mod;
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

		[Ask("Weapon is Two-handed")]
		public bool TwoHanded { get; set; }  // TODO: Implement this + test cases.

		[Ask("Weapon is Heavy")]
		public bool WeaponIsHeavy { get; set; }  // TODO: test cases.

		[Ask("Weapon is Ranged")]
		public bool WeaponIsRanged { get; set; } // TODO: test cases.

		[Ask("Weapon is Finesse")]
		public bool WeaponIsFinesse { get; set; } // TODO: test cases.

		public bool HasWeaponInHand { get; set; } // TODO: test cases.
		public Weapons weaponProficiency { get; set; }
		public int WeaponsInHand { get; set; }  // TODO: Implement this (0, 1 or 2) + test cases.

		public static Character From(CharacterDto characterDto)
		{
			Character character = new Character();
			//character.playerID = AllPlayers.GetPlayerIdFromName(characterDto.name);
			character.features = GetFeatures(characterDto.features, character);
			character.name = characterDto.name;
			character.playerShortcut = characterDto.playerShortcut;
			character.playingNow = !string.IsNullOrWhiteSpace(characterDto.playingNow);
			character.Hidden = !string.IsNullOrWhiteSpace(characterDto.hidden);
			character.race = characterDto.race;
			character.heShe = characterDto.heShe;
			character.hisHer = characterDto.hisHer;
			character.himHer = characterDto.himHer;
			character.HeShe = characterDto.heShe.InitialCap();
			character.HisHer = characterDto.hisHer.InitialCap();
			character.resistancesVulnerabilitiesImmunitiesStr = characterDto.resistancesVulnerabilitiesImmunitiesStr;
			character.sourceName = characterDto.sourceName;
			character.sceneName = characterDto.sceneName;
			character.taleSpireId = characterDto.taleSpireId;
			character.videoAnchorHorizontal = characterDto.videoAnchorHorizontal;
			character.videoAnchorVertical = characterDto.videoAnchorVertical;
			character.videoHeight = characterDto.videoHeight;
			character.videoWidth = characterDto.videoWidth;

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

			character.HitPoints = characterDto.hitPoints;
			character.playerID = characterDto.playerId;
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
			character.halfProficiency = DndUtils.ToSkill(characterDto.halfProficiency);
			character.doubleProficiency = DndUtils.ToSkill(characterDto.doubleProficiency);
			character.savingThrowProficiency = DndUtils.ToAbility(characterDto.savingThrowProficiency);
			character.spellCastingAbility = DndUtils.ToAbility(characterDto.spellCastingAbility);
			character.initiative = characterDto.initiative;
			character.rollInitiative = DndUtils.ToVantage(characterDto.rollInitiative);
			character.hueShift = characterDto.hueShift;
			character.dieBackColor = characterDto.dieBackColor;
			character.dieFontColor = characterDto.dieFontColor;
			character.bubbleTextColor = characterDto.bubbleTextColor;
			character.headshotIndex = characterDto.headshotIndex;
			character.alignmentStr = characterDto.alignment;
			character.AddWeaponsFrom(characterDto.weapons);
			character.AddItemsFrom(characterDto.items);
			character.AddAmmunitionFrom(characterDto.ammunition);
			character.AddSpellsFrom(characterDto.spells);
			character.weaponProficiency = DndUtils.ToWeapon(characterDto.weaponProficiency);
			character.leftMostPriority = characterDto.leftMostPriority;
			return character;
		}

		static List<AssignedFeature> GetFeatures(string featureListStr, Character player)
		{
			List<AssignedFeature> results = new List<AssignedFeature>();
			if (featureListStr.IndexOf(',') >= 0)  // Features need to be semicolon-delimited
			{
				System.Diagnostics.Debugger.Break();
			}
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

			string ammoParameters = "";

			if (ammunitionName.Has("("))
			{
				if (ammunitionName.Has(")"))
				{
					ammoParameters = ammunitionName.EverythingAfter("(").TrimEnd(')');
					ammunitionName = ammunitionName.EverythingBefore("(");
				}
			}

			CarriedAmmunition carriedAmmunition = new CarriedAmmunition();
			carriedAmmunition.Kind = ammunitionName;
			carriedAmmunition.SetAmmunitionParameters(ammoParameters, this);
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

		public void RemoveSpell(string name)
		{
			KnownSpell foundSpell = KnownSpells.FirstOrDefault(x => x.SpellName == name);
			if (foundSpell == null)
				return;
			KnownSpells.Remove(foundSpell);
		}

		public CarriedWeapon AddWeapon(string weaponStr)
		{
			if (string.IsNullOrWhiteSpace(weaponStr))
				return null;

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
							int plusPos = parameter.IndexOf("+");
							if (plusPos > 0)
							{
								carriedWeapon.DamageDieBonus = parameter.Substring(0, plusPos);
								parameter = parameter.Substring(plusPos);
							}
							carriedWeapon.HitPlusModifier = MathUtils.GetInt(parameter);
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
			if (weapon != null)
				CarriedWeapons.Add(carriedWeapon);
			return carriedWeapon;
		}

		public CarriedItem AddItem(string itemStr)
		{
			if (string.IsNullOrWhiteSpace(itemStr))
				return null;

			CarriedItem carriedItem = new CarriedItem();

			DndItem item = AllDndItems.Get(itemStr);
			carriedItem.Item = item;
			if (item != null)
				CarriedItems.Add(carriedItem);
			return carriedItem;
		}

		void AddWeaponsFrom(string weaponsStr)
		{
			if (string.IsNullOrWhiteSpace(weaponsStr))
				return;
			string[] weaponStrs = weaponsStr.Split(';');
			foreach (string weapon in weaponStrs)
				AddWeapon(weapon.Trim());
		}
		void AddItemsFrom(string itemsStr)
		{
			if (string.IsNullOrWhiteSpace(itemsStr))
				return;
			string[] itemsStrs = itemsStr.Split(';');
			foreach (string item in itemsStrs)
				AddItem(item.Trim());
		}

		public void ApplyModPermanently(Mod mod, string description)
		{
			// TODO: Implement this!
		}

		public void ApplyModTemporarily(Mod mod, string description)
		{
			// TODO: Implement this!
		}

		public CastedSpell CastTest(Spell spell, Creature targetCreature = null)
		{
			spellToCast = null;

			if (Game != null)
				spellToCast = Game.Cast(this, spell, targetCreature);

			return spellToCast;
		}

		public void ClearAllCasting()
		{
			if (spellPreviouslyCasting == null && spellActivelyCasting == null && spellPrepared == null)
				return;
			spellPreviouslyCasting = null;
			spellActivelyCasting = null;
			spellPrepared = null;
			OnStateChanged(this, new StateChangedEventArgs("spellActivelyCasting", null, null));
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

		public void DeactivateFeature(string featureNameStr)
		{
			AssignedFeature foundFeature = features.FirstOrDefault(x => DndUtils.GetCleanItemName(x.Feature.Name.ToLower()) == DndUtils.GetCleanItemName(featureNameStr).ToLower());
			if (foundFeature == null)
				return;

			foundFeature.Deactivate();
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
			if (Game == null)
				return null;
			return Game.GetActiveSpells(this);
		}

		public int GetAmmunitionCount(string kind)
		{
			CarriedAmmunition ammo = GetAmmunition(kind);
			if (ammo == null)
				return 0;
			return ammo.Count;
		}

		public int GetAttackingAbilityModifier(WeaponProperties weaponProperties, AttackType attackType)
		{
			CheckForAttackingAbilityOverride();

			if (OverrideAttackingAbility != Ability.none)
				attackingAbility = OverrideAttackingAbility;
			else
				GetAttackingAbility(weaponProperties, attackType);

			attackingAbilityModifier = GetAbilityModifier(attackingAbility);

			return (int)Math.Round(attackingAbilityModifier + attackingAbilityModifierBonusThisRoll);
		}

		private void GetAttackingAbility(WeaponProperties weaponProperties, AttackType attackType)
		{
			double dexterityModifier = GetAbilityModifier(Ability.dexterity);
			double strengthModifier = GetAbilityModifier(Ability.strength);

			attackingType = attackType;

			if ((weaponProperties & WeaponProperties.Finesse) == WeaponProperties.Finesse)
			{
				if (dexterityModifier > strengthModifier)
					attackingAbility = Ability.dexterity;
				else
					attackingAbility = Ability.strength;

			}
			else if (attackType == AttackType.Range)
			{
				attackingAbility = Ability.dexterity;
			}
			else
			{
				attackingAbility = Ability.strength;
			}
		}

		private void CheckForAttackingAbilityOverride()
		{
			OverrideAttackingAbility = Ability.none;
			OverrideWeaponMagicHue = int.MinValue;
			List<CastedSpell> activeSpells = GetActiveSpells();
			if (activeSpells != null)
				foreach (CastedSpell castedSpell in activeSpells)
				{
					castedSpell.Spell.TriggerGetAttackAbility(this, null, castedSpell);
				}
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
			if (hasHalfProficiencyBonusForSkill(skill))
				return 0.5;
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
			//if (name == "Bard")
			//{
			//	results[1] += 2;
			//}
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

		// TODO: Implement TargetIsFlanked when map is built and integrated into the game.
		[Ask("Target is flanked")]
		public bool TargetIsFlanked { get; set; }

		// TODO: Implement TargetIsMarked when map is built and integrated into the game as well as targets.
		[Ask("Target is Marked")]
		public bool TargetIsMarked { get; set; }


		// TODO: Incorporate VantageThisRoll into die rolls.
		public VantageKind VantageThisRoll
		{
			get
			{
				if (advantageDiceThisRoll > 0 && disadvantageDiceThisRoll > 0)
					return VantageKind.Normal;

				if (advantageDiceThisRoll > 0)
					return VantageKind.Advantage;

				if (disadvantageDiceThisRoll > 0)
					return VantageKind.Disadvantage;

				return VantageKind.Normal;
			}
		}

		bool hasDoubleProficiencyBonusForSkill(Skills skill)
		{
			return (doubleProficiency & skill) == skill;
		}

		bool hasHalfProficiencyBonusForSkill(Skills skill)
		{
			return (halfProficiency & skill) == skill;
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
			return hasDoubleProficiencyBonusForSkill(skill) || (proficientSkills & skill) == skill || (halfProficiency & skill) == skill;
		}

		bool hasSavingThrowProficiency(Ability ability)
		{
			return (savingThrowProficiency & ability) == ability;
		}

		public override void ChangeTempHP(double deltaTempHp)
		{
			double saveTempHitPoints = tempHitPoints;
			base.ChangeTempHP(deltaTempHp);
			if (tempHitPoints != saveTempHitPoints)
				OnStateChanged(this, new StateChangedEventArgs("tempHitPoints", saveTempHitPoints, tempHitPoints));
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
			CastedSpell castedSpell = new CastedSpell(spellJustCast, this);
			JustCastSpell(castedSpell);
		}

		public void JustCastSpell(CastedSpell castedSpell)
		{
			Expressions.BeginUpdate();
			try
			{
				foreach (AssignedFeature assignedFeature in features)
					assignedFeature.SpellJustCast(this, castedSpell);
			}
			finally
			{
				Expressions.EndUpdate(this);
			}
		}

		public void AfterPlayerSwingsWeapon()
		{
			Expressions.BeginUpdate();
			try
			{
				foreach (AssignedFeature assignedFeature in features)
				{
					assignedFeature.AfterPlayerSwings(this);
				}
			}
			finally
			{
				Expressions.EndUpdate(this);
			}
		}

		public void BeforePlayerRollsDice(DiceRoll diceRoll, ref VantageKind vantageKind)
		{
			vantageKind = GetVantage(diceRoll.Type, diceRoll.SavingThrow, diceRoll.SkillCheck, vantageKind);
			Expressions.BeginUpdate();
			try
			{
				foreach (AssignedFeature assignedFeature in features)
				{
					assignedFeature.BeforePlayerRolls(this);
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
				Game.CheckAlarmsPlayerStartsTurn(this);
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

		public void PlayerEndsTurn()
		{
			Expressions.BeginUpdate();
			try
			{
				Game.CheckAlarmsPlayerEndsTurn(this);
				//foreach (AssignedFeature assignedFeature in features)
				//{
				//	assignedFeature.PlayerEndsTurn(this);
				//}
			}
			finally
			{
				Expressions.EndUpdate(this);
			}
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
					{
						assignedFeature.Activate(forceApply);
					}
					else
					{
						assignedFeature.Deactivate(forceApply);
					}
				}
			}
			finally
			{
				Expressions.EndUpdate(this);
				reapplyingActiveFeatures = false;
				if (queuedRecalcOptions != RecalcOptions.None)
				{
					Recalculate(queuedRecalcOptions);
				}
			}
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
						targetedCreatureHitPoints = (int)Math.Round(creature.HitPoints);

					if (creature.HitPoints <= 0)
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

		public bool SpellIsActive(string spellName)
		{
			List<CastedSpell> spells = GetActiveSpells();
			if (spells == null)
				return false;
			return spells.FirstOrDefault(x => x.Spell.Name == spellName) != null;
		}

		public void StartAction()
		{
			ResetPlayerActionBasedState();
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
			WeaponIsRanged = false;
			WeaponIsFinesse = false;
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

		public void TestPrepareWeaponAttack(string weaponName)
		{
			CarriedWeapon foundWeapon = CarriedWeapons.FirstOrDefault(x => x.Name == weaponName || x.Weapon.Name == weaponName);
			if (foundWeapon == null)
			{
				foundWeapon = AddWeapon(weaponName);
			}
			if (foundWeapon == null)
				return;
			List<PlayerActionShortcut> shortcut = PlayerActionShortcut.FromWeapon(foundWeapon, null, this);
			if (shortcut.Count > 0)
				PrepareWeaponAttack(shortcut[0]);
		}

		public override void PrepareWeaponAttack(PlayerActionShortcut playerActionShortcut)
		{
			_attackNum++;
			ResetPlayerActionBasedState();
			if (playerActionShortcut != null)
			{
				attackingAbilityModifier = playerActionShortcut.AttackingAbilityModifier;
				attackingAbility = playerActionShortcut.AttackingAbility;
				attackingType = playerActionShortcut.AttackingType;
				if (playerActionShortcut.Type == DiceRollType.Attack)
				{
					HasWeaponInHand = playerActionShortcut.CarriedWeapon != null;
					if (HasWeaponInHand)
					{
						Weapon weapon = AllWeapons.Get(playerActionShortcut.CarriedWeapon.Weapon.Name);
						WeaponIsFinesse = (weapon.weaponProperties & WeaponProperties.Finesse) == WeaponProperties.Finesse;
						WeaponIsHeavy = (weapon.weaponProperties & WeaponProperties.Heavy) == WeaponProperties.Heavy;
						WeaponIsRanged = (weapon.weaponProperties & WeaponProperties.Ranged) == WeaponProperties.Ranged;

						Expressions.BeginUpdate();
						try
						{
							foreach (AssignedFeature assignedFeature in features)
							{
								assignedFeature.WeaponRaised(this);
							}
						}
						finally
						{
							Expressions.EndUpdate(this);
						}

					}
				}
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

			if (tempHitPoints != 0)
			{
				tempHitPoints = 0;
				stateChanged = true;
			}

			if (HitPoints != maxHitPoints)
			{
				HitPoints = maxHitPoints;
				stateChanged = true;
			}

			if (stateChanged)
				OnStateChanged(this, new StateChangedEventArgs(string.Join(",", keys), 1, 0, true));

		}

		public bool HasRemainingSpellSlotCharges(int spellSlotLevel)
		{
			string key = DndUtils.GetSpellSlotLevelKey(spellSlotLevel);
			return GetIntState(key) < GetIntState(key + STR_RechargeableMaxSuffix);
		}

		[JsonIgnore]
		public string heShe { get; set; }

		[JsonIgnore]
		public string hisHer { get; set; }

		[JsonIgnore]
		public string himHer { get; set; }

		[JsonIgnore]
		public string HisHer { get; set; }

		[JsonIgnore]
		public string HeShe { get; set; }

		[JsonIgnore]
		public int NumWildMagicChecks { get; set; }

		public bool ShowingNameplate { get; set; } = true;

		[JsonIgnore]
		public Ability OverrideAttackingAbility { get; set; }

		[JsonIgnore]
		public int OverrideWeaponMagicHue { get; set; } = int.MinValue;

		[JsonIgnore]
		public string playerShortcut { get; set; }

		public int GetSpellcastingLevel()
		{
			return Level;
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
					case -1:
						return "Special: ";
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

			void AddKnownSpellToGroup(KnownSpell knownSpell)
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

			foreach (KnownSpell knownSpell in KnownSpells)
				AddKnownSpellToGroup(knownSpell);

			foreach (KnownSpell temporarySpell in temporarySpells)
				AddKnownSpellToGroup(temporarySpell);

			for (int i = 1; i <= HighestSpellSlot; i++)
				GetSpellGroupByLevel(i); // Ensures we have groups even for spell levels that have no spells (e.g., if a character only has first and third-level spells, this ensures we'll generate a group of second-level spell slots).

			SpellData = new List<SpellGroup>();

			foreach (int key in spellGroupsByLevel.Keys)
				SpellData.Add(spellGroupsByLevel[key]);
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
				report.Add($"{pair.Key} = {GetValueStr(pair.Value)}");

			foreach (Rechargeable rechargeable in rechargeables)
				report.Add($"{rechargeable.DisplayName} ({rechargeable.VarName}): {rechargeable.ChargesUsed}/{rechargeable.TotalCharges}");

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

		public List<CarriedItemDto> CarriedEquipment { get; set; }

		void BuildItemList()
		{
			CarriedEquipment = new List<CarriedItemDto>();
			foreach (CarriedItem carriedItem in CarriedItems)
			{
				CarriedEquipment.Add(new CarriedItemDto()
				{
					Name = carriedItem.Item.Name,
					Equipped = carriedItem.Equipped,
					CostValue = carriedItem.Item.CostValue,
					Weight = carriedItem.Item.Weight
				}
				);
			}
		}

		public void PrepareForSerialization()
		{
			BuildItemList();
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

		void SetField(string fieldName, ref decimal field, decimal newValue, bool isRechargeable = false)
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
			SetField("baseArmorClass", ref baseArmorClass, character.baseArmorClass);
			SetField("baseCharisma", ref baseCharisma, character.baseCharisma);
			SetField("baseConstitution", ref baseConstitution, character.baseConstitution);
			SetField("baseDexterity", ref baseDexterity, character.baseDexterity);
			SetField("baseIntelligence", ref baseIntelligence, character.baseIntelligence);
			SetField("baseStrength", ref baseStrength, character.baseStrength);
			SetField("alignment", ref alignmentStr, character.alignmentStr);
			SetField("deathSaveDeath1", ref deathSaveDeath1, character.deathSaveDeath1);
			SetField("deathSaveDeath2", ref deathSaveDeath2, character.deathSaveDeath2);
			SetField("deathSaveDeath3", ref deathSaveDeath3, character.deathSaveDeath3);
			SetField("deathSaveLife1", ref deathSaveLife1, character.deathSaveLife1);
			SetField("deathSaveLife2", ref deathSaveLife2, character.deathSaveLife2);
			SetField("deathSaveLife3", ref deathSaveLife3, character.deathSaveLife3);
			SetField("experiencePoints", ref experiencePoints, character.experiencePoints);
			SetField("goldPieces", ref goldPieces, character.goldPieces);
			SetField("hitPoints", ref hitPoints, character.HitPoints);
			SetField("initiative", ref initiative, character.initiative);
			SetField("inspiration", ref _inspiration, character.inspiration);
			SetField("load", ref load, character.load);
			SetField("name", ref base.name, character.name);
			SetField("proficiencyBonus", ref proficiencyBonus, character.proficiencyBonus);
			SetField("baseWalkingSpeed", ref baseWalkingSpeed, character.baseWalkingSpeed);
			SetField("tempHitPoints", ref tempHitPoints, character.tempHitPoints);
			SetField("totalHitDice", ref totalHitDice, character.totalHitDice);
			SetField("weight", ref weight, character.weight);

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
				spellPrepared = null;
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
				eventCategory.Add(new EventGroup(assignedFeature.Feature, assignedFeature.Feature?.Name, eventType, DndEventManager.knownEvents[type]));
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

			foreach (KnownSpell spell in player.temporarySpells)
			{
				eventCategory.Add(new EventGroup(spell, spell.SpellName, eventType, DndEventManager.knownEvents[type]));
			}

			return eventCategory;
		}

		public void RebuildAllEvents()
		{
			eventCategories.Clear();
			AddEvents(CreateFeatureEvents(this));
			AddEvents(CreateSpellEvents(this));
		}
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

		public IEnumerable GetAllEventGroups()
		{
			List<EventGroup> allGroups = new List<EventGroup>();
			foreach (EventCategory eventCategory in eventCategories)
			{
				allGroups.AddRange(eventCategory.Groups);
			}
			return allGroups;
		}

		public KnownSpell GetMatchingSpell(string spellName)
		{
			return KnownSpells.FirstOrDefault(x => x.SpellName == spellName);
		}

		public void PreparingSpell()
		{
			WeaponIsFinesse = false;
			WeaponIsHeavy = false;
			WeaponIsRanged = false;
		}

		public static void SetBoolProperty(Creature player, string memberName, bool value)
		{
			if (player == null)
				return;

			PropertyInfo propertyInfo = player.GetType().GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);
			if (null != propertyInfo && propertyInfo.CanWrite)
			{
				propertyInfo.SetValue(player, value, null);
				return;
			}

			FieldInfo fieldInfo = player.GetType().GetField(memberName, BindingFlags.Public | BindingFlags.Instance);
			if (null != fieldInfo)
			{
				fieldInfo.SetValue(player, value);
			}
		}

		public static bool GetBoolProperty(Character player, string memberName)
		{
			if (player == null)
				return false;

			PropertyInfo propertyInfo = player.GetType().GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);
			if (null != propertyInfo && propertyInfo.CanWrite)
				return (bool)propertyInfo.GetValue(player);

			FieldInfo fieldInfo = player.GetType().GetField(memberName, BindingFlags.Public | BindingFlags.Instance);
			if (null != fieldInfo)
				return (bool)fieldInfo.GetValue(player);

			return false;
		}

		public void GiveSpell(KnownSpell givenSpell, object data1, object data2, object data3, object data4, object data5, object data6, object data7)
		{
			if (givenSpell == null)
				return;
			Spell giftedSpell = AllSpells.Get(givenSpell.SpellName, this);
			if (giftedSpell != null)
				giftedSpell.TriggerSpellReceived(this, giftedSpell, data1, data2, data3, data4, data5, data6, data7);
			temporarySpells.Add(givenSpell);
			OnStateChanged(this, new StateChangedEventArgs("temporarySpells", null, null));
		}

		public void TakeSpell(string spellName)
		{
			KnownSpell knownSpell = temporarySpells.FirstOrDefault(x => x.SpellName == spellName);
			if (knownSpell == null)
				return;
			temporarySpells.Remove(knownSpell);
			OnStateChanged(this, new StateChangedEventArgs("temporarySpells", null, null));
		}

		public void ChangeWealth(decimal deltaGoldPieces)
		{
			decimal oldValue = goldPieces;
			goldPieces += deltaGoldPieces;
			// TODO: Add Debt to solve this so Debt and gold are both non negative.
			if (goldPieces < 0)  // 
				goldPieces = 0;
			if (goldPieces != oldValue)
				OnStateChanged(this, new StateChangedEventArgs("goldPieces", oldValue, goldPieces));
		}

		CarriedWeapon GetActiveWeapon()
		{
			string activeWeaponName = ActiveWeaponName;
			if (!activeWeaponName.HasSomething())
				return null;
			return CarriedWeapons.FirstOrDefault(x => x.Name == activeWeaponName || x.Weapon.Name == activeWeaponName);
		}

		public void UseAmmunition(string kind, int value)
		{
			CarriedAmmunition carriedAmmunition = ReadiedAmmunition;
			if (carriedAmmunition == null)
				carriedAmmunition = GetAmmunition(kind);
			carriedAmmunition.Count -= value;
			if (carriedAmmunition.Count < 0)
				carriedAmmunition.Count = 0;
			string ammunitionName;

			if (carriedAmmunition.Name.HasSomething())
				ammunitionName = carriedAmmunition.Name;
			else
				ammunitionName = $"normal {carriedAmmunition.Kind.ToLower()}";

			OnRequestMessageToAll($"{firstName}'s remaining {ammunitionName} count: {carriedAmmunition.Count}");
		}

		public CarriedAmmunition GetAmmunition(string kind)
		{
			return CarriedAmmunition.FirstOrDefault(x => x.Kind == kind);
		}

		public List<CarriedAmmunition> GetAllAmmunition(string kind)
		{
			return CarriedAmmunition.Where(x => x.Kind == kind).ToList();
		}

		public void AttackingNow(Target target)
		{
			List<CastedSpell> activeSpells = GetActiveSpells();
			if (activeSpells == null)
				return;
			foreach (CastedSpell castedSpell in activeSpells)
			{
				castedSpell.Spell.TriggerPlayerAttacks(this as Character, target, castedSpell);
			}
			CarriedWeapon activeWeapon = GetActiveWeapon();
			if (activeWeapon != null && activeWeapon.Weapon.RequiresAmmunition())
			{
				UseAmmunition(activeWeapon.Weapon.AmmunitionKind, 1);
			}
		}

		public CarriedAmmunition ChooseAmmunition(string ammunitionKind)
		{
			PickAmmunitionEventArgs pickAmmunitionEventArgs = new PickAmmunitionEventArgs(this, ammunitionKind);
			OnPickAmmunition(this, pickAmmunitionEventArgs);
			return pickAmmunitionEventArgs.Ammunition;
		}

		public bool HasAmmunition(string kind)
		{
			return GetAmmunitionCount(kind) > 0;
		}

		public override double Strength
		{
			get
			{
				if (WildShape != null)
					return WildShape.Strength;
				return base.Strength;
			}
		}
		public override double Dexterity
		{
			get
			{
				if (WildShape != null)
					return WildShape.Dexterity;
				return base.Dexterity;
			}
		}
		public override double Constitution
		{
			get
			{
				if (WildShape != null)
					return WildShape.Constitution;
				return base.Constitution;
			}
		}

		public override double darkvisionRadius
		{
			get
			{
				if (WildShape != null)
					return WildShape.darkvisionRadius;
				return base.darkvisionRadius;
			}
			set
			{
				if (WildShape != null)
					WildShape.darkvisionRadius = value;
				base.darkvisionRadius = value;
			}
		}

		public override double tremorSenseRadius
		{
			get
			{
				if (WildShape != null)
					return WildShape.tremorSenseRadius;
				return base.tremorSenseRadius;
			}
			set
			{
				if (WildShape != null)
					WildShape.tremorSenseRadius = value;
				base.tremorSenseRadius = value;
			}
		}

		public override double truesightRadius
		{
			get
			{
				if (WildShape != null)
					return WildShape.truesightRadius;
				return base.truesightRadius;
			}
			set
			{
				if (WildShape != null)
					WildShape.truesightRadius = value;
				base.truesightRadius = value;
			}
		}

		public override double blindsightRadius
		{
			get
			{
				if (WildShape != null)
					return WildShape.blindsightRadius;
				return base.blindsightRadius;
			}
			set
			{
				if (WildShape != null)
					WildShape.blindsightRadius = value;
				base.blindsightRadius = value;
			}
		}

		[JsonIgnore]
		public Spell JustWarnedAbout { get; set; }
		/// <summary>
		/// The name of the OBS scene holding the character's video feed.
		/// </summary>
		public string sceneName { get; set; }

		/// <summary>
		/// The percent across of the video feed's width where the anchor is located.
		/// </summary>
		public double videoAnchorHorizontal { get; set; }

		/// <summary>
		/// The percent down the video feed's height where the anchor is located.
		/// </summary>
		public double videoAnchorVertical { get; set; }

		/// <summary>
		/// The height of the video feed when it's scaled normally.
		/// </summary>
		public double videoHeight { get; set; }

		/// <summary>
		/// The width of the video feed when it's scaled normally.
		/// </summary>
		public double videoWidth { get; set; }

		public void AddFeature(string featureName)
		{
			features.Add(AssignedFeature.From(featureName, this));
		}

		public CanCastResult CanCast(Spell spell)
		{
			return new CanCastResult(Location, spell.Range);
		}
		public CharacterClass FirstSpellCastingClass()
		{
			return Classes.FirstOrDefault(x => DndUtils.CanCastSpells(x.Name));
		}

		public override bool Equals(object obj)
		{
			return obj is Character character &&
						 spellCastingAbility == character.spellCastingAbility;
		}

		public string GetResistancesVulnerabilitiesImmunitiesStr()
		{
			// TODO: Implement this.
			return resistancesVulnerabilitiesImmunitiesStr;
		}

		public override double GetSkillCheckMod(Skills skill)
		{
			switch (skill)
			{
				case Skills.acrobatics:
					return skillModAcrobatics;
				case Skills.animalHandling:
					return skillModAnimalHandling;
				case Skills.arcana:
					return skillModArcana;
				case Skills.athletics:
					return skillModAthletics;
				case Skills.deception:
					return skillModDeception;
				case Skills.history:
					return skillModHistory;
				case Skills.insight:
					return skillModInsight;
				case Skills.intimidation:
					return skillModIntimidation;
				case Skills.investigation:
					return skillModInvestigation;
				case Skills.medicine:
					return skillModMedicine;
				case Skills.nature:
					return skillModNature;
				case Skills.perception:
					return skillModPerception;
				case Skills.performance:
					return skillModPerformance;
				case Skills.persuasion:
					return skillModPersuasion;
				case Skills.religion:
					return skillModReligion;
				case Skills.sleightOfHand:
					return skillModSleightOfHand;
				case Skills.stealth:
					return skillModStealth;
				case Skills.survival:
					return skillModSurvival;
				case Skills.strength:
					return strengthMod;
				case Skills.dexterity:
					return dexterityMod;
				case Skills.charisma:
					return charismaMod;
				case Skills.intelligence:
					return intelligenceMod;
				case Skills.constitution:
					return constitutionMod;
				case Skills.wisdom:
					return wisdomMod;
			}

			return 0;
		}
	}
}