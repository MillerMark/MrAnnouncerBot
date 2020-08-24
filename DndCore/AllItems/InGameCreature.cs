using System;
using System.Linq;
using Newtonsoft.Json;
using GoogleHelper;

namespace DndCore
{
	[SheetName("DnD Game")]
	[TabName("Creatures")]
	public class InGameCreature
	{
		[Column]
		public string Name { get; set; }
		
		[Column]
		public string Kind { get; set; }
		
		[Column]
		[JsonIgnore]
		public string ImageUrlOverride { get; set; }

		public bool IsTalking { get; set; }

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


		// TODO: Handle this in TS and show damage effects.
		public double PercentDamageJustInflicted { get; set; }
		public double PercentHealthJustGiven { get; set; }


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

		public void TakeSomeDamage(DamageType damageType, AttackKind attackKind, int damage)
		{
			Creature.TakeDamage(damageType, attackKind, damage);
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
	}
}

