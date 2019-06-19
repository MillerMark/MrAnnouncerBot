using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace DndCore
{
	public class Creature
	{
		const string STR_Absolute = ".Absolute";
		const string STR_AddAbilityModifier = ".AddAbilityModifier";
		const string STR_Advantage = ".Advantage";
		const string STR_Disadvantage = ".Disadvantage";
		const string STR_LimitAbilityModifier = ".LimitAbilityModifier";
		const string STR_Multiplier = ".Multiplier";

		public Conditions activeConditions = Conditions.None;
		public Against advantages = Against.none;
		public string alignment = string.Empty;

		public List<Attack> attacks = new List<Attack>();
		public double baseArmorClass = 0;
		public double tempArmorClassMod = 0;
		public double baseCharisma;

		public double baseConstitution;

		public double baseDexterity;
		public double baseIntelligence;
		public double baseSpeed;

		public double baseStrength;
		public double baseWisdom;
		public double blindsightRadius = 0;
		public double burrowingSpeed = 0;

		Dictionary<string, double> calculatedMods = new Dictionary<string, double>();
		public double climbingSpeed = 0;
		public Conditions conditionImmunities = Conditions.None;
		public CreatureSize creatureSize = CreatureSize.Medium;
		public ObservableCollection<CurseBlessingDisease> cursesAndBlessings = new ObservableCollection<CurseBlessingDisease>();
		public ObservableCollection<DamageFilter> damageImmunities = new ObservableCollection<DamageFilter>();
		public ObservableCollection<DamageFilter> damageResistance = new ObservableCollection<DamageFilter>();
		public ObservableCollection<DamageFilter> damageVulnerability = new ObservableCollection<DamageFilter>();

		public double darkvisionRadius = 0;
		public Against disadvantages = Against.none;
		public ObservableCollection<ItemViewModel> equipment = new ObservableCollection<ItemViewModel>();

		public double flyingSpeed = 0;
		public double goldPieces = 0;

		public double hitPoints = 0;
		public double initiative = 0;
		public CreatureKinds kind = CreatureKinds.None;
		public Languages languagesSpoken = Languages.None;
		public Languages languagesUnderstood = Languages.None;
		public double maxHitPoints = 0;
		public List<Attack> multiAttack = new List<Attack>();
		public MultiAttackCount multiAttackCount = MultiAttackCount.oneEach;
		public string name = string.Empty;

		protected bool needToRecalculateMods;
		public int offTurnActions = 0;
		public int onTurnActions = 1;
		public string raceClass = string.Empty;
		public Senses senses = Senses.Hearing | Senses.NormalVision | Senses.Smell;
		public double swimmingSpeed = 0;
		public double telepathyRadius = 0; // feet
		public double tempHitPoints = 0;
		public double tremorSenseRadius = 0;
		public double truesightRadius = 0;

		public Creature()
		{
			equipment.CollectionChanged += ItemCollectionChanged;
			cursesAndBlessings.CollectionChanged += ItemCollectionChanged;
			damageImmunities.CollectionChanged += ItemCollectionChanged;
			damageResistance.CollectionChanged += ItemCollectionChanged;
			damageVulnerability.CollectionChanged += ItemCollectionChanged;
		}

		public double ArmorClass
		{
			get
			{
				double absolute = GetAbsolute();
				double thisArmorClass = baseArmorClass;
				if (absolute > 0)
					thisArmorClass = absolute;
				return thisArmorClass + tempArmorClassMod + GetMods();
			}
		}

		public Dictionary<string, double> CalculatedMods
		{
			get
			{
				RecalculateModsIfNecessary();
				return calculatedMods;
			}
			set => calculatedMods = value;
		}
		public double Charisma
		{
			get
			{
				return baseCharisma + GetMods(Ability.Charisma);
			}
		}
		public double Constitution
		{
			get
			{
				return baseConstitution + GetMods(Ability.Constitution);
			}

		}


		public double Dexterity
		{
			get
			{
				return baseDexterity + GetMods(Ability.Dexterity);
			}
		}
		public DndGame Game { get; set; }
		public double Intelligence
		{
			get
			{
				return baseIntelligence + GetMods(Ability.Intelligence);
			}

		}
		public double LastDamagePointsTaken { get; protected set; }

		public DamageType LastDamageTaken { get; protected set; }
		public double Speed
		{
			get
			{
				return baseSpeed + GetMods();
			}
		}
		public double Strength
		{
			get
			{
				return baseStrength + GetMods(Ability.Strength);
			}
		}
		public double Wisdom
		{
			get
			{
				return baseWisdom + GetMods(Ability.Wisdom);
			}

		}

		public void AddDamageImmunity(DamageType damageType, AttackKind attackKind = AttackKind.Any)
		{
			damageImmunities.Add(new DamageFilter(damageType, attackKind));
		}

		public void AddDamageResistance(DamageType damageType, AttackKind attackKind)
		{
			damageResistance.Add(new DamageFilter(damageType, attackKind));
		}

		public void AddDamageVulnerability(DamageType damageType, AttackKind attackKind = AttackKind.Any)
		{
			damageVulnerability.Add(new DamageFilter(damageType, attackKind));
		}

		void AddMod(string key, double value)
		{
			if (calculatedMods.ContainsKey(key))
				CalculatedMods[key] += value;
			else
				CalculatedMods.Add(key, value);
		}

		int CalculateAbilityModifier(double abilityScore)
		{
			return (int)Math.Floor((abilityScore - 10) / 2.0);
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
					AddMod(mod.TargetName + STR_Multiplier, mod.Multiplier);
				}
				else if (mod.Absolute != 0)
				{
					AddMod(mod.TargetName + STR_Absolute, mod.Absolute);
				}

				if (mod.AddAbilityModifier != Ability.None)
				{
					AddMod(mod.TargetName + STR_AddAbilityModifier, (int)mod.AddAbilityModifier);
					AddMod(mod.TargetName + STR_LimitAbilityModifier, mod.ModifierLimit);
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

		bool CanBeAffectedBy(Conditions conditions)
		{
			if (conditions == Conditions.None)
				return false;

			return !IsImmuneTo(conditions);
		}

		public void Equip(ItemViewModel item)
		{
			ItemViewModel existingItem = equipment.FirstOrDefault(x => x == item);
			if (existingItem == null)
				equipment.Add(item);
			item.equipped = true;
			needToRecalculateMods = true;
		}

		bool FilterCatches(ObservableCollection<DamageFilter> filter, DamageType damageType, AttackKind attackKind)
		{
			foreach (DamageFilter damageFilter in filter)
				if (damageFilter.Matches(damageType, attackKind))
					return true;
			return false;
		}

		double GetAbsolute([CallerMemberName] string key = null)
		{
			if (key == null)
				return 0d;
			RecalculateModsIfNecessary();
			return GetAbsoluteMod(key);
		}

		double GetAbsoluteMod(object key)
		{
			string keyStr = key.ToString() + STR_Absolute;
			if (CalculatedMods.ContainsKey(keyStr))
				return CalculatedMods[keyStr];
			return 0;
		}

		public virtual double GetAttackModifier(Ability modifier)
		{
			switch (modifier)
			{
				case Ability.Strength:
					return CalculateAbilityModifier(Strength);
				case Ability.Dexterity:
					return CalculateAbilityModifier(Dexterity);
				case Ability.Constitution:
					return CalculateAbilityModifier(Constitution);
				case Ability.Intelligence:
					return CalculateAbilityModifier(Intelligence);
				case Ability.Wisdom:
					return CalculateAbilityModifier(Wisdom);
				case Ability.Charisma:
					return CalculateAbilityModifier(Charisma);
			}
			return 0;
		}

		public int GetAttackRoll(int basicRoll, Ability modifier)
		{
			return basicRoll + (int)Math.Floor(GetAttackModifier(modifier));
		}

		double GetCalculatedMod(object key)
		{
			string keyStr = key.ToString();
			double abilityModifier = 0;

			if (CalculatedMods.ContainsKey(keyStr + STR_AddAbilityModifier))
			{
				int abilityIndex = (int)CalculatedMods[keyStr + STR_AddAbilityModifier];
				Ability ability = (Ability)abilityIndex;
				switch (ability)
				{
					case Ability.Strength:
						abilityModifier = CalculateAbilityModifier(Strength);
						break;
					case Ability.Dexterity:
						abilityModifier = CalculateAbilityModifier(Dexterity);
						break;
					case Ability.Constitution:
						abilityModifier = CalculateAbilityModifier(Constitution);
						break;
					case Ability.Intelligence:
						abilityModifier = CalculateAbilityModifier(Intelligence);
						break;
					case Ability.Wisdom:
						abilityModifier = CalculateAbilityModifier(Wisdom);
						break;
					case Ability.Charisma:
						abilityModifier = CalculateAbilityModifier(Charisma);
						break;
				}
				double abilityModifierLimit = 0;
				if (CalculatedMods.ContainsKey(keyStr + STR_LimitAbilityModifier))
				{
					abilityModifierLimit = CalculatedMods[keyStr + STR_LimitAbilityModifier];
				}
				if (abilityModifierLimit != 0 && abilityModifier > abilityModifierLimit)
					abilityModifier = abilityModifierLimit;
			}
			if (CalculatedMods.ContainsKey(keyStr))
				return CalculatedMods[keyStr] + abilityModifier;
			return abilityModifier;
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

		public DiceRollKind GetSkillCheckDice(Skills skills)
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
				return DiceRollKind.Advantage;

			if (vantageCount < 0)
				return DiceRollKind.Disadvantage;

			return DiceRollKind.Normal;
		}

		public bool HasAnyCondition(Conditions conditions)
		{
			return (activeConditions & conditions) != 0;
		}

		public bool HasCondition(Conditions condition)
		{
			return (activeConditions & condition) == condition;
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

		public bool IsCapableOfSense(Senses sense)
		{
			return (senses & sense) == sense;
		}

		public bool IsImmuneTo(Conditions conditions)
		{
			return (conditionImmunities & conditions) == conditions;
		}

		public bool IsImmuneTo(DamageType damageType, AttackKind attackKind)
		{
			return FilterCatches(damageImmunities, damageType, attackKind);
		}

		public bool IsResistantTo(DamageType damageType, AttackKind attackKind)
		{
			return FilterCatches(damageResistance, damageType, attackKind);
		}

		public bool IsVulnerableTo(DamageType damageType, AttackKind attackKind)
		{
			return FilterCatches(damageVulnerability, damageType, attackKind);
		}

		void ItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			needToRecalculateMods = true;
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

		public void Pack(ItemViewModel item)
		{
			equipment.Add(item);
		}
		public void QueueAction(ActionAttack actionAttack)
		{
			Game.QueueAction(this, actionAttack);
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

		public void Unequip(ItemViewModel item)
		{
			ItemViewModel firstItem = equipment.FirstOrDefault(x => x == item);
			if (firstItem == null)
				return;
			firstItem.equipped = false;
			needToRecalculateMods = true;
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
	}
}