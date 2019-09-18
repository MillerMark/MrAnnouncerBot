using System;
using System.Linq;

namespace DndCore
{
	public class Spell
	{

		public int Range { get; set; }
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
		public string OriginalDieStr { get; set; }
		public string BonusPerLevel { get; set; }
		public double PerLevelBonus { get; set; }
		public string OnCast { get; set; }
		public string OnCasting { get; set; }
		public string OnPlayerAttacks { get; set; }
		public string OnPlayerHitsTarget { get; set; }
		public string OnDispel { get; set; }



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
			string bonusThreshold = BonusThreshold.ToLower();
			
			DieRollDetails details = DieRollDetails.From(OriginalDieStr, spellcastingAbilityModifier);

			string dieStr = details.ToString();

			if (string.IsNullOrWhiteSpace(bonusThreshold))
			{
				return dieStr;
			}

			int compareLevel = spellSlotLevel;
			if (bonusThreshold.StartsWith("c"))
				compareLevel = spellCasterLevel;

			int minThreshold = bonusThreshold.GetFirstInt();
			double multiplier = compareLevel - minThreshold;

			string bonusPerLevelStr = BonusPerLevel;
			if (PerLevelBonus == 0)
				return dieStr;

			int totalBonus = (int)Math.Floor(PerLevelBonus * multiplier);
			if (totalBonus <= 0)
				return dieStr;

			DieRollDetails bonusDetails = DieRollDetails.From(bonusPerLevelStr, spellcastingAbilityModifier);

			foreach (Roll bonusRoll in bonusDetails.Rolls)
			{
				Roll matchingRoll = details.GetRoll(bonusRoll.Sides);
				if (matchingRoll == null)
					details.AddRoll(bonusRoll.ToString());
				else if (multiplier != 0)
				{
					matchingRoll.Count += (int)Math.Floor(bonusRoll.Count * multiplier);
					matchingRoll.Offset += (int)Math.Floor(bonusRoll.Offset * multiplier);
				}
			}

			return details.ToString();
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

		public void RecalculateDieStr(int spellSlotLevel, int spellCasterLevel, int spellcastingAbilityModifier)
		{
			DieStr = GetDieStr(spellSlotLevel, spellCasterLevel, spellcastingAbilityModifier);
		}

		public static Spell FromDto(SpellDto spellDto, int spellSlotLevel, int spellCasterLevel, int spellcastingAbilityModifier)
		{
			SpellComponents spellComponents = GetSpellComponents(spellDto);

			const string concentrationHeader = "Concentration, ";
			string spellDuration = spellDto.duration;
			if (spellDuration.StartsWith(concentrationHeader))
				spellDuration = spellDuration.Substring(concentrationHeader.Length).Trim();

			Spell spell = new Spell()
			{
				CastingTime = GetCastingTime(spellDto.casting_time),
				Components = spellComponents,
				Description = spellDto.description,
				Duration = DndTimeSpan.FromDurationStr(spellDuration),
				Material = spellDto.components_materials_description,
				Level = GetLevel(spellDto.level),
				Name = spellDto.name,
				RangeType = GetRangeType(spellDto),
				SpellType = GetSpellType(spellDto),
				SavingThrowAbility = GetSavingThrowAbility(spellDto),
				RequiresConcentration = GetRequiresConcentration(spellDto),
				BonusThreshold = spellDto.bonus_threshold,
				OriginalDieStr = spellDto.die_str,
				BonusPerLevel = spellDto.bonus_per_level,
				PerLevelBonus = spellDto.bonus_per_level.GetFirstDouble(),
				SpellCasterLevel = spellCasterLevel,
				SpellSlotLevel = spellSlotLevel,
				OnCast = spellDto.onCast,
				OnCasting = spellDto.onCasting,
				OnPlayerAttacks = spellDto.onPlayerAttacks,
				OnPlayerHitsTarget = spellDto.onPlayerHitsTarget,
				OnDispel = spellDto.onDispel
			};
			spell.RecalculateDieStr(spellSlotLevel, spellCasterLevel, spellcastingAbilityModifier);
			spell.Range = GetRange(spellDto, spell.RangeType);
			return spell;
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
			return SpellType == SpellType.MeleeSpell || SpellType == SpellType.RangedSpell;
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
	}
}
