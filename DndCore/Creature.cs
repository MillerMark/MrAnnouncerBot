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
		public List<Item> equipment = new List<Item>();
		public List<CurseBlessingDisease> cursesAndBlessings = new List<CurseBlessingDisease>();
		public List<DamageFilter> damageImmunities = new List<DamageFilter>();
		public List<DamageFilter> damageResistance = new List<DamageFilter>();
		public List<DamageFilter> damageVulnerability = new List<DamageFilter>();
		public Against advantages = Against.none;
		public Against disadvantages = Against.none;
		public Conditions conditionImmunities = Conditions.none;
		public Conditions activeConditions = Conditions.none;
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
			damageResistance.Add(new DamageFilter(damageType, attackKind, FilterType.Include));
		}

		public void AddDamageImmunity(DamageType damageType, AttackKind attackKind = AttackKind.Any)
		{
			damageImmunities.Add(new DamageFilter(damageType, attackKind, FilterType.Include));
		}

		public void AddDamageVulnerability(DamageType damageType, AttackKind attackKind = AttackKind.Any)
		{
			damageVulnerability.Add(new DamageFilter(damageType, attackKind, FilterType.Include));
		}

		bool FilterCatches(List<DamageFilter> filter, DamageType damageType, AttackKind attackKind)
		{
			foreach (DamageFilter damageFilter in filter)
				if (damageFilter.Catches(damageType, attackKind))
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

		public void Pack(Item item)
		{
			equipment.Add(item);
		}

		public void Unpack(Item item, int count = 1)
		{
			int index = equipment.IndexOf(item);
			if (index >= 0)
			{
				Item thisItem = equipment[index];
				if (thisItem.count > 0)
					if (count == int.MaxValue)
						thisItem.count = 0;
					else
						thisItem.count -= count;

				if (thisItem.count <= 0)
					equipment.RemoveAt(index);
			}
		}

		public void Equip(Item item)
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
			if (HasAnyCondition(Conditions.petrified | Conditions.unconscious))
				return false;

			if (sense == Senses.NormalVision)
				return IsCapableOfSense(Senses.NormalVision) && !HasCondition(Conditions.blinded);

			if (sense == Senses.Blindsight)
				return blindsightRadius > 0 && !HasCondition(Conditions.blinded);

			if (sense == Senses.Darkvision)
				return darkvisionRadius > 0 && !HasCondition(Conditions.blinded);

			if (sense == Senses.Hearing)
				return IsCapableOfSense(Senses.Hearing) && !HasCondition(Conditions.deafened);

			if (sense == Senses.Truesight)
				return truesightRadius > 0 && !HasCondition(Conditions.blinded);

			if (sense == Senses.Tremorsense)
				return tremorSenseRadius > 0;

			if (sense == Senses.Smell)
				return IsCapableOfSense(Senses.Smell);

			return false;
		}

		bool MatchesCreature(CreatureKinds creatureKinds)
		{
			if (creatureKinds == CreatureKinds.None)
				return false;
			return (creatureKinds & kind) == kind;
		}

		public bool CanBeAffectedBy(Attack attack)
		{
			bool canBeAffectedByTheAttack = CanBeAffectedBy(attack.conditions);
			foreach (Damage damage in attack.damages)
				if (damage.DamageType != DamageType.None && !IsImmuneTo(damage.DamageType, damage.AttackKind))
				{
					canBeAffectedByTheAttack = true;
					break;
				}
			if (!canBeAffectedByTheAttack)
				return false;

			// Check to see if the attack excludes us...
			if (attack.includeCreatures != CreatureKinds.None || attack.excludeCreatures != CreatureKinds.None || attack.includeTargetSenses != Senses.None || attack.excludeTargetSenses != Senses.None)
			{
				bool isNotIncludedCreature = attack.includeCreatures != CreatureKinds.None && !MatchesCreature(attack.includeCreatures);
				bool isExcludedCreature = attack.excludeCreatures != CreatureKinds.None && MatchesCreature(attack.excludeCreatures);

				bool missingTargetedSense = attack.includeTargetSenses != Senses.None && !HasSense(attack.includeTargetSenses);
				bool hasExcludedSense = attack.excludeTargetSenses != Senses.None && HasSense(attack.excludeTargetSenses);

				return !(isExcludedCreature || hasExcludedSense || isNotIncludedCreature || missingTargetedSense);
			}

			return true;
		}

		private bool CanBeAffectedBy(Conditions conditions)
		{
			if (conditions == Conditions.none)
				return false;

			return !IsImmuneTo(conditions);
		}

	}
}