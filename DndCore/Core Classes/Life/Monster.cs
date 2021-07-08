using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;
using Newtonsoft.Json;

namespace DndCore
{
	[SheetName("DnD")]
	[TabName("Monsters")]
	public class Monster : Creature
	{
		public override int Level { get => (int)Math.Min(21, Math.Ceiling(challengeRating)); }

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

		int _intId;

		[JsonIgnore]
		public override int IntId { get { return _intId; } }

		public void SetIntId(int id)
		{
			_intId = id;
		}

		[Column]
		[Indexer]
		public string Kind { get; set; }

		public double challengeRating;

		public string ChallengeRatingStr
		{
			get
			{
				if (challengeRating == 1 / 8d)
					return "1/8";
				if (challengeRating == 1 / 4d)
					return "1/4";
				if (challengeRating == 1 / 2d)
					return "1/2";
				return challengeRating.ToString();
			}
		}

		[Column("imageCrop")]
		public string imageCropStr
		{
			get
			{
				return ImageCropInfo.ToCSV();
			}
		}
		

		public PictureCropInfo ImageCropInfo { get; set; }
		public double charismaMod;
		public double constitutionMod;
		public double dexterityMod;
		public double strengthMod;
		public double wisdomMod;
		public double intelligenceMod;

		public double experiencePoints;
		public string hitPointsDice;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}


		public bool naturalArmor { get; set; }

		public int passivePerception
		{
			get
			{
				if (PassivePerceptionOverride != int.MinValue)
					return PassivePerceptionOverride;
				return 10 + (int)Math.Round(wisdomMod);
			}
			set
			{
				PassivePerceptionOverride = value;
			}
		}

		public double savingCharismaMod { get; set; }
		public double savingConstitutionMod { get; set; }
		public double savingDexterityMod { get; set; }
		public double savingIntelligenceMod { get; set; }

		public double savingStrengthMod { get; set; }
		public double savingWisdomMod { get; set; }

		public int skillsModStealth { get; set; }
		
		[Column("img_url")]
		public string ImageUrl { get; set; }

		public List<string> traits = new List<string>();

		public void AddAttack(Attack attack)
		{
			attacks.Add(attack);
		}

		public void AddLanguages(Languages languages)
		{
			languagesSpoken |= languages;
			languagesUnderstood |= languages;
		}

		public void AddMultiAttack(params string[] attackNames)
		{
			foreach (string attackName in attackNames)
			{
				Attack attack = attacks.Find(x => x.Name == attackName);
				if (attack != null)
					multiAttack.Add(attack);
			}
		}

		public override double GetAbilityModifier(Ability modifier)
		{
			switch (modifier)
			{
				case Ability.strength:
					return strengthMod;
				case Ability.dexterity:
					return dexterityMod;
				case Ability.constitution:
					return constitutionMod;
				case Ability.intelligence:
					return intelligenceMod;
				case Ability.wisdom:
					return wisdomMod;
				case Ability.charisma:
					return charismaMod;
			}
			return 0;
		}

		public override Attack GetAttack(string attackName)
		{
			return attacks.Find(x => x.Name == attackName);
		}

		public int GetAttackRoll(int value, string attackName)
		{
			Attack attack = GetAttack(attackName);
			if (attack == null)
				return 0;
			return value + (int)Math.Floor(attack.plusToHit);
		}

		public DamageResult GetDamageFromAttack(Creature creature, string attackName, int savingThrow, int attackRoll = int.MaxValue)
		{
			if (attackRoll < creature.ArmorClass)
				return null;
			Attack attack = attacks.Find(x => x.Name == attackName);
			if (attack != null)
				return attack.GetDamage(creature, savingThrow);
			return null;
		}

		public void SetAbilities(int strength, int strengthMod, int dexterity, int dexterityMod, int constitution, int constitutionMod, int intelligence, int intelligenceMod, int wisdom, int wisdomMod, int charisma, int charismaMod)
		{
			this.baseStrength = strength;
			this.strengthMod = strengthMod;
			this.baseDexterity = dexterity;
			this.dexterityMod = dexterityMod;
			this.baseConstitution = constitution;
			this.constitutionMod = constitutionMod;
			this.baseIntelligence = intelligence;
			this.intelligenceMod = intelligenceMod;
			this.baseWisdom = wisdom;
			this.wisdomMod = wisdomMod;
			this.baseCharisma = charisma;
			this.charismaMod = charismaMod;
		}

		// ![](36C54F2C5F48AF603559DA1AFB31DAF9.png;https://www.dndbeyond.com/monsters/vine-blight )
		/// <summary>
		/// Sets abilities from a string copied from dndbeyond.com
		/// Expecting the string to look like this:
		///     "STR
		///      15 (+2)
		///      DEX
		///      8 (-1)
		///      CON
		///      14 (+2)
		///      INT
		///      5 (-3)
		///      WIS
		///      10 (+0)
		///      CHA
		///      3 (-4)"
		/// </summary>
		/// <param name="str"></param>
		public void SetAbilitiesFromStr(string str)
		{
			string[] lines = str.Split('\n');
			SetAbilityFromStr(lines[1], ref baseStrength, ref strengthMod);
			SetAbilityFromStr(lines[3], ref baseDexterity, ref dexterityMod);
			SetAbilityFromStr(lines[5], ref baseConstitution, ref constitutionMod);
			SetAbilityFromStr(lines[7], ref baseIntelligence, ref intelligenceMod);
			SetAbilityFromStr(lines[9], ref baseWisdom, ref wisdomMod);
			SetAbilityFromStr(lines[11], ref baseCharisma, ref charismaMod);
		}

		void SetAbilityFromStr(string str, ref double ability, ref double abilityMod)
		{
			int openParen = str.IndexOf("(");
			int closeParen = str.IndexOf(")");
			string abilityStr = str.Substring(0, openParen).Trim();
			string abilityModStr = str.Substring(openParen + 1, closeParen - openParen - 1).Trim();
			ability = abilityStr.ToDouble();
			abilityMod = abilityModStr.ToDouble();
		}

		public void PrepareAttack(Creature creature, string attackName)
		{
			PrepareAttack(creature, GetAttack(attackName));
		}

		void SetFromMeta(string meta)
		{
			string[] parts = meta.Split(',');
			if (parts.Length != 2)
			{
				System.Diagnostics.Debugger.Break();
				return;
			}
			alignmentStr = parts[1].Trim();
			string[] sizeCreatureKind = parts[0].Split(' ');
			if (sizeCreatureKind.Length != 2)
			{
				System.Diagnostics.Debugger.Break();
			}
			Alignment = DndUtils.ToAlignment(alignmentStr);
			creatureSize = DndUtils.ToCreatureSize(sizeCreatureKind[0]);
			kind = DndUtils.ToCreatureKind(sizeCreatureKind[1]);
		}

		void SetArmorClassFromStr(string armorClass)
		{
			// TODO: Add support for alternate forms, e.g., "10 In Humanoid Form, 11 In Bear Or Hybrid Form"
			string[] acDetails = armorClass.Split(' ');
			if (acDetails.Length == 0)
				return;
			if (int.TryParse(acDetails[0], out int result))
				baseArmorClass = result;
		}

		void SetHitPointsFromStr(string hitPointsStr)
		{
			string[] hpDetails = hitPointsStr.Split(' ');
			if (hpDetails.Length == 0)
				return;
			if (int.TryParse(hpDetails[0], out int result))
			{
				maxHitPoints = result;
				HitPoints = result;
			}
			if (hpDetails.Length > 1)
				hitPointsDice = hpDetails[1].Trim('(', ')');
		}

		void SetAbilitiesFrom(MonsterDto monsterDto)
		{
			baseStrength = MathUtils.GetInt(monsterDto.STR);
			strengthMod = GetNumberInParens(monsterDto.STR_mod);
			baseIntelligence = MathUtils.GetInt(monsterDto.INT);
			intelligenceMod = GetNumberInParens(monsterDto.INT_mod);
			baseDexterity = MathUtils.GetInt(monsterDto.DEX);
			dexterityMod = GetNumberInParens(monsterDto.DEX_mod);
			baseConstitution = MathUtils.GetInt(monsterDto.CON);
			constitutionMod = GetNumberInParens(monsterDto.CON_mod);
			baseWisdom = MathUtils.GetInt(monsterDto.WIS);
			wisdomMod = GetNumberInParens(monsterDto.WIS_mod);
			baseCharisma = MathUtils.GetInt(monsterDto.CHA);
			charismaMod = GetNumberInParens(monsterDto.CHA_mod);
		}

		private static int GetNumberInParens(string numStr)
		{
			return MathUtils.GetInt(numStr.Trim('(', ')', ' '));
		}

		void SetSpeed(string movementKind, string valueStr)
		{
			if (!int.TryParse(valueStr.Trim(), out int value))
				return;
			switch (movementKind)
			{
				case "walk":
					baseWalkingSpeed = value;
					break;
				case "fly":
					flyingSpeed = value;
					break;
				case "burrow":
					burrowingSpeed = value;
					break;
				case "climb":
					climbingSpeed = value;
					break;
				case "swim":
					swimmingSpeed = value;
					break;
				default:
					System.Diagnostics.Debugger.Break();
					break;
			}
		}

		void SetIndividualSpeedFromStr(string individualSpeed)
		{
			int unitPos = individualSpeed.IndexOf("ft.");
			if (unitPos <= 0)
				return;
			string numberStr = individualSpeed.Substring(0, unitPos).Trim();
			// TODO: Add support for descriptive speed modifiers beyond the unit (e.g., "(hover)" or "in bear form").
			string[] parts = numberStr.Split(' ');
			if (parts.Length > 0)
				if (parts.Length > 1)
				{
					// First part is expected to describe the kind of movement (climb, burrow, fly, etc.). 
					// Second part is the value in feet of movement. (e.g., "burrow 40").
					SetSpeed(parts[0], parts[1]);
				}
				else
					SetSpeed("walk", parts[0]);
		}

		void SetSpeedFromStr(string speed)
		{
			string[] speeds = speed.Split(',');
			foreach (string individualSpeed in speeds)
			{
				SetIndividualSpeedFromStr(individualSpeed);
			}
		}

		Dictionary<Skills, int> skillCheckOverrides = new Dictionary<Skills, int>();

		void AddSkillCheckOverride(Skills skill, int value)
		{
			if (!skillCheckOverrides.ContainsKey(skill))
				skillCheckOverrides.Add(skill, value);
			else
				skillCheckOverrides[skill] = value;
		}

		public int PassivePerceptionOverride = int.MinValue;
		void SetSkillCheckBonus(string skillStr)
		{
			skillStr = skillStr.Trim();
			if (string.IsNullOrEmpty(skillStr))
				return;
			int lastSpacePos = skillStr.LastIndexOf(' ');
			if (lastSpacePos < 0)
				return;

			string skillName = skillStr.Substring(0, lastSpacePos).Trim();
			string bonusStr = skillStr.Substring(lastSpacePos).Trim();


			if (!int.TryParse(bonusStr, out int bonus))
				return;

			Skills skill = DndUtils.ToSkill(skillName);
			AddSkillCheckOverride(skill, bonus);
		}
		void SetSkillCheckBonuses(string skillListStr)
		{
			string[] skills = skillListStr.Split(',');
			foreach (string skill in skills)
			{
				SetSkillCheckBonus(skill);
			}
		}

		void SetSenseFromStr(string sense)
		{
			sense = sense.Trim();
			if (sense.Contains(" ft"))
				sense = sense.EverythingBefore(" ft");
			int lastSpacePos = sense.LastIndexOf(" ");
			if (lastSpacePos < 0)
				return;
			string senseName = sense.Substring(0, lastSpacePos);
			string valueStr = sense.Substring(lastSpacePos + 1);
			if (!int.TryParse(valueStr, out int value))
				return;

			switch (senseName)
			{
				case "Passive Perception":
					PassivePerceptionOverride = value;
					break;
				case "Blindsight":
					blindsightRadius = value;
					break;
				case "Darkvision":
					darkvisionRadius = value;
					break;
				case "Truesight":
					truesightRadius = value;
					break;
				case "Tremorsense":
					tremorSenseRadius = value;
					break;
				default:
					System.Diagnostics.Debugger.Break();
					break;
			}
		}

		void SetSensesFromStr(string senseListStr)
		{
			if (string.IsNullOrEmpty(senseListStr))
				return;
			string[] senseStrs = senseListStr.Split(',');
			foreach (string sense in senseStrs)
			{
				SetSenseFromStr(sense);
			}
		}

		void SetChallengeRatingFromStr(string challengeStr)
		{
			switch (challengeStr)
			{
				case "1/8":
					challengeRating = 1d / 8d;
					break;
				case "1/4":
					challengeRating = 1d / 4d;
					break;
				case "1/2":
					challengeRating = 1d / 2d;
					break;
				default:
					if (double.TryParse(challengeStr, out double value))
						challengeRating = value;
					break;
			}
		}

		void SetExperiencePointsFromStr(string str)
		{
			if (int.TryParse(str, out int value))
				experiencePoints = value;
		}

		void SetChallengeRatingXpFromStr(string challenge)
		{
			if (string.IsNullOrEmpty(challenge))
				return;
			challenge = challenge.Replace(" XP)", "");
			challenge = challenge.Replace("(", "");
			challenge = challenge.Replace(",", "");
			string[] parts = challenge.Split(' ');
			if (parts.Length != 2)
			{
				System.Diagnostics.Debugger.Break();
				return;
			}
			SetChallengeRatingFromStr(parts[0]);
			SetExperiencePointsFromStr(parts[1]);
		}
		void SetSavingThrowModsFrom(string savingThrows)
		{
			if (string.IsNullOrWhiteSpace(savingThrows))
				return;
			string[] splitSaveMods = savingThrows.Split(',');
			foreach (string saveMod in splitSaveMods)
			{
				string[] parts = saveMod.Trim().Split(' ');
				if (parts.Length != 2)
				{
					System.Diagnostics.Debugger.Break();
					continue;
				}
				if (!double.TryParse(parts[1], out double value))
				{
					System.Diagnostics.Debugger.Break();
					continue;
				}
				switch (parts[0])
				{
					case "CON":
						savingConstitutionMod = value;
						break;
					case "INT":
						savingIntelligenceMod = value;
						break;
					case "WIS":
						savingWisdomMod = value;
						break;
					case "DEX":
						savingDexterityMod = value;
						break;
					case "CHA":
						savingCharismaMod = value;
						break;
					case "STR":
						savingStrengthMod = value;
						break;
				}
			}
		}

		public static Monster From(MonsterDto monsterDto)
		{
			Monster monster = new Monster();
			monster.SetFromMeta(monsterDto.Meta);
			monster.Kind = monsterDto.Kind;
			monster.SetArmorClassFromStr(monsterDto.ArmorClass);
			monster.SetHitPointsFromStr(monsterDto.HitPoints);
			monster.SetAbilitiesFrom(monsterDto);
			monster.SetSavingThrowModsFrom(monsterDto.SavingThrows);
			monster.SetSpeedFromStr(monsterDto.Speed);
			monster.SetSkillCheckBonuses(monsterDto.Skills);
			monster.SetDamageImmunities(monsterDto.DamageImmunities);
			monster.SetConditionImmunities(monsterDto.ConditionImmunities);
			monster.SetDamageVulnerabilities(monsterDto.DamageVulnerabilities);
			monster.SetDamageResistances(monsterDto.DamageResistances);
			monster.SetSensesFromStr(monsterDto.Senses);
			monster.SetChallengeRatingXpFromStr(monsterDto.Challenge);
			monster.ImageUrl = monsterDto.img_url;
			monster.ImageCropInfo = PictureCropInfo.FromStr(monsterDto.imageCrop);
			return monster;
		}

		public bool HasOverridingSkillMod(Skills skill)
		{
			return skillCheckOverrides.ContainsKey(skill);
		}

		public int GetOverridingSkillMod(Skills skill)
		{
			return skillCheckOverrides[skill];
		}

		public static Monster Clone(Monster source)
		{
			Monster monster = new Monster();
			if (source == null)
				return null;
			monster.ManuallyAddedConditions = source.ManuallyAddedConditions;
			monster.advantages = source.advantages;
			monster.Alignment = source.Alignment;
			monster.alignmentStr = source.alignmentStr;
			monster.attacks = source.attacks;
			monster.baseArmorClass = source.baseArmorClass;
			monster.baseCharisma = source.baseCharisma;
			monster.baseConstitution = source.baseConstitution;
			monster.baseDexterity = source.baseDexterity;
			monster.baseIntelligence = source.baseIntelligence;
			monster.baseStrength = source.baseStrength;
			monster.baseWalkingSpeed = source.baseWalkingSpeed;
			monster.baseWisdom = source.baseWisdom;
			monster.blindsightRadius = source.blindsightRadius;
			monster.burrowingSpeed = source.burrowingSpeed;
			monster.CalculatedMods = source.CalculatedMods;
			monster.challengeRating = source.challengeRating;
			monster.charismaMod = source.charismaMod;
			monster.climbingSpeed = source.climbingSpeed;
			monster.conditionImmunities = source.conditionImmunities;
			monster.constitutionMod = source.constitutionMod;
			monster.creatureSize = source.creatureSize;
			//monster.cursesAndBlessings = source.cursesAndBlessings;
			monster.damageImmunities = source.damageImmunities;
			monster.damageResistance = source.damageResistance;
			monster.damageVulnerability = source.damageVulnerability;
			monster.darkvisionRadius = source.darkvisionRadius;
			monster.dexterityMod = source.dexterityMod;
			monster.disadvantages = source.disadvantages;
			//monster.equipment = source.equipment;
			monster.experiencePoints = source.experiencePoints;
			monster.flyingSpeed = source.flyingSpeed;
			//monster.Game = source.Game;
			monster.GoldPieces = source.GoldPieces;
			monster.HitPoints = source.HitPoints;
			monster.ImageUrl = source.ImageUrl;
			monster.ImageCropInfo = source.ImageCropInfo;
			monster.initiative = source.initiative;
			monster.intelligenceMod = source.intelligenceMod;
			monster.kind = source.kind;
			monster.languagesSpoken = source.languagesSpoken;
			monster.languagesUnderstood = source.languagesUnderstood;
			monster.LastDamagePointsTaken = source.LastDamagePointsTaken;
			monster.LastDamageTaken = source.LastDamageTaken;
			monster.Location = source.Location;
			monster.maxHitPoints = source.maxHitPoints;
			monster.multiAttack = source.multiAttack;  // shared
			monster.multiAttackCount = source.multiAttackCount;  // shared
			monster.name = source.name;
			monster.Kind = source.Kind;
			monster.naturalArmor = source.naturalArmor;
			monster.offTurnActions = source.offTurnActions;
			monster.onTurnActions = source.onTurnActions;
			monster.passivePerception = source.passivePerception;
			monster.PassivePerceptionOverride = source.PassivePerceptionOverride;
			monster.race = source.race;
			monster.savingCharismaMod = source.savingCharismaMod;
			monster.savingConstitutionMod = source.savingConstitutionMod;
			monster.savingDexterityMod = source.savingDexterityMod;
			monster.savingIntelligenceMod = source.savingIntelligenceMod;
			monster.savingStrengthMod = source.savingStrengthMod;
			monster.savingWisdomMod = source.savingWisdomMod;
			monster.senses = source.senses;
			monster.skillCheckOverrides = source.skillCheckOverrides;
			monster.skillsModStealth = source.skillsModStealth;
			monster.strengthMod = source.strengthMod;
			monster.swimmingSpeed = source.swimmingSpeed;
			monster.telepathyRadius = source.telepathyRadius;
			monster.tempArmorClassMod = source.tempArmorClassMod;
			monster.tempHitPoints = source.tempHitPoints;
			monster.traits = source.traits;  // shared
			monster.tremorSenseRadius = source.tremorSenseRadius;
			monster.truesightRadius = source.truesightRadius;
			monster.wisdomMod = source.wisdomMod;
			return monster;
		}

		public override double GetSavingThrowModifier(Ability savingThrowAbility)
		{
			switch (savingThrowAbility)
			{
				case Ability.strength:
					return savingStrengthMod;
				case Ability.dexterity:
					return savingDexterityMod;
				case Ability.constitution:
					return savingConstitutionMod;
				case Ability.intelligence:
					return savingIntelligenceMod;
				case Ability.wisdom:
					return savingWisdomMod;
				case Ability.charisma:
					return savingCharismaMod;
			}
			return 0;
		}

		public override double GetSkillCheckMod(Skills skill)
		{
			Ability ability = DndUtils.ToAbility(skill);
			return GetAbilityModifier(ability);
		}
	}
}