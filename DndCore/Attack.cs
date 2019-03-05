using System;
using System.Collections.Generic;
using System.Linq;

namespace DndCore
{
	public class Attack
	{
		public Conditions conditions;
		public RechargeOdds rechargeOdds = RechargeOdds.ZeroInSix;  // Chances out of six that this attack recharges at the start of the creatures turn.
		public DndTimeSpan recharges = DndTimeSpan.Never;
		public DndTimeSpan lasts = DndTimeSpan.OneMinute;
		public bool needsRecharging = false;
		public string description;
		public double reachRange;
		public double rangeMax;
		public double plusToHit;
		public int targetLimit = 1;
		public List<Damage> damages = new List<Damage>();
		public List<Damage> successfulSaveDamages = new List<Damage>();
		public SavingThrow savingThrow;
		public AttackType type;
		public Senses includeTargetSenses = Senses.None;
		public CreatureSize includeCreatureSizes = CreatureSizes.All;
		public CreatureKinds includeCreatures = CreatureKinds.None;
		
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

		public Attack AddDamage(DamageType damageType, string damageRoll, AttackKind attackKind, TimePoint damageHits = TimePoint.Immediately, TimePoint saveOpportunity = TimePoint.None)
		{
			damages.Add(new Damage(damageType, attackKind, damageRoll, damageHits, saveOpportunity));
			return this;
		}

		public void ApplyDamageTo(Character player)
		{
			foreach (Damage damage in damages)
				damage.ApplyTo(player);
		}

		public static Attack Melee(string name, int plusToHit, int reach, int targetLimit = int.MaxValue)
		{
			return new Attack(name, AttackType.Melee, plusToHit, reach, targetLimit);
		}

		public static Attack Ranged(string name, int plusToHit, int range, int rangeMax, int targetLimit = int.MaxValue)
		{
			return new Attack(name, AttackType.Range, plusToHit, range, rangeMax, targetLimit);
		}

		public static Attack Area(string name, int range)
		{
			return new Attack(name, AttackType.Area, 0 /* plusToHit */, range);
		}

		public Attack AddRecharge(RechargeOdds rechargeOdds)
		{
			this.rechargeOdds = rechargeOdds;
			return this;
		}

		public Attack AddSavingThrow(int success, Ability ability)
		{
			savingThrow = new SavingThrow(success, ability);
			return this;
		}

		//public Attack AddFilteredCondition(Conditions conditions, int escapeDC, CreatureSize creatureSizeFilter = CreatureSize.Medium, int concurrentTargets = int.MaxValue)
		//{
		//	filteredConditions.Add(new DamageConditions(conditions, creatureSizeFilter, escapeDC, concurrentTargets));
		//	return this;
		//}

		public Attack AddConditions(Conditions conditions, CreatureSize creatureSizeFilter = CreatureSizes.All)
		{
			this.conditions = conditions;
			this.includeCreatureSizes = creatureSizeFilter;
			return this;
		}

		public Attack AddDuration(DndTimeSpan dndTimeSpan)
		{
			lasts = dndTimeSpan;
			return this;
		}

		public DamageResult GetDamageResult(Creature creature, int savingThrow)
		{
			if (!creature.CanBeAffectedBy(this))
				return DamageResult.None;

			DamageResult damageResult = new DamageResult();

			if (this.savingThrow != null && this.savingThrow.Saves(savingThrow))
			{
				damageResult.CollectDamages(creature, successfulSaveDamages);
				return damageResult;
			}

			damageResult.CollectDamages(creature, damages);

			if (includeCreatureSizes.HasFlag(creature.creatureSize))
				damageResult.conditionsAdded |= conditions;

			//foreach (DamageConditions damageCondition in filteredConditions)
			//{
			//	if (damageCondition.CreatureSizeFilter.HasFlag(creature.creatureSize) && savingThrow < damageCondition.EscapeDC)
			//		damageResult.conditionsAdded |= damageCondition.Conditions;
			//}

			return damageResult;
		}

		public Attack AddGrapple(int savingThrow, CreatureSize creatureSizeFilter = CreatureSizes.All)
		{
			return AddSavingThrow(savingThrow, Ability.Strength | Ability.Dexterity)
						.AddConditions(Conditions.Grappled | Conditions.Restrained, creatureSizeFilter);
		}

		public void ExcludeCreatureKinds(CreatureKinds creatureKinds)
		{
			const CreatureKinds allCreatureKinds = CreatureKinds.Aberrations |
				CreatureKinds.Beasts |
				CreatureKinds.Celestials |
				CreatureKinds.Constructs |
				CreatureKinds.Dragons |
				CreatureKinds.Elemental |
				CreatureKinds.Fey |
				CreatureKinds.Fiends |
				CreatureKinds.Giants |
				CreatureKinds.Humanoids |
				CreatureKinds.Monstrosities |
				CreatureKinds.Oozes |
				CreatureKinds.Plants |
				CreatureKinds.Undead;

			includeCreatures = allCreatureKinds & ~creatureKinds;
		}

		public string Name { get; set; }
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

		public bool HasCondition(Conditions condition)
		{
			return (conditionsAdded & condition) == condition;
		}

		public void CollectDamages(Creature creature, List<Damage> damages)
		{
			foreach (Damage damage in damages)
				if (damage.DamageType != DamageType.None && !creature.IsImmuneTo(damage.DamageType, damage.AttackKind))
				{
					damageTypes |= damage.DamageType;
					hitPointChange = (int)-Math.Round(damage.GetDamageRoll());
				}
		}
	}
}

