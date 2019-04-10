using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DndCore
{
	public class Creature
	{
		const string STR_Advantage = ".Advantage";
		const string STR_Disadvantage = ".Disadvantage";
		public string name = string.Empty;
		public CreatureKinds kind = CreatureKinds.None;
		public CreatureSize creatureSize = CreatureSize.Medium;
		public string raceClass = string.Empty;

		public ObservableCollection<ItemViewModel> equipment = new ObservableCollection<ItemViewModel>();
		public ObservableCollection<CurseBlessingDisease> cursesAndBlessings = new ObservableCollection<CurseBlessingDisease>();
		public ObservableCollection<DamageFilter> damageImmunities = new ObservableCollection<DamageFilter>();
		public ObservableCollection<DamageFilter> damageResistance = new ObservableCollection<DamageFilter>();
		public ObservableCollection<DamageFilter> damageVulnerability = new ObservableCollection<DamageFilter>();
		public Against advantages = Against.none;
		public Against disadvantages = Against.none;
		public Conditions conditionImmunities = Conditions.None;
		public Conditions activeConditions = Conditions.None;
		public int onTurnActions = 1;
		public int offTurnActions = 0;
		public double initiative = 0;

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

		public double baseStrength;
		public double Strength
		{
			get
			{
				return baseStrength + GetMods(Ability.Strength);
			}
		}
		public double baseSpeed;
		public double Speed
		{
			get
			{
				return baseSpeed + GetMods();
			}
		}

		Dictionary<string, double> calculatedMods = new Dictionary<string, double>();

		public double baseDexterity;

		double GetCalculatedMod(object key)
		{
			string keyStr = key.ToString();
			if (CalculatedMods.ContainsKey(keyStr))
				return CalculatedMods[keyStr];
			return 0;
		}

		double GetMods(Ability ability)
		{
			RecalculateModsIfNecessary();
			return GetCalculatedMod(ability);
		}

		double GetMods([CallerMemberName] string key = null)
		{
			if (key == null)
				return 0d;
			RecalculateModsIfNecessary();
			return GetCalculatedMod(key);
		}


		public double Dexterity
		{
			get
			{
				return baseDexterity + GetMods(Ability.Dexterity);
			}
		}

		public double baseConstitution;
		public double Constitution
		{
			get
			{
				return baseConstitution + GetMods(Ability.Constitution);
			}

		}
		public double baseIntelligence;
		public double Intelligence
		{
			get
			{
				return baseIntelligence + GetMods(Ability.Intelligence);
			}

		}
		public double baseWisdom;
		public double Wisdom
		{
			get
			{
				return baseWisdom + GetMods(Ability.Wisdom);
			}

		}
		public double baseCharisma;
		public double Charisma
		{
			get
			{
				return baseCharisma + GetMods(Ability.Charisma);
			}
		}

		public Creature()
		{
			equipment.CollectionChanged += ItemCollectionChanged;
			cursesAndBlessings.CollectionChanged += ItemCollectionChanged;
			damageImmunities.CollectionChanged += ItemCollectionChanged;
			damageResistance.CollectionChanged += ItemCollectionChanged;
			damageVulnerability.CollectionChanged += ItemCollectionChanged;
		}

		private void ItemCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			needToRecalculateMods = true;
		}

		protected bool needToRecalculateMods;

		void AddMod(string key, double value)
		{
			if (calculatedMods.ContainsKey(key))
				CalculatedMods[key] += value;
			else
				CalculatedMods.Add(key, value);
		}

		void CalculateMods(ModViewModel mod, bool equipped)
		{
			if (mod.RequiresConsumption)
				return;

			if (mod.RequiresEquipped && !equipped)
				return;

			if (!string.IsNullOrWhiteSpace(mod.TargetName) && mod.TargetName != "None")
			{
				if (mod.Offset > 0)
				{
					AddMod(mod.TargetName, mod.Offset);
				}
				else if (mod.Multiplier != 1)
				{
					AddMod(mod.TargetName + ".Multiplier", mod.Multiplier);
				}
				else if (mod.Absolute != 0)
				{
					AddMod(mod.TargetName + ".Absolute", mod.Multiplier);
				}
			}

			if (mod.AddsAdvantage)
			{
				AddMod(mod.VantageSkillFilter.ToString() + STR_Advantage, 1);
			}
			else if (mod.AddsDisadvantage)
			{
				AddMod(mod.VantageSkillFilter.ToString() + STR_Disadvantage, 1);
			}
		}
		void CalculateModsForItem(ItemViewModel itemViewModel)
		{
			foreach (ModViewModel modViewModel in itemViewModel.mods)
				CalculateMods(modViewModel, itemViewModel.equipped);
		}

		void RecalculateModsIfNecessary()
		{
			if (!needToRecalculateMods)
				return;

			needToRecalculateMods = false;

			calculatedMods.Clear();
			foreach (ItemViewModel itemViewModel in equipment)
			{
				CalculateModsForItem(itemViewModel);
			}
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

		bool FilterCatches(ObservableCollection<DamageFilter> filter, DamageType damageType, AttackKind attackKind)
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

		public Dictionary<string, double> CalculatedMods {
			get
			{
				RecalculateModsIfNecessary();
				return calculatedMods;
			}
			set => calculatedMods = value;
		}

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

		public void Unequip(ItemViewModel item)
		{
			ItemViewModel firstItem = equipment.FirstOrDefault(x => x == item);
			if (firstItem == null)
				return;
			firstItem.equipped = false;
			needToRecalculateMods = true;
		}

		public void Equip(ItemViewModel item)
		{
			this.equipment.Add(item);
			item.equipped = true;
			needToRecalculateMods = true;
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
					return CalculateModifier(Strength);
				case Ability.Dexterity:
					return CalculateModifier(Dexterity);
				case Ability.Constitution:
					return CalculateModifier(Constitution);
				case Ability.Intelligence:
					return CalculateModifier(Intelligence);
				case Ability.Wisdom:
					return CalculateModifier(Wisdom);
				case Ability.Charisma:
					return CalculateModifier(Charisma);
			}
			return 0;
		}

		public int GetAttackRoll(int basicRoll, Ability modifier)
		{
			return basicRoll + (int)Math.Floor(GetAttackModifier(modifier));
		}

		public DiceRoll GetSkillCheckDice(Skills skills)
		{
			string baseKey = skills.ToString();
			int vantageCount = 0;
			if (CalculatedMods.ContainsKey(baseKey + STR_Advantage))
			{
				vantageCount++;
			}
			else if (CalculatedMods.ContainsKey(baseKey + STR_Disadvantage))
			{
				vantageCount--;
			}

			if (vantageCount > 0)
				return DiceRoll.Advantage;

			if (vantageCount < 0)
				return DiceRoll.Disadvantage;

			return DiceRoll.Normal;
		}
	}
}