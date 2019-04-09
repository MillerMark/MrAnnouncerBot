using System;
using System.Collections.Generic;
using System.Linq;

namespace DndCore
{
	public class Creature
	{
		public string name = string.Empty;
		public CreatureKinds kind = CreatureKinds.None;
		public CreatureSize creatureSize = CreatureSize.Medium;
		public string raceClass = string.Empty;
		public List<ItemViewModel> equipment = new List<ItemViewModel>();
		public List<CurseBlessingDisease> cursesAndBlessings = new List<CurseBlessingDisease>();
		public List<DamageFilter> damageImmunities = new List<DamageFilter>();
		public List<DamageFilter> damageResistance = new List<DamageFilter>();
		public List<DamageFilter> damageVulnerability = new List<DamageFilter>();
		public Against advantages = Against.none;
		public Against disadvantages = Against.none;
		public Conditions conditionImmunities = Conditions.None;
		public Conditions activeConditions = Conditions.None;
		public int onTurnActions = 1;
		public int offTurnActions = 0;
		public double initiative = 0;

		public double speed = 0;
		public double flyingSpeed = 0;
		public double burrowingSpeed = 0;
		public double swimmingSpeed = 0;
		public double climbingSpeed = 0;
		public double armorClass = 0;
		public Senses senses = Senses.Hearing | Senses.NormalVision | Senses.Smell;
		public Languages languagesUnderstood = Languages.None;
		public Languages languagesSpoken = Languages.None;
		public double telepathyRadius = 0; // feet
		public string alignment = string.Empty;

		public double darkvisionRadius = 0;
		public double blindsightRadius = 0;
		public double tremorSenseRadius = 0;
		public double truesightRadius = 0;

		public double hitPoints = 0;
		public double tempHitPoints = 0;
		public double maxHitPoints = 0;
		public double goldPieces = 0;

		public double strength;
		public double dexterity;
		public double constitution;
		public double intelligence;
		public double wisdom;
		public double charisma;

		public Creature()
		{

		}

		public void AddDamageResistance(DamageType damageType, AttackKind attackKind)
		{
			damageResistance.Add(new DamageFilter(damageType, attackKind));
		}

		public void AddDamageImmunity(DamageType damageType, AttackKind attackKind = AttackKind.Any)
		{
			damageImmunities.Add(new DamageFilter(damageType, attackKind));
		}

		public void AddDamageVulnerability(DamageType damageType, AttackKind attackKind = AttackKind.Any)
		{
			damageVulnerability.Add(new DamageFilter(damageType, attackKind));
		}

		bool FilterCatches(List<DamageFilter> filter, DamageType damageType, AttackKind attackKind)
		{
			foreach (DamageFilter damageFilter in filter)
				if (damageFilter.Matches(damageType, attackKind))
					return true;
			return false;
		}

		public bool IsImmuneTo(Conditions conditions)
		{
			return (conditionImmunities & conditions) == conditions;
		}

		public bool IsImmuneTo(DamageType damageType, AttackKind attackKind)
		{
			return FilterCatches(damageImmunities, damageType, attackKind);
		}

		public bool IsVulnerableTo(DamageType damageType, AttackKind attackKind)
		{
			return FilterCatches(damageVulnerability, damageType, attackKind);
		}

		public bool IsResistantTo(DamageType damageType, AttackKind attackKind)
		{
			return FilterCatches(damageResistance, damageType, attackKind);
		}

		public DamageType LastDamageTaken { get; protected set; }
		public double LastDamagePointsTaken { get; protected set; }

		public void Pack(ItemViewModel item)
		{
			equipment.Add(item);
		}

		public void Unpack(ItemViewModel item, int count = 1)
		{
			int index = equipment.IndexOf(item);
			if (index >= 0)
			{
				ItemViewModel thisItem = equipment[index];
				if (thisItem.count > 0)
					if (count == int.MaxValue)
						thisItem.count = 0;
					else
						thisItem.count -= count;

				if (thisItem.count <= 0)
					equipment.RemoveAt(index);
			}
		}

		public void Equip(ItemViewModel item)
		{
			this.equipment.Add(item);
			item.equipped = true;
		}

		public void TakeDamage(DamageType damageType, AttackKind attackKind, double points)
		{
			if (IsImmuneTo(damageType, attackKind))
			{
				LastDamageTaken = DamageType.None;
				LastDamagePointsTaken = 0;
				return;
			}

			if (IsResistantTo(damageType, attackKind))
				points /= 2;

			LastDamageTaken = damageType;
			LastDamagePointsTaken = points;
			hitPoints -= points;
		}

		public bool HasCondition(Conditions condition)
		{
			return (activeConditions & condition) == condition;
		}

		public bool HasAnyCondition(Conditions conditions)
		{
			return (activeConditions & conditions) != 0;
		}

		public bool IsCapableOfSense(Senses sense)
		{
			return (senses & sense) == sense;
		}

		public bool HasSense(Senses sense)
		{
			if (sense == Senses.None)
				return false;

			if (HasAnyCondition(Conditions.Petrified | Conditions.Unconscious))
				return false;

			if (sense == Senses.NormalVision)
				return IsCapableOfSense(Senses.NormalVision) && !HasCondition(Conditions.Blinded);

			if (sense == Senses.Blindsight)
				return blindsightRadius > 0 && !HasCondition(Conditions.Blinded);

			if (sense == Senses.Darkvision)
				return darkvisionRadius > 0 && !HasCondition(Conditions.Blinded);

			if (sense == Senses.Hearing)
				return IsCapableOfSense(Senses.Hearing) && !HasCondition(Conditions.Deafened);

			if (sense == Senses.Truesight)
				return truesightRadius > 0 && !HasCondition(Conditions.Blinded);

			if (sense == Senses.Tremorsense)
				return tremorSenseRadius > 0;

			if (sense == Senses.Smell)
				return IsCapableOfSense(Senses.Smell);

			return false;
		}

		bool MatchesCreatureKind(CreatureKinds creatureKinds)
		{
			if (creatureKinds == CreatureKinds.None)
				return false;
			return creatureKinds.HasFlag(kind);
		}

		bool MatchesSize(CreatureSize size)
		{
			if (size == CreatureSize.None)
				return false;
			return size.HasFlag(creatureSize);
		}

		public bool CanBeAffectedBy(Attack attack)
		{
			foreach (Damage damage in attack.damages)
			{
				bool canBeAffectedByTheAttack = false;
				if (damage.DamageType == DamageType.Condition && CanBeAffectedBy(damage.Conditions))
					canBeAffectedByTheAttack = true;
				else if (damage.DamageType != DamageType.None && damage.DamageType != DamageType.Condition && !IsImmuneTo(damage.DamageType, damage.AttackKind))
					canBeAffectedByTheAttack = true;

				if (canBeAffectedByTheAttack)
				{
					if (damage.IncludeCreatures != CreatureKinds.None || damage.IncludeTargetSenses != Senses.None || damage.IncludeCreatureSizes != CreatureSize.None)
					{
						bool missesCreatureKind = damage.IncludeCreatures != CreatureKinds.None && !MatchesCreatureKind(damage.IncludeCreatures);
						bool missesSense = damage.IncludeTargetSenses != Senses.None && !HasSense(damage.IncludeTargetSenses);
						bool missesCreatureSize = damage.IncludeCreatureSizes != CreatureSize.None && !MatchesSize(damage.IncludeCreatureSizes);

						bool attackHitsCreature = !(missesCreatureKind || missesSense || missesCreatureSize);

						if (attackHitsCreature)
							return true;
					}
				}
			}

			return false;
		}

		private bool CanBeAffectedBy(Conditions conditions)
		{
			if (conditions == Conditions.None)
				return false;

			return !IsImmuneTo(conditions);
		}

		private int CalculateModifier(double ability)
		{
			return (int)Math.Floor((ability - 10) / 2.0);
		}

		public virtual double GetAttackModifier(Ability modifier)
		{
			switch (modifier)
			{
				case Ability.Strength:
					return CalculateModifier(strength);
				case Ability.Dexterity:
					return CalculateModifier(dexterity);
				case Ability.Constitution:
					return CalculateModifier(constitution);
				case Ability.Intelligence:
					return CalculateModifier(intelligence);
				case Ability.Wisdom:
					return CalculateModifier(wisdom);
				case Ability.Charisma:
					return CalculateModifier(charisma);
			}
			return 0;
		}

		public int GetAttackRoll(int basicRoll, Ability modifier)
		{
			return basicRoll + (int)Math.Floor(GetAttackModifier(modifier));
		}
	}
}