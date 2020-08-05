using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using GoogleHelper;

namespace DndCore
{
	[HasDndEvents]
	[SheetName("DnD")]
	[TabName("Spells")]
	public class Spell
	{
		public int Range { get; set; }
		public int AmmoCount { get; set; }
		public int OriginalAmmoCount { get; set; }
		public string AmmoCount_word
		{
			get
			{
				return DndUtils.ToWordStr(AmmoCount);
			}
			set { }
		}
		public string AmmoCount_Word
		{
			get
			{
				return DndUtils.ToWordStr(AmmoCount).InitialCap();
			}
			set { }
		}
		public SpellRangeType RangeType { get; set; }
		public SpellType SpellType { get; set; }
		public DndTimeSpan Duration { get; set; }
		public DndTimeSpan CastingTime { get; set; }

		[Indexer("name")]
		public string Name { get; set; }
		[Indexer("index")]
		public string Index { get; set; }

		public string Description { get; set; }
		public SpellComponents Components { get; set; }
		public string Material { get; set; }
		public int OwnerId { get; set; }
		public bool RequiresConcentration { get; set; }
		public int Level { get; set; }  // 0 == cantrip, -1 == special power (like Heavenly Smite)
		public int SpellSlotLevel { get; set; }
		public int SpellCasterLevel { get; set; }
		public string DieStr { get; set; }
		public Ability SavingThrowAbility { get; set; }
		public string BonusThreshold { get; set; }
		public int BonusMax { get; set; }
		public string OriginalDieStr { get; set; }
		public string BonusPerLevel { get; set; }
		public double PerLevelBonus { get; set; }

		[DndEvent]
		[Column]
		public string OnCast { get; set; }

		[DndEvent]
		[Column]
		public string OnReceived { get; set; }

		[DndEvent]
		[Column]
		public string OnPreparing { get; set; }

		[DndEvent]
		[Column]
		public string OnPreparationComplete { get; set; }

		[DndEvent]
		[Column]
		public string OnGetAttackAbility { get; set; }

		[DndEvent]
		[Column]
		public string OnPlayerPreparesAttack { get; set; }

		[DndEvent]
		[Column]
		public string OnDieRollStopped { get; set; }

		[DndEvent]
		[Column]
		public string OnPlayerAttacks { get; set; }

		[DndEvent]
		[Column]
		public string OnPlayerHitsTarget { get; set; }

		[DndEvent]
		[Column]
		public string OnDispel { get; set; }
		public string AvailableWhen { get; set; }



		public Spell()
		{
			SpellType = SpellType.OtherSpell;
			RangeType = SpellRangeType.DistanceFeet;
		}

		static DndTimeSpan GetCastingTime(string durationStr)
		{
			string duration = durationStr.ToLower();

			if (duration.IndexOf("until dispelled") > 0)
				return DndTimeSpan.Forever;

			if (duration.IndexOf("special") > 0)
				return DndTimeSpan.Unknown;

			return DndTimeSpan.FromDurationStr(duration);
		}

		static int GetRange(SpellDto spellDto, SpellRangeType rangeType)
		{
			switch (rangeType)
			{
				case SpellRangeType.DistanceFeet:
				case SpellRangeType.SelfPlusFeetLine:
				case SpellRangeType.DistanceMiles:
				case SpellRangeType.SelfPlusFlatRadius:
				case SpellRangeType.SelfPlusSphereRadius:
				case SpellRangeType.SelfPlusCone:
				case SpellRangeType.SelfPlusCube:
					return spellDto.range.GetFirstInt();
			}
			return 0;
		}
		static bool GetRequiresConcentration(SpellDto spellDto)
		{
			return spellDto.duration.StartsWith("Concentration");
		}
		static SpellRangeType GetRangeType(SpellDto spellDto)
		{
			string range = spellDto.range.ToLower();
			if (range == "touch")
				return SpellRangeType.Touch;
			if (range == "special")
				return SpellRangeType.Special;
			if (range == "sight")
				return SpellRangeType.Sight;
			if (range == "unlimited")
				return SpellRangeType.Unlimited;

			if (range.IndexOf("self") >= 0)
			{
				if (range.IndexOf("cone") >= 0)
					return SpellRangeType.SelfPlusCone;
				if (range.IndexOf("line") >= 0)
					return SpellRangeType.SelfPlusFeetLine;
				if (range.IndexOf("cube") >= 0)
					return SpellRangeType.SelfPlusCube;
				if (range.IndexOf("sphere") >= 0)
					return SpellRangeType.SelfPlusSphereRadius;
				if (range.IndexOf("radius") >= 0)
					return SpellRangeType.SelfPlusFlatRadius;

				return SpellRangeType.Self;
			}

			if (range.IndexOf("mile") >= 0)
				return SpellRangeType.DistanceMiles;


			if (range.IndexOf("feet") >= 0 || range.IndexOf("foot") >= 0)
				return SpellRangeType.DistanceFeet;

			throw new NotImplementedException();
		}

		static int GetLevel(string levelStr)
		{
			levelStr = levelStr.ToLower();
			if (levelStr.IndexOf("cantrip") > 0)
				return 0;
			if (levelStr == "*")
				return -1;
			return levelStr.GetFirstInt();
		}

		string GetDieStr(int spellSlotLevel, int spellCasterLevel, int spellcastingAbilityModifier)
		{
			DieRollDetails details = DieRollDetails.From(OriginalDieStr, spellcastingAbilityModifier);

			string dieStr = details.ToString();

			if (string.IsNullOrWhiteSpace(BonusThreshold))
				return dieStr;

			if (!GetMultiplier(spellSlotLevel, spellCasterLevel, out double multiplier))
				return dieStr;

			DieRollDetails bonusDetails = DieRollDetails.From(BonusPerLevel, spellcastingAbilityModifier);

			foreach (Roll bonusRoll in bonusDetails.Rolls)
			{
				Roll matchingRoll = details.GetRoll(bonusRoll.Sides);
				if (matchingRoll == null)
					details.AddRoll(bonusRoll.ToString());
				else if (multiplier != 0)
				{
					matchingRoll.Count += (int)Math.Floor(bonusRoll.Count * multiplier);
					if (BonusMax > 0 && matchingRoll.Count > BonusMax)
						matchingRoll.Count = BonusMax;
					matchingRoll.Offset += (int)Math.Floor(bonusRoll.Offset * multiplier);
				}
			}

			return details.ToString();
		}

		private bool GetMultiplier(int spellSlotLevel, int spellCasterLevel, out double multiplier)
		{
			string bonusThreshold = BonusThreshold.ToLower();
			int compareLevel = spellSlotLevel;
			if (bonusThreshold.StartsWith("c"))
				compareLevel = spellCasterLevel;

			int minThreshold = bonusThreshold.GetFirstInt();
			multiplier = compareLevel - minThreshold;
			int totalBonus = (int)Math.Floor(PerLevelBonus * multiplier);
			return totalBonus > 0;
		}

		private static SpellType GetSpellType(SpellDto spellDto)
		{
			if (!string.IsNullOrWhiteSpace(spellDto.attack_type))
			{
				string attackType = spellDto.attack_type.ToLower();
				if (attackType.IndexOf("melee") >= 0)
					return SpellType.MeleeSpell;
				if (attackType.IndexOf("ranged") >= 0)
					return SpellType.RangedSpell;
				if (attackType.IndexOf("damage") >= 0)
					return SpellType.DamageSpell;
			}

			if (!string.IsNullOrWhiteSpace(spellDto.saving_throw))  // Some spells, like Ice Knife, can be both a saving throw and a ranged attack.
																															// TODO: Remove enum element SpellType.SavingThrowSpell and make it a boolean property of Spell.
				return SpellType.SavingThrowSpell;

			string dieStr = spellDto.die_str.ToLower();
			if (!string.IsNullOrWhiteSpace(dieStr))
			{
				if (dieStr.StartsWith("^"))
					return SpellType.StartNextTurnSpell;
				if (dieStr.StartsWith("+"))
					return SpellType.HitBonusSpell;
				if (dieStr.IndexOf("healing") >= 0)
					return SpellType.HealingSpell;
				if (dieStr.IndexOf("hpcapacity") >= 0)
					return SpellType.HpCapacitySpell;
				return SpellType.DamageSpell;
			}

			return SpellType.OtherSpell;
		}

		static Ability GetSavingThrowAbility(SpellDto spellDto)
		{
			string savingThrow = spellDto.saving_throw.ToLower();
			if (string.IsNullOrWhiteSpace(savingThrow))
				return Ability.none;
			if (savingThrow == "strength")
				return Ability.strength;
			if (savingThrow == "charisma")
				return Ability.charisma;
			if (savingThrow == "constitution")
				return Ability.constitution;
			if (savingThrow == "dexterity")
				return Ability.dexterity;
			if (savingThrow == "intelligence")
				return Ability.intelligence;
			if (savingThrow == "wisdom")
				return Ability.wisdom;
			return Ability.none;
		}

		public bool MorePowerfulWhenCastAtHigherLevels
		{
			get
			{
				return PerLevelBonus != 0;
			}
		}

		// TODO: Implement CastWith Hue1, Hue2, Bright1, & Bright2. Tie these to actual effects.
		public string CastWith { get; set; }
		public string Hue1 { get; set; }
		public string Bright1 { get; set; }
		public string Hue2 { get; set; }
		public string Bright2 { get; set; }


		public string RangeStr { get; set; }
		public string ComponentsStr { get; set; }
		public string CastingTimeStr { get; set; }
		public string DurationStr { get; set; }
		public SchoolOfMagic SchoolOfMagic { get; set; }

		public void RecalculateDiceAndAmmo(int spellSlotLevel, int spellCasterLevel, int spellcastingAbilityModifier)
		{
			DieStr = GetDieStr(spellSlotLevel, spellCasterLevel, spellcastingAbilityModifier);
			AmmoCount = GetAmmoCount(spellSlotLevel, spellCasterLevel, spellcastingAbilityModifier);
		}
		int GetAmmoCount(int spellSlotLevel, int spellCasterLevel, int spellcastingAbilityModifier)
		{
			if (!GetMultiplier(spellSlotLevel, spellCasterLevel, out double multiplier))
				return OriginalAmmoCount;

			if (!int.TryParse(BonusPerLevel, out int bonusPerLevel))
				return OriginalAmmoCount;

			return OriginalAmmoCount + (int)Math.Floor(multiplier * bonusPerLevel);
		}

		public string DieStrRaw
		{
			get
			{
				return GetSpellRawDieStr();
			}
		}
		

		public static Spell FromDto(SpellDto spellDto, int spellSlotLevel, int spellCasterLevel, int spellcastingAbilityModifier)
		{
			if (spellDto == null)
				return null;

			SpellComponents spellComponents = GetSpellComponents(spellDto);

			const string concentrationHeader = "Concentration, ";
			string spellDuration = spellDto.duration;
			if (spellDuration.StartsWith(concentrationHeader))
				spellDuration = spellDuration.Substring(concentrationHeader.Length).Trim();

			int spellLevel = GetLevel(spellDto.level);
			Spell spell = new Spell()
			{
				CastingTime = GetCastingTime(spellDto.casting_time),
				Components = spellComponents,
				Description = spellDto.description,
				Duration = DndTimeSpan.FromDurationStr(spellDuration),
				Material = spellDto.components_materials_description,
				Level = spellLevel,
				Name = spellDto.name,
				Index = spellDto.index,
				RangeType = GetRangeType(spellDto),
				SpellType = GetSpellType(spellDto),
				SavingThrowAbility = GetSavingThrowAbility(spellDto),
				RequiresConcentration = GetRequiresConcentration(spellDto),
				BonusThreshold = spellDto.bonus_threshold,
				BonusMax = MathUtils.GetInt(spellDto.bonus_max),
				OriginalDieStr = spellDto.die_str,
				BonusPerLevel = spellDto.bonus_per_level,
				PerLevelBonus = spellDto.bonus_per_level.GetFirstDouble(),
				SpellCasterLevel = spellCasterLevel,
				SpellSlotLevel = spellSlotLevel >= 0 ? spellSlotLevel : spellLevel,
				OnCast = spellDto.onCast,
				OnReceived = spellDto.onReceived,
				OnPreparing = spellDto.onPreparing,
				OnPreparationComplete = spellDto.onPreparationComplete,
				OnGetAttackAbility = spellDto.onGetAttackAbility,
				OnPlayerPreparesAttack = spellDto.onPlayerPreparesAttack,
				OnDieRollStopped = spellDto.onDieRollStopped,
				OnPlayerAttacks = spellDto.onPlayerAttacks,
				OnPlayerHitsTarget = spellDto.onPlayerHitsTarget,
				OnDispel = spellDto.onDispel,
				Hue1 = spellDto.hue1,
				Bright1 = spellDto.bright1,
				RangeStr = GetRangeStr(spellDto),
				ComponentsStr = GetComponentsStr(spellComponents, spellDto),
				DurationStr = GetDurationStr(spellDto),
				SchoolOfMagic = DndUtils.ToSchoolOfMagic(spellDto.school),
				CastingTimeStr = GetCastingTimeStr(spellDto),
				Hue2 = spellDto.hue2,
				Bright2 = spellDto.bright2,
				CastWith = spellDto.cast_with,
				AvailableWhen = spellDto.availableWhen
			};

			if (spell.SpellSlotLevel < spellLevel)
				spell.SpellSlotLevel = spellLevel;

			spell.OriginalAmmoCount = spellDto.ammo_count; // Must be set before calculating dice and ammo.
			spell.RecalculateDiceAndAmmo(spell.SpellSlotLevel, spellCasterLevel, spellcastingAbilityModifier);
			spell.Range = GetRange(spellDto, spell.RangeType);
			return spell;
		}
		static string GetRangeStr(SpellDto spellDto)
		{
			return spellDto.range;
		}
		static string GetComponentsStr(SpellComponents spellComponents, SpellDto spellDto)
		{
			string result = "";
			if ((spellComponents & SpellComponents.Verbal) == SpellComponents.Verbal)
				result += "V, ";
			if ((spellComponents & SpellComponents.Somatic) == SpellComponents.Somatic)
				result += "S, ";
			if ((spellComponents & SpellComponents.Material) == SpellComponents.Material)
			{
				result += $"M ";
				if (!string.IsNullOrWhiteSpace(spellDto.components_materials_description))
					result += $"({spellDto.components_materials_description})";
			}
			result = result.Trim(new char[] { ' ', ','});
			return result;
		}

		static string GetDurationStr(SpellDto spellDto)
		{
			return spellDto.duration;
		}

		static string GetCastingTimeStr(SpellDto spellDto)
		{
			return spellDto.casting_time;
		}

		private static SpellComponents GetSpellComponents(SpellDto spellDto)
		{
			SpellComponents spellComponents = SpellComponents.None;
			if (spellDto.components_material)
				spellComponents |= SpellComponents.Material;
			if (spellDto.components_somatic)
				spellComponents |= SpellComponents.Somatic;
			if (spellDto.components_verbal)
				spellComponents |= SpellComponents.Verbal;
			return spellComponents;
		}

		public bool HasRange(int value, SpellRangeType spellRangeType)
		{
			return Range == value && RangeType == spellRangeType;
		}

		public bool MustRollDiceToCast()
		{
			bool hasDieStr = !string.IsNullOrWhiteSpace(DieStr) && (OriginalDieStr == null || !OriginalDieStr.Trim().StartsWith("+"));
			return
				SpellType == SpellType.MeleeSpell ||
				SpellType == SpellType.RangedSpell ||
				SpellType == SpellType.DamageSpell ||
				(SpellType == SpellType.SavingThrowSpell && hasDieStr) ||
				(SpellType == SpellType.HpCapacitySpell && hasDieStr) ||
				(SpellType == SpellType.HealingSpell && hasDieStr);

			//return SpellType == SpellType.MeleeSpell || SpellType == SpellType.RangedSpell || SpellType == SpellType.DamageSpell ||
			//	(SpellType == SpellType.SavingThrowSpell && !string.IsNullOrWhiteSpace(DieStr) && 
			//	(OriginalDieStr == null || !OriginalDieStr.Trim().StartsWith("+")));
		}

		public void TriggerPreparing(Character player, Target target, CastedSpell castedSpell)
		{
			if (player.NeedToBreakBeforeFiringEvent(EventType.SpellEvents, Name)) Debugger.Break();
			Expressions.Do(OnPreparing, player, target, castedSpell);
		}

		public void TriggerPreparationComplete(Character player, Target target, CastedSpell castedSpell)
		{
			if (player.NeedToBreakBeforeFiringEvent(EventType.SpellEvents, Name)) Debugger.Break();
			Expressions.Do(OnPreparationComplete, player, target, castedSpell);
		}

		public void TriggerGetAttackAbility(Character player, Target target, CastedSpell castedSpell)
		{
			if (player.NeedToBreakBeforeFiringEvent(EventType.SpellEvents, Name)) Debugger.Break();
			Expressions.Do(OnGetAttackAbility, player, target, castedSpell);
		}

		public void TriggerCast(Character player, Target target, CastedSpell castedSpell)
		{
			if (player.NeedToBreakBeforeFiringEvent(EventType.SpellEvents, Name)) Debugger.Break();
			Expressions.Do(OnCast, player, target, castedSpell);
		}

		public void TriggerReceived(Character player, Target target, CastedSpell castedSpell)
		{
			if (player.NeedToBreakBeforeFiringEvent(EventType.SpellEvents, Name)) Debugger.Break();
			Expressions.Do(OnReceived, player, target, castedSpell);
		}

		public void TriggerDispel(Character player, Target target, CastedSpell castedSpell)
		{
			if (player.NeedToBreakBeforeFiringEvent(EventType.SpellEvents, Name)) Debugger.Break();
			Expressions.Do(OnDispel, player, target, castedSpell);
		}

		public void TriggerPlayerPreparesAttack(Character player, Target target, CastedSpell castedSpell)
		{
			if (player.NeedToBreakBeforeFiringEvent(EventType.SpellEvents, Name)) Debugger.Break();
			Expressions.Do(OnPlayerPreparesAttack, player, target, castedSpell);
		}

		public void TriggerDieRollStopped(Character player, Target target, CastedSpell castedSpell, DiceStoppedRollingData dice)
		{
			if (player.NeedToBreakBeforeFiringEvent(EventType.SpellEvents, Name)) Debugger.Break();
			Expressions.Do(OnDieRollStopped, player, target, castedSpell, dice);
		}

		public void TriggerPlayerAttacks(Character player, Target target, CastedSpell castedSpell)
		{
			if (player.NeedToBreakBeforeFiringEvent(EventType.SpellEvents, Name)) Debugger.Break();
			Expressions.Do(OnPlayerAttacks, player, target, castedSpell);
		}

		public void TriggerPlayerHitsTarget(Character player, Target target, CastedSpell castedSpell)
		{
			if (player.NeedToBreakBeforeFiringEvent(EventType.SpellEvents, Name)) Debugger.Break();
			Expressions.Do(OnPlayerHitsTarget, player, target, castedSpell);
		}
		public Spell Clone(Character player, int spellSlotLevel)
		{
			Spell result = new Spell();
			RecalculateDiceAndAmmo(spellSlotLevel, player.level, player.SpellcastingAbilityModifier);
			result.BonusPerLevel = BonusPerLevel;
			result.BonusThreshold = BonusThreshold;
			result.BonusMax = BonusMax;
			result.CastingTime = CastingTime;
			result.Components = Components;
			result.Description = Description;
			result.DieStr = DieStr;
			result.Duration = Duration;
			result.Level = Level;
			result.Material = Material;
			result.Name = Name;
			result.Index = Index;
			result.OnCast = OnCast;
			result.OnReceived = OnReceived;
			result.OnPreparing = OnPreparing;
			result.OnPreparationComplete = OnPreparationComplete;
			result.OnGetAttackAbility = OnGetAttackAbility;
			result.OnDispel = OnDispel;
			result.OnPlayerPreparesAttack = OnPlayerPreparesAttack;
			result.OnDieRollStopped = OnDieRollStopped;
			result.OnPlayerAttacks = OnPlayerAttacks;
			result.OnPlayerHitsTarget = OnPlayerHitsTarget;
			result.OriginalDieStr = OriginalDieStr;
			result.OriginalAmmoCount = OriginalAmmoCount;
			result.AmmoCount = AmmoCount;
			result.PerLevelBonus = PerLevelBonus;
			result.Range = Range;
			result.RangeType = RangeType;
			result.RequiresConcentration = RequiresConcentration;
			result.SavingThrowAbility = SavingThrowAbility;
			result.AvailableWhen = AvailableWhen;
			result.SpellCasterLevel = SpellCasterLevel;
			result.SpellType = SpellType;
			result.SchoolOfMagic = SchoolOfMagic;
			result.CastingTimeStr = CastingTimeStr;
			result.DurationStr = DurationStr;
			result.RangeStr = RangeStr;
			result.ComponentsStr = ComponentsStr;
			result.Bright1 = Bright1;
			result.Bright2 = Bright2;
			result.Hue1 = Hue1;
			result.Hue2 = Hue2;

			// Override...
			result.OwnerId = player != null ? player.playerID : -1;
			result.SpellSlotLevel = spellSlotLevel;

			result.RecalculateDiceAndAmmo(spellSlotLevel, player.level, player.GetSpellcastingAbilityModifier());

			return result;
		}

		public void TriggerSpellReceived(Character recipient, Spell givenSpell, object data1 = null, object data2 = null, object data3 = null, object data4 = null, object data5 = null, object data6 = null, object data7 = null)
		{
			DndProperty.GlobalSet("data1", data1);
			DndProperty.GlobalSet("data2", data2);
			DndProperty.GlobalSet("data3", data3);
			DndProperty.GlobalSet("data4", data4);
			DndProperty.GlobalSet("data5", data5);
			DndProperty.GlobalSet("data6", data6);
			DndProperty.GlobalSet("data7", data7);
			TriggerReceived(recipient, null, new CastedSpell(givenSpell, recipient));
		}

		public string GetSpellRawDieStr(string filter = null)
		{
			string workStr = DieStr;
			if (workStr.StartsWith("^"))
				workStr = workStr.Substring(1);
			string[] dieStrs = workStr.Split(',');
			List<string> results = new List<string>();
			for (int i = 0; i < dieStrs.Length; i++)
			{
				if (dieStrs[i].Contains("("))
				{
					if (filter == null || dieStrs[i].Contains(filter))
						results.Add(dieStrs[i].EverythingBefore("(").Trim());
				}
				else
					results.Add(dieStrs[i].Trim());
			}
			string result = string.Join(" + ", results);

			return result;
		}
	}
}
