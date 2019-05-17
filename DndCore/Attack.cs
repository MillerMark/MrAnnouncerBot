using System;
using System.Linq;
using System.Collections.Generic;
using DndCore.Enums;
using DndCore.CoreClasses;

namespace DndCore
{
	public class Attack
	{

		public List<Damage> damages = new List<Damage>();
		public string description;
		public DndTimeSpan lasts = DndTimeSpan.OneMinute;
		public bool needsRecharging = false;
		public double plusToHit;
		public double rangeMax;
		public double reachRange;
		public RechargeOdds rechargeOdds = RechargeOdds.ZeroInSix;  // Chances out of six that this attack recharges at the start of the creatures turn.
		public DndTimeSpan recharges = DndTimeSpan.Never;
		public List<Damage> successfulSaveDamages = new List<Damage>();
		public int targetLimit = 1;
		public AttackType type;


		public Attack(string name)
		{
			Name = name;
		}

		public Attack(string name, AttackType type, int plusToHit, int reach, int targetLimit = int.MaxValue) : this(name)
		{
			this.type = type;
			this.plusToHit = plusToHit;
			reachRange = reach;
			this.targetLimit = targetLimit;
		}

		public Attack(string name, AttackType type, int plusToHit, int range, int rangeMax, int targetLimit = int.MaxValue) : this(name)
		{
			this.type = type;
			this.plusToHit = plusToHit;
			reachRange = range;
			this.rangeMax = rangeMax;
			this.targetLimit = targetLimit;
		}

		//public Attack AddFilteredCondition(Conditions conditions, int escapeDC, CreatureSize creatureSizeFilter = CreatureSize.Medium, int concurrentTargets = int.MaxValue)
		//{
		//	filteredConditions.Add(new DamageConditions(conditions, creatureSizeFilter, escapeDC, concurrentTargets));
		//	return this;
		//}

		public Damage LastDamage
		{
			get
			{
				if (damages.Count == 0)
					return null;
				return damages.Last();
			}
		}

		public string Name { get; set; }

		public static Attack Area(string name, int range)
		{
			return new Attack(name, AttackType.Area, 0 /* plusToHit */, range);
		}

		public static Attack Melee(string name, int plusToHit, int reach, int targetLimit = int.MaxValue)
		{
			return new Attack(name, AttackType.Melee, plusToHit, reach, targetLimit);
		}

		public static Attack Ranged(string name, int plusToHit, int range, int rangeMax, int targetLimit = int.MaxValue)
		{
			return new Attack(name, AttackType.Range, plusToHit, range, rangeMax, targetLimit);
		}


		public Attack AddCondition(Conditions conditions, int savingThrow, Ability savingThrowAbility, CreatureSize includeCreatureSizes = CreatureSizes.All)
		{
			Damage damage = new Damage(DamageType.Condition, AttackKind.Any, Dice.NoRoll, TimePoint.Immediately, TimePoint.Immediately, conditions, savingThrow, savingThrowAbility);
			damage.Conditions = conditions;
			damage.IncludeCreatureSizes = includeCreatureSizes;
			damages.Add(damage);
			return this;
		}

		public Attack AddDamage(DamageType damageType, string damageRoll, AttackKind attackKind, TimePoint damageHits = TimePoint.Immediately, TimePoint saveOpportunity = TimePoint.None, Conditions conditions = Conditions.None, int savingThrowSuccess = int.MaxValue, Ability savingThrowAbility = Ability.None)
		{
			damages.Add(new Damage(damageType, attackKind, damageRoll, damageHits, saveOpportunity, conditions, savingThrowSuccess, savingThrowAbility));
			return this;
		}

		public Attack AddDuration(DndTimeSpan dndTimeSpan)
		{
			lasts = dndTimeSpan;
			return this;
		}

		public Attack AddGrapple(int savingThrow, CreatureSize includeCreatureSizes = CreatureSizes.All)
		{
			Damage damage = new Damage(DamageType.Condition, AttackKind.Any, Dice.NoRoll, TimePoint.Immediately, TimePoint.Immediately, Conditions.Grappled | Conditions.Restrained, savingThrow, Ability.Strength | Ability.Dexterity);
			damage.IncludeCreatureSizes = includeCreatureSizes;
			damages.Add(damage);
			return this;
		}

		public Attack AddRecharge(RechargeOdds rechargeOdds)
		{
			this.rechargeOdds = rechargeOdds;
			return this;
		}

		public void ApplyDamageTo(Character player)
		{
			foreach (Damage damage in damages)
				damage.ApplyTo(player);
		}

		public DamageResult GetDamage(Creature creature, int savingThrow)
		{
			if (!creature.CanBeAffectedBy(this))
				return DamageResult.None;

			DamageResult damageResult = new DamageResult();

			damageResult.CollectDamages(creature, damages, successfulSaveDamages, savingThrow, creature.creatureSize);

			return damageResult;
		}
	}

	public class DamageResult
	{
		public static readonly DamageResult None = new DamageResult();
		public Conditions conditionsAdded = Conditions.None;
		public DamageType damageTypes = DamageType.None;
		public int hitPointChange = 0;

		public DamageResult()
		{

		}

		public void CollectDamages(Creature creature, List<Damage> mainDamages, List<Damage> successfulSavedDamages, int savingThrow, CreatureSize creatureSize)
		{
			hitPointChange = 0;
			foreach (Damage damage in successfulSavedDamages)
				if (damage.Saves(savingThrow))
				{
					if (damage.Conditions != Conditions.None)
					{
						if (damage.IncludeCreatureSizes.HasFlag(creatureSize))
							conditionsAdded |= damage.Conditions;
					}
					if (damage.DamageType != DamageType.None && !creature.IsImmuneTo(damage.DamageType, damage.AttackKind))
					{
						damageTypes |= damage.DamageType;
						hitPointChange += (int)-Math.Round(damage.GetDamageRoll());
					}
				}

			foreach (Damage damage in mainDamages)
				if (!damage.Saves(savingThrow))
				{
					if (damage.Conditions != Conditions.None)
					{
						if (damage.IncludeCreatureSizes.HasFlag(creatureSize))
						{
							conditionsAdded |= damage.Conditions;
							damageTypes |= DamageType.Condition;
						}
					}
					DamageType damageType = damage.DamageType & ~(DamageType.Condition | DamageType.None);
					if (damageType != DamageType.None && !creature.IsImmuneTo(damageType, damage.AttackKind))
					{
						damageTypes |= damageType;
						hitPointChange += (int)-Math.Round(damage.GetDamageRoll());
					}
				}
		}

		public bool HasCondition(Conditions condition)
		{
			return (conditionsAdded & condition) == condition;
		}
	}
}

