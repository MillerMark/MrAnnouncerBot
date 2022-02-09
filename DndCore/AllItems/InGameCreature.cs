using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using SheetsPersist;

namespace DndCore
{
	[DocumentName("DnD Game")]
	[SheetName("Creatures")]
	public class InGameCreature
	{
		[Column]
		public string Name { get; set; }
		
		[Column]
		public string Kind { get; set; }

		[Column]
		public string TaleSpireId { get; set; }

		[Column]
		public string BloodColor { get; set; }

		[Column("BgHex")]
		public string BackgroundHex { get; set; }

		[Column("FgHex")]
		public string ForegroundHex { get; set; }

		[Column]
		[JsonIgnore]
		public string ImageUrlOverride { get; set; }

		public bool IsSelected { get; set; }

		public Conditions Conditions { get => Creature.AllConditions; set => Creature.ManuallyAddedConditions = value; }

		public bool TurnIsActive { get; set; }

		public string ImageURL
		{
			get
			{
				if (Creature is Monster monster)
					return monster.ImageUrl;
				return string.Empty;
			}
		}

		[Column]
		[JsonIgnore]
		public string ImageCropOverride { get; set; }

		[Column]
		[JsonIgnore]
		public int MaxHitPointsOverride { get; set; }

		[Column("HitPoints")]
		[JsonIgnore]
		public string HitPointsStr { get; set; }

		[Column("TempHitPoints")]
		[JsonIgnore]
		public string TempHitPointsStr { get; set; }

		[Column]
		[Indexer]
		public int Index { get; set; }
		
		[Column]
		public int NumAhems { get; set; }

		[Column]
		public int NumNames { get; set; }

		[Column("IsEnemy")]
		[JsonIgnore]
		public string IsEnemyStr { get; set; }

		public bool IsEnemy
		{
			get
			{
				return IsEnemyStr == "x";
			}
			set
			{
				if (value)
					IsEnemyStr = "x";
			}
		}

		public bool FriendFoeStatusUnknown
		{
			get
			{
				return IsEnemyStr == "?";
			}
			set
			{
				if (value)
					IsEnemyStr = "?";
			}
		}

		public bool IsAlly
		{
			get
			{
				return string.IsNullOrWhiteSpace(IsEnemyStr) || IsEnemyStr == "-";
			}
			set
			{
				if (value)
					IsEnemyStr = "-";
			}
		}

		public static event CreatureHealthChangedEventHandler CreatureHealthChanged;

		void InGameCreatureHealthChanged(double percentChange)
		{
			CreatureHealthChanged?.Invoke(this, new CreatureHealthChangedEventArgs(Creature, percentChange));
		}

		void InGameCreatureHealed(double percentHealthJustGiven)
		{
			InGameCreatureHealthChanged(percentHealthJustGiven);
		}

		void InGameCreatureDamaged(double percentDamageJustInflicted)
		{
			InGameCreatureHealthChanged(-percentDamageJustInflicted);
		}


		double percentDamageJustInflicted;
		// TODO: Handle this in TS and show damage effects.
		public double PercentDamageJustInflicted 
		{ 
			get 
			{ 
				return percentDamageJustInflicted; 
			}
			set
			{
				if (percentDamageJustInflicted == value)
					return;
				percentDamageJustInflicted = value;
				if (percentDamageJustInflicted > 0)
					InGameCreatureDamaged(percentDamageJustInflicted);
			}
		}

		double percentHealthJustGiven;
		// TODO: Handle this in TS and show damage effects.
		public double PercentHealthJustGiven
		{
			get
			{
				return percentHealthJustGiven;
			}
			set
			{
				if (percentHealthJustGiven == value)
					return;
				percentHealthJustGiven = value;
				if (percentHealthJustGiven > 0)
					InGameCreatureHealed(percentHealthJustGiven);
			}
		}


		public double CropX
		{
			get
			{
				if (Creature is Monster monster)
					return monster.ImageCropInfo.X * monster.ImageCropInfo.DpiFactor;
				return 0;
			}
		}
		public double CropY
		{
			get
			{
				if (Creature is Monster monster)
					return monster.ImageCropInfo.Y * monster.ImageCropInfo.DpiFactor;
				return 0;
			}
		}

		public double CropWidth
		{
			get
			{
				if (Creature is Monster monster)
					return monster.ImageCropInfo.Width * monster.ImageCropInfo.DpiFactor;
				return PictureCropInfo.MinWidth;
			}
		}

		public string Alignment
		{
			get
			{
				if (Creature is Monster monster)
					return monster.alignmentStr;
				return string.Empty;
			}
		}

		public double Health
		{
			get
			{
				if (Creature == null)
					return 0;
				return (Creature.HitPoints + Creature.tempHitPoints) / Creature.maxHitPoints;
			}
		}
		

		// TODO: Implement this.
		public bool IsTargeted { get; set; }

		[JsonIgnore]
		public bool OnScreen { get; set; }


		Creature creature;

		[JsonIgnore]
		public Creature Creature
		{
			get
			{
				if (creature == null)
				{
					Monster monster = Monster.Clone(AllMonsters.GetByKind(Kind));
					if (monster != null)
					{
						monster.SetIntId(-Index);
						// TODO: Fix race (or use "Kind") field and use Name for the monster's name.
						if (!string.IsNullOrWhiteSpace(Name))
							monster.Name = Name;
						if (MaxHitPointsOverride > 0)
						{
							monster.maxHitPoints = MaxHitPointsOverride;
						}

						monster.HitPoints = monster.maxHitPoints;
						if (double.TryParse(HitPointsStr, out double hitPoints))
								monster.HitPoints = hitPoints;

						if (double.TryParse(TempHitPointsStr, out double tempHp))
							monster.tempHitPoints = tempHp;

						if (!string.IsNullOrWhiteSpace(ImageUrlOverride))
							monster.ImageUrl = ImageUrlOverride;
						if (!string.IsNullOrWhiteSpace(ImageCropOverride))
							monster.ImageCropInfo = PictureCropInfo.FromStr(ImageCropOverride);

						monster.dieBackColor = BackgroundHex;
						monster.dieFontColor = ForegroundHex;
						monster.taleSpireId = TaleSpireId;
						monster.bloodColor = BloodColor;
						creature = monster;
					}
				}
				return creature;
			}
		}
		
		public double TotalHp => Creature.HitPoints + Creature.tempHitPoints;

		public InGameCreature()
		{

		}
		
		public static InGameCreature FromMonster(Monster monster)
		{
			InGameCreature inGameCreature = new InGameCreature();
			inGameCreature.Kind = monster.Kind;
			return inGameCreature;
		}

		double beforeTotalHp;

		public void StartTakingDamage()
		{
			PercentDamageJustInflicted = 0;
			beforeTotalHp = TotalHp;
		}

		public void FinishTakingDamage()
		{
			double totalDamageTaken = GetTotalDamageTaken();
			if (totalDamageTaken != 0)
				PercentDamageJustInflicted = MathUtils.Clamp(totalDamageTaken / Creature.maxHitPoints, 0, 1);
			UpdateHitPointsStr();
		}

		public double GetTotalDamageTaken()
		{
			return beforeTotalHp - TotalHp;
		}

		public void TakeDamage(DamageType damageType, AttackKind attackKind, int damage)
		{
			PercentDamageJustInflicted = 0;
			double beforeTotalHp = TotalHp;
			Creature.TakeDamage(damageType, attackKind, damage);
			double totalDamageTaken = beforeTotalHp - TotalHp;
			if (totalDamageTaken != 0)
			{
				PercentDamageJustInflicted = MathUtils.Clamp(totalDamageTaken / Creature.maxHitPoints, 0, 1);
			}
			UpdateHitPointsStr();
		}

		void UpdateHitPointsStr()
		{
			if (Creature == null)
			{
				HitPointsStr = string.Empty;
				TempHitPointsStr = string.Empty;
			}
			else
			{
				if (Creature.HitPoints == Creature.maxHitPoints)
					HitPointsStr = "";
				else
					HitPointsStr = Creature.HitPoints.ToString();
				if (Creature.tempHitPoints == 0)
					TempHitPointsStr = string.Empty;
				else
					TempHitPointsStr = Creature.tempHitPoints.ToString();
			}
		}

		public void ChangeHealth(int amount)
		{
			if (amount > 0)
				AddHealth(amount);
			else
				TakeDamage(DamageType.None, AttackKind.Any, -amount);
		}

		private void AddHealth(int amount)
		{
			PercentHealthJustGiven = 0;
			Creature.ChangeHealth(amount);
			PercentHealthJustGiven = MathUtils.Clamp(amount / Creature.maxHitPoints, 0, 1);
			UpdateHitPointsStr();
		}

		public static int GetUniversalIndex(int index)
		{
			return -index;  // In-game creature indices will be negative.
		}

		public static int GetNormalIndexFromUniversal(int index)
		{
			return -index;  // In-game creature indices will be negative.
		}

		public static bool IsIndexToInGameCreature(int index)
		{
			return index < 0;  // In-game creature indices will be negative.
		}

		public bool IsTotallyImmuneToDamage(System.Collections.Generic.Dictionary<DamageType, int> latestDamage, AttackKind attackKind)
		{
			foreach (DamageType damageType in latestDamage.Keys)
			{
				if (!Creature.IsImmuneTo(damageType, attackKind))
					return false;
			}
			return true;
		}

		// TODO: Consider moving down to Creature
		public void ToggleCondition(Conditions conditions)
		{
			if (Conditions.HasFlag(conditions))  // Bit is set.
				Conditions &= ~conditions;  // clear the bit
			else
				Conditions |= conditions;
		}

		public void ClearAllConditions()
		{
			Conditions = Conditions.None;
		}

		public void TakeDamage(DndGame game, Dictionary<DamageType, int> latestDamage, AttackKind attackKind, double multiplier = 1)
		{
			StartTakingDamage();
			foreach (DamageType damageType in latestDamage.Keys)
				TakeSomeDamage(game, damageType, attackKind, (int)Math.Floor(latestDamage[damageType] * multiplier));
			FinishTakingDamage();
		}

		public void TakeHalfDamage(Character player, Dictionary<DamageType, int> latestDamage, AttackKind attackKind)
		{
			StartTakingDamage();
			foreach (DamageType damageType in latestDamage.Keys)
			{
				int damageTaken = DndUtils.HalveValue(latestDamage[damageType]);
				TakeSomeDamage(player?.Game, damageType, attackKind, damageTaken);
			}
			FinishTakingDamage();
		}

		public void TakeSomeDamage(DndGame game, DamageType damageType, AttackKind attackKind, int amount)
		{
			if (amount <= 0)
				return;

			double previousHP = TotalHp;
			Creature.TakeDamage(damageType, attackKind, amount);

			if (game == null)
				return;

			double hpLost = previousHP - TotalHp;
			if (hpLost == 0)
				return;

			string tempHpDetails = string.Empty;
			if (Creature.tempHitPoints > 0)
				tempHpDetails = $" (tempHp: {Creature.tempHitPoints})";

			string message;
			if (hpLost == 1)
				message = $"{Name} just took 1 point of {damageType} damage. HP is now: {Creature.HitPoints}/{Creature.maxHitPoints}{tempHpDetails}";
			else
				message = $"{Name} just took {hpLost} points of {damageType} damage. HP is now: {Creature.HitPoints}/{Creature.maxHitPoints}{tempHpDetails}";

			game.TellDungeonMaster(message);
		}

		public void CreatureRollingSavingThrow()
		{
			Creature.RollingSavingThrowNow();
		}

		public bool SideMatches(WhatSide whatSide)
		{
			if (whatSide.HasFlag(WhatSide.All))
				return true;
			if (whatSide.HasFlag(WhatSide.Friendly) && IsAlly)
				return true;
			if (whatSide.HasFlag(WhatSide.Enemy) && IsEnemy)
				return true;
			if (whatSide.HasFlag(WhatSide.Neutral) && !IsAlly && !IsEnemy)
				return true;
			return false;
		}
	}
}

