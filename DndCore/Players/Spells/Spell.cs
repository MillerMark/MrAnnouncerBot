using System;
using System.Linq;

namespace DndCore
{
	[HasDndEvents]
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
		public string Name { get; set; }
		public string Description { get; set; }
		public SpellComponents Components { get; set; }
		public string Material { get; set; }
		public int OwnerId { get; set; }
		public bool RequiresConcentration { get; set; }
		public int Level { get; set; }  // 0 == cantrip
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
		public string OnCast { get; set; }
		[DndEvent]
		public string OnCasting { get; set; }
		[DndEvent]
		public string OnPlayerAttacks { get; set; }
		[DndEvent]
		public string OnPlayerHitsTarget { get; set; }
		[DndEvent]
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
			if (!string.IsNullOrWhiteSpace(spellDto.saving_throw))
				return SpellType.SavingThrowSpell;
			if (!string.IsNullOrWhiteSpace(spellDto.attack_type))
			{
				string attackType = spellDto.attack_type.ToLower();
				if (attackType.IndexOf("melee") >= 0)
					return SpellType.MeleeSpell;
				if (attackType.IndexOf("ranged") >= 0)
					return SpellType.RangedSpell;
			}

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
				string workStr = DieStr;
				if (workStr.StartsWith("^"))
					workStr = workStr.Substring(1);
				string[] dieStrs = workStr.Split(',');
				for (int i = 0; i < dieStrs.Length; i++)
				{
					if (dieStrs[i].Contains("("))
						dieStrs[i] = dieStrs[i].EverythingBefore("(").Trim();
					else
						dieStrs[i] = dieStrs[i].Trim();
				}
				string result = string.Join(" + ", dieStrs);

 				return result;
			}
		}
		

		public static Spell FromDto(SpellDto spellDto, int spellSlotLevel, int spellCasterLevel, int spellcastingAbilityModifier)
		{
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
				OnCasting = spellDto.onCasting,
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
			return SpellType == SpellType.MeleeSpell || SpellType == SpellType.RangedSpell || 
				(SpellType == SpellType.SavingThrowSpell && !string.IsNullOrWhiteSpace(DieStr) && 
				(OriginalDieStr == null || !OriginalDieStr.Trim().StartsWith("+")));
		}

		public void TriggerOnCasting(Character player, Creature targetCreature, CastedSpell castedSpell)
		{
			Expressions.Do(OnCasting, player, targetCreature, castedSpell);
		}

		public void TriggerCast(Character player, Creature targetCreature, CastedSpell castedSpell)
		{
			Expressions.Do(OnCast, player, targetCreature, castedSpell);
		}

		public void TriggerDispel(Character player, Creature targetCreature, CastedSpell castedSpell)
		{
			Expressions.Do(OnDispel, player, targetCreature, castedSpell);
		}

		public void TriggerPlayerAttacks(Character player, Creature target, CastedSpell castedSpell)
		{
			Expressions.Do(OnPlayerAttacks, player, target, castedSpell);
		}

		public void TriggerPlayerHitsTarget(Character player, Creature target, CastedSpell castedSpell)
		{
			Expressions.Do(OnPlayerHitsTarget, player, target, castedSpell);
		}
		public Spell Clone(Character player, int spellSlotLevel)
		{
			Spell result = new Spell();
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
			result.OnCast = OnCast;
			result.OnCasting = OnCasting;
			result.OnDispel = OnDispel;
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
	}
}
