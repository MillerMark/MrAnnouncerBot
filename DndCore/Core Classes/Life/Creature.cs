using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using GoogleHelper;

namespace DndCore
{
	public abstract class Creature
	{
		string _dieBackColor = "#ffffff";
		string _dieFontColor = "#000000";
		public virtual string dieBackColor { get => _dieBackColor; set => _dieBackColor = value; }
		public virtual string dieFontColor { get => _dieFontColor; set => _dieFontColor = value; }

		public abstract int GetSpellcastingAbilityModifier();
		[JsonIgnore]
		public abstract int Level { get; }
		public abstract int IntId { get; }

		[JsonIgnore]
		public int SafeId
		{
			get
			{
				if (this is Character)
					return Math.Abs(IntId);
				return -Math.Abs(IntId);  // In game creatures all have negative creature indices.
			}
		}
		

		public event MessageEventHandler RequestMessageToDungeonMaster;

		public event ConditionsChangedEventHandler ConditionsChanged;
		protected virtual void OnConditionsChanged(object sender, ConditionsChangedEventArgs ea)
		{
			ConditionsChanged?.Invoke(sender, ea);
		}

		const string STR_Absolute = ".Absolute";
		const string STR_AddAbilityModifier = ".AddAbilityModifier";
		const string STR_Advantage = ".Advantage";
		const string STR_Disadvantage = ".Disadvantage";
		const string STR_LimitAbilityModifier = ".LimitAbilityModifier";
		const string STR_Multiplier = ".Multiplier";

		private Conditions activeConditions { get; set; } = Conditions.None;
		public Against advantages { get; set; } = Against.none;
		public string alignmentStr = string.Empty;
		public Alignment Alignment { get; set; }
		public string race { get; set; }

		[JsonIgnore]
		public List<Attack> attacks = new List<Attack>();

		[JsonIgnore]
		public Vector Location { get; protected set; }

		public void SetLocation(Vector location)
		{
			Location = location;
		}

		public double baseArmorClass = 0;
		public double tempArmorClassMod = 0;
		public double baseCharisma;

		public double baseConstitution;

		public double baseDexterity;
		public double baseIntelligence;

		public double baseStrength;
		public double baseWisdom;

		// TODO: May want to make this private after removing dependencies to this variable name due to JSON serialization in TS files.
		public double baseWalkingSpeed = 0;
		public double burrowingSpeed { get; set; } = 0;

		[JsonIgnore]
		Dictionary<string, double> calculatedMods = new Dictionary<string, double>();
		public double climbingSpeed = 0;
		public Conditions conditionImmunities = Conditions.None;
		public CreatureSize creatureSize = CreatureSize.Medium;

		[JsonIgnore]
		public ObservableCollection<CurseBlessingDisease> cursesAndBlessings = new ObservableCollection<CurseBlessingDisease>();
		[JsonIgnore]
		public ObservableCollection<DamageFilter> damageImmunities = new ObservableCollection<DamageFilter>();
		[JsonIgnore]
		public ObservableCollection<DamageFilter> damageResistance = new ObservableCollection<DamageFilter>();
		[JsonIgnore]
		public ObservableCollection<DamageFilter> damageVulnerability = new ObservableCollection<DamageFilter>();

		public virtual double darkvisionRadius { get; set; } = 0;
		public virtual double tremorSenseRadius { get; set; } = 0;
		public virtual double truesightRadius { get; set; } = 0;
		public virtual double blindsightRadius { get; set; } = 0;

		[JsonIgnore]
		public CastedSpell concentratedSpell;

		public Against disadvantages { get; set; } = Against.none;
		public ObservableCollection<ItemViewModel> equipment = new ObservableCollection<ItemViewModel>();

		public double flyingSpeed { get; set; } = 0;

		[Column]
		public decimal goldPieces = 0;

		public decimal GoldPieces
		{
			get { return goldPieces; }
			set
			{
				goldPieces = value;
			}
		}


		[Column]
		public double hitPoints = 0;


		public double HitPoints
		{
			get { return hitPoints; }
			set
			{
				hitPoints = value;
			}
		}


		public double initiative = 0;
		public CreatureKinds kind = CreatureKinds.None;
		public Languages languagesSpoken = DndCore.Languages.None;
		public Languages languagesUnderstood = DndCore.Languages.None;
		public double maxHitPoints { get; set; } = 0;

		[JsonIgnore]
		public List<Attack> multiAttack = new List<Attack>();

		[JsonIgnore]
		public MultiAttackCount multiAttackCount = MultiAttackCount.oneEach;

		public string name = string.Empty;

		[JsonIgnore]
		public string Name { get => name; set => name = value; }


		[JsonIgnore]
		public bool IsSelected { get; set; }

		protected bool needToRecalculateMods;
		public int offTurnActions = 0;
		public int onTurnActions = 1;

		public Senses senses { get; set; } = Senses.Hearing | Senses.NormalVision | Senses.Smell;
		public double swimmingSpeed { get; set; } = 0;
		public double telepathyRadius { get; set; } = 0; // feet
		public double tempHitPoints = 0;

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

		[JsonIgnore]
		public Dictionary<string, Dictionary<string, PropertyMod>> PropertyMods { get; set; } = new Dictionary<string, Dictionary<string, PropertyMod>>();

		[JsonIgnore]
		public Dictionary<string, VantageMod> VantageMods { get; set; } = new Dictionary<string, VantageMod>();

		public virtual double Strength
		{
			get
			{
				return baseStrength + GetMods(Ability.strength);
			}
		}
		public virtual double Wisdom
		{
			get
			{
				return baseWisdom + GetMods(Ability.wisdom);
			}

		}


		public virtual double Charisma
		{
			get
			{
				return baseCharisma + GetMods(Ability.charisma);
			}
		}
		public virtual double Constitution
		{
			get
			{
				return baseConstitution + GetMods(Ability.constitution);
			}

		}


		public virtual double Dexterity
		{
			get
			{
				return baseDexterity + GetMods(Ability.dexterity);
			}
		}

		[JsonIgnore]
		public DndGame Game { get; set; }

		public double Intelligence
		{
			get
			{
				return baseIntelligence + GetMods(Ability.intelligence);
			}

		}
		public double LastDamagePointsTaken { get; protected set; }

		public DamageType LastDamageTaken { get; protected set; }
		public double WalkingSpeed
		{
			get
			{
				return CalculateProperty(baseWalkingSpeed);
			}
			set
			{
				baseWalkingSpeed = GetBasePropertyValue(value);
			}
		}

		//    [ ] Tooltip to return an explanation of mods (one per line)
		//        "-10 due to Ray of Frost (expires at end of Merkin's next turn)."
		string GetModExplanation(string propertyName)
		{
			return string.Empty;
		}

		public Conditions ActiveConditions
		{
			get => activeConditions;
			set
			{
				if (activeConditions == value)
					return;

				Conditions oldConditions = activeConditions;

				activeConditions = value;

				OnConditionsChanged(this, new ConditionsChangedEventArgs(oldConditions, activeConditions));
			}
		}

		[JsonIgnore]
		public Target ActiveTarget { get; set; }

		[JsonIgnore]
		public int SpellcastingAbilityModifier
		{
			get
			{
				return GetSpellcastingAbilityModifier();
			}
			set
			{
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

		void AddCalculatedMod(string key, double value)
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
					AddCalculatedMod(mod.TargetName, mod.Offset);
				}
				else if (mod.Multiplier != 1)
				{
					AddCalculatedMod(mod.TargetName + STR_Multiplier, mod.Multiplier);
				}
				else if (mod.Absolute != 0)
				{
					AddCalculatedMod(mod.TargetName + STR_Absolute, mod.Absolute);
				}

				if (mod.AddAbilityModifier != Ability.none)
				{
					AddCalculatedMod(mod.TargetName + STR_AddAbilityModifier, (int)mod.AddAbilityModifier);
					AddCalculatedMod(mod.TargetName + STR_LimitAbilityModifier, mod.ModifierLimit);
				}
			}

			if (mod.AddsAdvantage)
			{
				AddCalculatedMod(mod.VantageSkillFilter.ToString() + STR_Advantage, 1);
			}
			else if (mod.AddsDisadvantage)
			{
				AddCalculatedMod(mod.VantageSkillFilter.ToString() + STR_Disadvantage, 1);
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

		public virtual double GetAbilityModifier(Ability modifier)
		{
			switch (modifier)
			{
				case Ability.strength: return CalculateAbilityModifier(Strength);
				case Ability.dexterity: return CalculateAbilityModifier(Dexterity);
				case Ability.constitution: return CalculateAbilityModifier(Constitution);
				case Ability.intelligence: return CalculateAbilityModifier(Intelligence);
				case Ability.wisdom: return CalculateAbilityModifier(Wisdom);
				case Ability.charisma: return CalculateAbilityModifier(Charisma);
			}
			return 0;
		}

		public int GetAttackRoll(int basicRoll, Ability modifier)
		{
			return basicRoll + (int)Math.Floor(GetAbilityModifier(modifier));
		}

		double GetCalculatedMod(object key)
		{
			string keyStr = key.ToString();
			if (key is Ability)
			{
				keyStr = keyStr.InitialCap();
			}
			double abilityModifier = 0;

			if (CalculatedMods.ContainsKey(keyStr + STR_AddAbilityModifier))
			{
				Ability ability = (Ability)(int)CalculatedMods[keyStr + STR_AddAbilityModifier];
				abilityModifier = GetAbilityModifier(ability);
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

		public double GetMods(Ability ability)
		{
			RecalculateModsIfNecessary();
			return GetCalculatedMod(ability);
		}

		double GetPropertyModMultiplier(string key)
		{
			if (!PropertyMods.ContainsKey(key))
				return 1;

			double result = 1;
			foreach (string id in PropertyMods[key].Keys)
				result *= PropertyMods[key][id].Multiplier;

			return result;
		}

		double GetPropertyModOffset(string key)
		{
			if (!PropertyMods.ContainsKey(key))
				return 0;

			double result = 0;
			foreach (string id in PropertyMods[key].Keys)
				result += PropertyMods[key][id].Offset;

			return result;
		}

		double CalculateProperty(double baseValue, double min = 0, double max = double.MaxValue, [CallerMemberName] string key = null)
		{
			return MathUtils.Clamp(GetPropertyModMultiplier(key) * (baseValue + GetPropertyModOffset(key) + GetMods(key)), min, max);
		}

		double GetBasePropertyValue(double setToValue, [CallerMemberName] string key = null)
		{
			return setToValue / GetPropertyModMultiplier(key) - (GetPropertyModOffset(key) + GetMods(key));
		}

		double GetMods([CallerMemberName] string key = null)
		{
			if (key == null)
				return 0d;
			RecalculateModsIfNecessary();
			return GetCalculatedMod(key);
		}

		public VantageKind GetSkillCheckDice(Skills skills)
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
				return VantageKind.Advantage;

			if (vantageCount < 0)
				return VantageKind.Disadvantage;

			return VantageKind.Normal;
		}

		public bool HasAnyCondition(Conditions conditions)
		{
			return (ActiveConditions & conditions) != 0;
		}

		public bool HasCondition(Conditions condition)
		{
			return (ActiveConditions & condition) == condition;
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
			double totalDamageTaken = points;

			if (IsResistantTo(damageType, attackKind))
				totalDamageTaken = DndUtils.HalveValue(totalDamageTaken);
			else if (IsVulnerableTo(damageType, attackKind))
				totalDamageTaken *= 2;

			LastDamageTaken = damageType;

			if (totalDamageTaken > HitPoints + tempHitPoints)  // Can only drop to zero HP
				totalDamageTaken = HitPoints + tempHitPoints;

			LastDamagePointsTaken = totalDamageTaken;

			if (totalDamageTaken == 0)
				return;

			double damageToInflict = totalDamageTaken;
			if (tempHitPoints > 0)
			{
				if (damageToInflict > tempHitPoints)
				{
					damageToInflict -= tempHitPoints;
					tempHitPoints = 0;
				}
				else
				{
					tempHitPoints -= damageToInflict;
					damageToInflict = 0;
				}
			}

			HitPoints -= damageToInflict;
			OnDamaged(totalDamageTaken);
		}

		public event StateChangedEventHandler StateChanged;
		public event CreatureDamagedEventHandler Damaged;

		protected virtual void OnDamaged(double damageAmount)
		{
			CreatureDamagedEventArgs ea = new CreatureDamagedEventArgs(this, damageAmount);
			Damaged?.Invoke(this, ea);
		}

		protected virtual void OnStateChanged(object sender, StateChangedEventArgs ea)
		{
			StateChanged?.Invoke(sender, ea);
		}

		protected void OnStateChanged(string key, object oldValue, object newValue, bool isRechargeable = false)
		{
			OnStateChanged(this, new StateChangedEventArgs(key, oldValue, newValue, isRechargeable));
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

		public void DieRollStoppedForTestCases(int score, int damage = 0)
		{
			DiceStoppedRollingData diceStoppedRollingData = new DiceStoppedRollingData();
			diceStoppedRollingData.damage = damage;
			Game.DieRollStopped(this, score, diceStoppedRollingData);
		}

		public virtual void ReadyRollDice(DiceRollType rollType, string diceStr, int hiddenThreshold = int.MinValue)
		{
			if (Game != null)
				Game.SetHiddenThreshold(this, hiddenThreshold, rollType);
		}

		public virtual void PrepareWeaponAttack(PlayerActionShortcut playerActionShortcut)
		{

		}

		public virtual Attack GetAttack(string attackName)
		{
			return null;
		}

		public void PrepareAttack(Creature creature, Attack attack)
		{
			if (Game != null)
			{
				Game.CreatureTakingAction(this);
				Game.CreaturePreparesAttack(this, creature, attack, false);
			}
		}

		public void PrepareAttack(Creature target, PlayerActionShortcut shortcut)
		{
			if (Game != null)
				Game.CreatureTakingAction(this);
			PrepareWeaponAttack(shortcut);
			if (Game != null)
				Game.CreaturePreparesAttack(this, target, null, shortcut.UsesMagic);
		}

		public virtual void Target(Creature target)
		{
			if (Game != null)
			{
				Game.CreatureTakingAction(this);
				Game.CreaturePreparesAttack(this, target, null, false);
			}
		}

		public virtual void StartTurnResetState()
		{
		}

		public virtual void EndTurnResetState()
		{
		}

		void AddCastedMagic(Magic magic)
		{
			castedMagic.Add(magic);
			magic.Dispel += CastedMagic_Dispelled;
		}

		[JsonIgnore]
		List<Magic> receivedMagic = new List<Magic>();

		[JsonIgnore]
		List<Magic> castedMagic = new List<Magic>();

		void ReceiveMagic(Magic magic, int delayOffset = 0)
		{
			SystemVariables.Offset = delayOffset;
			magic.AddTarget(this);
			receivedMagic.Add(magic);
			magic.Dispel += ReceivedMagic_Dispelled;
			magic.TriggerOnReceived(this);
		}

		public void GiveMagic(string magicItemName, CastedSpell castedSpell, Target target, object data1, object data2, object data3, object data4, object data5, object data6, object data7, object data8)
		{
			Magic magic = new Magic(this, Game, magicItemName, castedSpell, data1, data2, data3, data4, data5, data6, data7, data8);
			if (target != null)
			{
				const int timeBetweenGiftNotification = 150;
				int delayOffset = 0;
				if (target.PlayerIds != null)
					foreach (int playerId in target.PlayerIds)
					{
						Character player = Game.GetPlayerFromId(playerId);
						if (player != null)
						{
							player.ReceiveMagic(magic, delayOffset);
							delayOffset += timeBetweenGiftNotification;
						}
					}
				if (target.Creatures != null)
					foreach (Creature creature in target.Creatures)
						if (creature != null)
						{
							creature.ReceiveMagic(magic, delayOffset);
							delayOffset += timeBetweenGiftNotification;
						}
			}

			AddCastedMagic(magic);
		}

		void RemoveReceivedMagic(Magic magic)
		{
			magic.Dispel -= ReceivedMagic_Dispelled;
			receivedMagic.Remove(magic);
		}

		void RemoveCastedMagic(Magic magic)
		{
			magic.Dispel -= CastedMagic_Dispelled;
			castedMagic.Remove(magic);
		}

		void ReceivedMagic_Dispelled(object sender, MagicEventArgs ea)
		{
			RemoveReceivedMagic(ea.Magic);
		}

		void CastedMagic_Dispelled(object sender, MagicEventArgs ea)
		{
			RemoveCastedMagic(ea.Magic);
		}

		public void AddPropertyMod(string propertyName, string id, double offset, double multiplier)
		{
			if (!PropertyMods.ContainsKey(propertyName))
				PropertyMods.Add(propertyName, new Dictionary<string, PropertyMod>());
			Dictionary<string, PropertyMod> propMods = PropertyMods[propertyName];
			if (!propMods.ContainsKey(id))
				propMods.Add(id, new PropertyMod(offset, multiplier));
			else
				propMods[id].Set(offset, multiplier);
		}

		public void RemovePropertyMod(string propertyName, string id)
		{
			if (!PropertyMods.ContainsKey(propertyName))
				return;
			Dictionary<string, PropertyMod> propMods = PropertyMods[propertyName];
			if (!propMods.ContainsKey(id))
				return;
			propMods.Remove(id);
		}

		public void AddVantageMod(string id, DiceRollType rollType, Skills skills, string dieLabel, int offset)
		{
			if (!VantageMods.ContainsKey(id))
				VantageMods.Add(id, new VantageMod(rollType, skills, dieLabel, offset));
			else
				VantageMods[id].Set(rollType, skills, dieLabel, offset);
		}

		protected virtual void OnRequestMessageToDungeonMaster(string message)
		{
			RequestMessageToDungeonMaster?.Invoke(this, new MessageEventArgs(message));
		}

		public VantageKind GetVantage(DiceRollType type, Ability savingThrow, Skills skillCheck = Skills.none, VantageKind initialVantage = VantageKind.Normal)
		{
			int advantageCount = 0;
			int disadvantageCount = 0;
			switch (initialVantage)
			{
				case VantageKind.Advantage:
					advantageCount = 1;
					break;
				case VantageKind.Disadvantage:
					disadvantageCount = 1;
					break;
			}

			foreach (string key in VantageMods.Keys)
			{
				if (VantageMods[key].RollType == type)
				{
					switch (type)
					{
						case DiceRollType.SkillCheck:
							Skills matchSkill = VantageMods[key].Detail;
							if ((DndUtils.IsSkillAGenericAbility(matchSkill) && matchSkill == DndUtils.FromSkillToAbility(skillCheck)) ||
								matchSkill == skillCheck)
							{
								if (VantageMods[key].Offset < 0)
								{
									OnRequestMessageToDungeonMaster($"{Name} has disadvantage on {matchSkill} skill checks due to {VantageMods[key].DieLabel}.");
									disadvantageCount++;
								}
								else if (VantageMods[key].Offset > 0)
								{
									OnRequestMessageToDungeonMaster($"{Name} has advantage on {matchSkill} skill checks due to {VantageMods[key].DieLabel}.");
									advantageCount++;
								}
							}
							break;
						// TODO: Implement vantage mods for attacks, initiative, etc.
						case DiceRollType.Attack:
						case DiceRollType.ChaosBolt:

							break;
						case DiceRollType.SavingThrow:
						case DiceRollType.OnlyTargetsSavingThrow:
						case DiceRollType.DamagePlusSavingThrow:
							if ((int)VantageMods[key].Detail == (int)savingThrow)
							{
								if (VantageMods[key].Offset < 0)
								{
									OnRequestMessageToDungeonMaster($"{Name} has disadvantage on {savingThrow} saving throws due to {VantageMods[key].DieLabel}.");
									disadvantageCount++;
								}
								else if (VantageMods[key].Offset > 0)
								{
									OnRequestMessageToDungeonMaster($"{Name} has advantage on {savingThrow} saving throws due to {VantageMods[key].DieLabel}.");
									advantageCount++;
								}
							}
							break;
						case DiceRollType.DeathSavingThrow:

							break;
						case DiceRollType.Initiative:

							break;
						case DiceRollType.NonCombatInitiative:

							break;
					}
				}
			}
			advantageCount = (int)MathUtils.Clamp(advantageCount, 0, 1);
			disadvantageCount = (int)MathUtils.Clamp(disadvantageCount, 0, 1);
			if (advantageCount == disadvantageCount)
				return VantageKind.Normal;
			else if (advantageCount > 0)
				return VantageKind.Advantage;
			else if (disadvantageCount > 0)
				return VantageKind.Disadvantage;

			return VantageKind.Normal;
		}

		public void RemoveVantageMod(string id)
		{
			if (!VantageMods.ContainsKey(id))
				return;
			VantageMods.Remove(id);
		}

		public virtual void ChangeTempHP(double deltaTempHp)
		{
			tempHitPoints += deltaTempHp;
			if (tempHitPoints < 0)
				tempHitPoints = 0;
		}

		public void ChangeHealth(double damageHealthAmount)
		{
			if (damageHealthAmount < 0)
				TakeDamage(DamageType.None, AttackKind.Any, -damageHealthAmount);
			else
				Heal(damageHealthAmount);
		}

		public void Heal(double deltaHealth)
		{
			if (deltaHealth <= 0)
				return;

			if (HitPoints >= maxHitPoints)
				return;

			double maxToHeal = maxHitPoints - HitPoints;
			if (deltaHealth > maxToHeal)
				HitPoints = maxHitPoints;
			else
				HitPoints += deltaHealth;
		}

		public abstract double GetSavingThrowModifier(Ability savingThrowAbility);

		void GetDamageAndAttackType(string damageStr, out DamageType damageType, out AttackKind attackKind)
		{
			attackKind = AttackKind.Any;
			if (damageStr.ToLower().StartsWith("and "))
				damageStr = damageStr.Substring(4);
			int spaceIndex = damageStr.IndexOf(' ');
			if (spaceIndex > 0)
			{
				string remainingStr = damageStr.Substring(spaceIndex).Trim();
				if (remainingStr.ToLower().Contains("nonmagical") || remainingStr.ToLower().Contains("non-magical"))
					attackKind = AttackKind.NonMagical;
				else if (remainingStr.ToLower().Contains("magical"))
					attackKind = AttackKind.Magical;
				damageStr = damageStr.Substring(0, spaceIndex);
			}
			damageType = DndUtils.ToDamage(damageStr);
		}

		public void SetDamageImmunities(string damageImmunities)
		{
			if (string.IsNullOrWhiteSpace(damageImmunities))
				return;

			damageImmunities = damageImmunities.Replace(';', ',');
			string[] parts = damageImmunities.Split(',');
			foreach (string part in parts)
			{
				GetDamageAndAttackType(part.Trim(), out DamageType damage, out AttackKind attackKind);
				if (damage != DamageType.None)
					AddDamageImmunity(damage, attackKind);
			}
		}

		public void SetDamageResistances(string damageResistances)
		{
			if (string.IsNullOrWhiteSpace(damageResistances))
				return;

			damageResistances = damageResistances.Replace(';', ',');
			string[] parts = damageResistances.Split(',');
			foreach (string part in parts)
			{
				GetDamageAndAttackType(part.Trim(), out DamageType damage, out AttackKind attackKind);
				if (damage != DamageType.None)
					AddDamageResistance(damage, attackKind);
			}
		}

		public void SetConditionImmunities(string conditionImmunities)
		{
			//if (!string.IsNullOrWhiteSpace(conditionImmunities))
			//{
			//	System.Diagnostics.Debugger.Break();
			//}
		}

		public void SetDamageVulnerabilities(string damageVulnerabilities)
		{
			if (string.IsNullOrWhiteSpace(damageVulnerabilities))
				return;

			damageVulnerabilities = damageVulnerabilities.Replace(';', ',');
			string[] parts = damageVulnerabilities.Split(',');
			foreach (string part in parts)
			{
				GetDamageAndAttackType(part.Trim(), out DamageType damage, out AttackKind attackKind);
				if (damage != DamageType.None)
					AddDamageVulnerability(damage, attackKind);
			}
		}
		public event SpellChangedEventHandler ConcentratedSpellChanged;

		protected virtual void OnConcentratedSpellChanged(object sender, SpellChangedEventArgs e)
		{
			ConcentratedSpellChanged?.Invoke(sender, e);
		}

		public void BreakConcentration()
		{
			if (concentratedSpell == null)
				return;

			if (concentratedSpell.Active)
			{
				Game.Dispel(concentratedSpell);
				concentratedSpell.Dispel();
			}
			OnConcentratedSpellChanged(this, new SpellChangedEventArgs(this, concentratedSpell.Spell.Name, SpellState.BrokeConcentration));
			concentratedSpell = null;
		}

		public void CastingSpellRequiringConcentration(CastedSpell spell)
		{
			if (concentratedSpell?.Spell.Name == spell?.Spell.Name)
				return;
			BreakConcentration();
			spell.Active = true;
			concentratedSpell = spell;
			OnConcentratedSpellChanged(this, new SpellChangedEventArgs(this, concentratedSpell.Spell.Name, SpellState.JustCast));
		}

		public void CheckConcentration(CastedSpell castedSpell)
		{
			if (castedSpell.Spell.RequiresConcentration)
				CastingSpellRequiringConcentration(castedSpell);
		}

		[JsonIgnore]
		public ActiveSpellData spellPrepared;

		[JsonIgnore]
		public ActiveSpellData spellActivelyCasting;

		[JsonIgnore]
		public ActiveSpellData spellPreviouslyCasting;

		public bool forceShowSpell = false;

		public void ShowPlayerCasting(CastedSpell castedSpell)
		{
			forceShowSpell = true;
			spellPreviouslyCasting = null;
			spellPrepared = null;
			spellActivelyCasting = ActiveSpellData.FromCastedSpell(castedSpell);
			OnStateChanged(this, new StateChangedEventArgs("spellActivelyCasting", null, null));
		}

		public void PrepareSpell(CastedSpell castedSpell)
		{
			forceShowSpell = true;
			spellPreviouslyCasting = null;
			spellActivelyCasting = null;
			spellPrepared = ActiveSpellData.FromCastedSpell(castedSpell);
			OnStateChanged(this, new StateChangedEventArgs("spellPrepared", null, null));
		}

		[JsonIgnore]
		public List<EventCategory> eventCategories = new List<EventCategory>();
		EventData FindEvent(EventType eventType, string parentName, string eventName)
		{
			foreach (EventCategory eventCategory in eventCategories)
			{
				EventData foundEvent = eventCategory.FindEvent(eventType, parentName, eventName);
				if (foundEvent != null)
					return foundEvent;
			}
			return null;
		}
		public bool NeedToBreakBeforeFiringEvent(EventType eventType, string parentName, [CallerMemberName] string eventName = "")
		{
			const string TriggerMethodPrefix = "Trigger";
			if (!eventName.StartsWith(TriggerMethodPrefix))
				return false;
			string realEventName = "On" + eventName.Substring(TriggerMethodPrefix.Length);

			EventData foundEvent = FindEvent(eventType, parentName, realEventName);
			if (foundEvent == null)
				return false;

			return foundEvent.BreakAtStart;
		}

		protected bool evaluatingExpression;

		public void StartingExpressionEvaluation()
		{
			evaluatingExpression = true;
		}

		[JsonIgnore]
		public Queue<SpellEffect> additionalSpellCastEffects = new Queue<SpellEffect>();

		public void AddSpellCastEffect(string effectName = "",
			int hue = 0, int saturation = 100, int brightness = 100,
			double scale = 1, double rotation = 0, double autoRotation = 0, int timeOffset = 0,
			int secondaryHue = 0, int secondarySaturation = 100, int secondaryBrightness = 100, int xOffset = 0, int yOffset = 0, double velocityX = 0, double velocityY = 0)
		{
			additionalSpellCastEffects.Enqueue(new SpellEffect(effectName, hue, saturation, brightness,
				scale, rotation, autoRotation, timeOffset,
				secondaryHue, secondarySaturation, secondaryBrightness, xOffset, yOffset, velocityX, velocityY));
		}

		public int hueShift = 0;

		[JsonIgnore]
		public Queue<SpellEffect> additionalSpellHitEffects = new Queue<SpellEffect>();
		public void AddSpellHitEffect(string effectName = "",
			int hue = 0, int saturation = 100, int brightness = 100,
			double scale = 1, double rotation = 0, double autoRotation = 0, int timeOffset = 0,
			int secondaryHue = 0, int secondarySaturation = 100, int secondaryBrightness = 100, int xOffset = 0, int yOffset = 0, double velocityX = 0, double velocityY = 0)
		{
			additionalSpellHitEffects.Enqueue(new SpellEffect(effectName, hue, saturation, brightness,
				scale, rotation, autoRotation, timeOffset,
				secondaryHue, secondarySaturation, secondaryBrightness, xOffset, yOffset, velocityX, velocityY));
		}

		public void Dispel(CastedSpell castedSpell)
		{
			if (Game != null)
				Game.Dispel(castedSpell);
			if (concentratedSpell?.Spell?.Name == castedSpell?.Spell?.Name)
			{
				OnConcentratedSpellChanged(this, new SpellChangedEventArgs(this, concentratedSpell.Spell.Name, SpellState.JustDispelled));
				concentratedSpell = null;
			}
		}

		[JsonIgnore]
		protected Dictionary<string, object> states = new Dictionary<string, object>();

		public bool HoldsState(string key)
		{
			return states.ContainsKey(key);
		}
		public event PlayerShowStateEventHandler PlayerShowState;

		protected virtual void OnPlayerShowState(object sender, PlayerShowStateEventArgs ea)
		{
			PlayerShowState?.Invoke(sender, ea);
		}

		public void ShowState(string message, string fillColor, string outlineColor, int delayMs = 0)
		{
			OnPlayerShowState(this, new PlayerShowStateEventArgs(this, message, fillColor, outlineColor, delayMs));
		}

		public Ability spellCastingAbility = Ability.none;

		protected const string STR_RechargeableMaxSuffix = "_max";


		[JsonIgnore]
		public List<Rechargeable> rechargeables = new List<Rechargeable>();

		public void SetState(string key, object newValue)
		{
			if (states.ContainsKey(key))
			{
				if (states[key] == newValue)
					return;
				object oldState = states[key];
				states[key] = newValue;
				OnStateChanged(key, oldState, newValue);
			}
			else
			{
				Rechargeable rechargeable = rechargeables.FirstOrDefault(x => x.VarName == key);
				if (rechargeable != null)
				{
					int newIntValue = (int)newValue;
					if (newIntValue == rechargeable.ChargesUsed)
						return;
					int oldValue = rechargeable.ChargesUsed;
					rechargeable.ChargesUsed = newIntValue;
					OnStateChanged(key, oldValue, newValue, true);
					return;
				}

				rechargeable = rechargeables.FirstOrDefault(x => x.VarName == key + STR_RechargeableMaxSuffix);
				if (rechargeable != null)
				{
					int newIntValue = (int)newValue;
					if (newIntValue == rechargeable.TotalCharges)
						return;
					int oldValue = rechargeable.TotalCharges;
					rechargeable.TotalCharges = newIntValue;
					OnStateChanged(key + STR_RechargeableMaxSuffix, oldValue, newValue, false); // max value is not considered a rechargeable, as the max value does not change and has no corresponding UI key.
					return;
				}

				states.Add(key, newValue);
				OnStateChanged(key, null, newValue);
			}
		}

		public object GetState(string key)
		{
			if (states.ContainsKey(key))
				return states[key];
			Rechargeable rechargeable = rechargeables.FirstOrDefault(x => x.VarName == key);
			if (rechargeable != null)
				return rechargeable.ChargesUsed;

			rechargeable = rechargeables.FirstOrDefault(x => x.VarName + STR_RechargeableMaxSuffix == key);
			if (rechargeable != null)
				return rechargeable.TotalCharges;

			return null;
		}
		public void SetNextAnswer(string answer)
		{
			NextAnswer = answer;
		}

		[JsonIgnore]
		public string NextAnswer { get; set; }
		public string diceWeAreRolling = string.Empty;
		public event RollDiceEventHandler RollDiceRequest;

		protected virtual void OnRollDiceRequest(object sender, RollDiceEventArgs ea)
		{
			RollDiceRequest?.Invoke(sender, ea);
		}

		public void RollDiceNow()
		{
			OnRollDiceRequest(this, new RollDiceEventArgs(diceWeAreRolling));
		}

		public void RemoveStateVar(string varName)
		{
			if (states.ContainsKey(varName))
				states.Remove(varName);
		}

		public void ReplaceDamageDice(string diceStr)
		{
			overrideReplaceDamageDice = diceStr;
		}

		[JsonIgnore]
		protected bool reapplyingActiveFeatures;

		[JsonIgnore]
		protected RecalcOptions queuedRecalcOptions = RecalcOptions.None;

		public void Recalculate(RecalcOptions recalcOptions)
		{
			//if (recalcOptions == RecalcOptions.None)
			//	return;
			if (reapplyingActiveFeatures || evaluatingExpression || Expressions.IsUpdating)
			{
				queuedRecalcOptions |= recalcOptions;
				return;
			}

			if ((recalcOptions & RecalcOptions.TurnBasedState) == RecalcOptions.TurnBasedState)
			{
				StartTurnResetState();
			}
			else if ((recalcOptions & RecalcOptions.ActionBasedState) == RecalcOptions.ActionBasedState)
			{
				ResetPlayerActionBasedState();
			}

			if ((recalcOptions & RecalcOptions.Resistance) == RecalcOptions.Resistance)
			{
				ResetPlayerResistance();
			}

			// ReapplyActiveFeatures(false);  // Causes perf issue in characters with features that call Recalculate(...)
		}

		[JsonIgnore]
		public string overrideReplaceDamageDice { get; set; } = string.Empty;
		public int disadvantageDiceThisRoll = 0;

		public void GiveDisadvantageThisRoll()
		{
			disadvantageDiceThisRoll++;
		}

		[JsonIgnore]
		public int advantageDiceThisRoll = 0;

		public void GiveAdvantageThisRoll()
		{
			advantageDiceThisRoll++;
		}

		[JsonIgnore]
		public double attackingAbilityModifierBonusThisRoll = 0;

		[JsonIgnore]
		public Ability attackingAbility = Ability.none;

		[JsonIgnore]
		public double attackingAbilityModifier = 0; // TODO: Set for spells?

		[JsonIgnore]
		public AttackKind attackingKind = AttackKind.Any;

		[JsonIgnore]
		public AttackType attackingType = AttackType.None;

		[JsonIgnore]
		public int attackOffsetThisRoll = 0;

		[JsonIgnore]
		public int damageOffsetThisRoll = 0;
		public bool targetThisRollIsCreature = false;

		public void ResetPlayerActionBasedState()
		{
			ActiveTarget = null;
			attackingAbility = Ability.none;
			attackingAbilityModifier = 0;
			attackingType = AttackType.None;
			attackingKind = AttackKind.Any;
			diceWeAreRolling = string.Empty;
			targetThisRollIsCreature = false;
			damageOffsetThisRoll = 0;
			attackOffsetThisRoll = 0;
			advantageDiceThisRoll = 0;
			attackingAbilityModifierBonusThisRoll = 0;  // This is only assigned from the expressions evaluator.
			disadvantageDiceThisRoll = 0;
		}

		void ResetPlayerResistance()
		{
			// TODO: implement this.
			//damageResistance
		}

		public void CompletingExpressionEvaluation()
		{
			evaluatingExpression = false;
			Recalculate(queuedRecalcOptions);
			queuedRecalcOptions = RecalcOptions.None;
		}


		[JsonIgnore]
		protected CastedSpell spellToCast;

		public void CompleteCast()
		{
			CastedSpell spellToComplete = spellToCast;
			if (spellToComplete == null && !Game.PlayerIsCastingSpell(concentratedSpell, playerID))
				spellToComplete = concentratedSpell;
			if (spellToComplete == null)
				return;

			if (Game != null)
				Game.CompleteCast(this, spellToComplete);

			spellToCast = null;
		}
		public int playerID { get; set; }

		public void AboutToCompleteCast()
		{

		}
		public string trailingEffectsThisRoll = string.Empty;

		public void AddTrailingEffects(string trailingEffects)
		{
			if (!string.IsNullOrWhiteSpace(trailingEffectsThisRoll))
				trailingEffectsThisRoll += ";";
			trailingEffectsThisRoll += trailingEffects;
		}

		protected int GetIntState(string key)
		{
			object result = GetState(key);
			if (result == null)
				return 0;
			return (int)result;
		}

		public void UseSpellSlot(int spellSlotLevel)
		{
			string key = DndUtils.GetSpellSlotLevelKey(spellSlotLevel);
			SetState(key, GetIntState(key) + 1);
		}

		/* 
		 * You automatically revert if you fall Unconscious, drop to 0 Hit Points, or die.
		 * Replaced:
		 *	Senses
		 *	Strength, Dexterity, Constitution
		 *	Speed stats (unless you are a water genasi - in which case you retain your swim speed)
		 *	
		 *	Hit Points (and Hit Dice). 
		 *	   When you revert to your normal form, you return to the number of Hit Points you had before you transformed.
		 *     However, if you revert as a result of Dropping to 0 Hit Points, any excess damage carries over to your normal form. 
		 *     For example, if you take 10 damage in animal form and have only 1 hit point left, you revert and take 9 damage. 
		 *     As long as the excess damage doesn't reduce your normal form to 0 Hit Points, you aren't knocked Unconscious.
	 
		 * Use the highest:
		 *	skill and saving throw Proficiency
		 
		 * You can't cast Spells.
		 * 
		 */
		public event PickWeaponEventHandler PickWeapon;
		protected virtual void OnPickWeapon(object sender, PickWeaponEventArgs ea)
		{
			PickWeapon?.Invoke(sender, ea);
		}

		public Target ChooseWeapon(string weaponFilter = null)
		{
			PickWeaponEventArgs pickWeaponEventArgs = new PickWeaponEventArgs(this, weaponFilter);
			OnPickWeapon(this, pickWeaponEventArgs);
			return new Target(pickWeaponEventArgs.Weapon);
		}

		public void AddSpellsFrom(string spellList)
		{
			if (string.IsNullOrWhiteSpace(spellList))
				return;

			string[] spellsStrs = spellList.Split(';');
			foreach (var spell in spellsStrs)
			{
				AddSpell(spell.Trim());
			}
		}

		[JsonIgnore]
		public Queue<SoundEffect> additionalSpellHitSoundEffects = new Queue<SoundEffect>();

		public void AddSpellHitSoundEffect(string fileName, int timeOffset = 0)
		{
			additionalSpellHitSoundEffects.Enqueue(new SoundEffect(fileName, timeOffset));
		}

		[JsonIgnore]
		public Queue<SoundEffect> additionalSpellCastSoundEffects = new Queue<SoundEffect>();
		public void AddSpellCastSoundEffect(string fileName, int timeOffset = 0)
		{
			additionalSpellCastSoundEffects.Enqueue(new SoundEffect(fileName, timeOffset));
		}

		public void ClearAdditionalSpellEffects()
		{
			additionalSpellHitEffects.Clear();
			additionalSpellHitSoundEffects.Clear();
			additionalSpellCastEffects.Clear();
			additionalSpellCastSoundEffects.Clear();
		}

		public void AddRechargeable(string displayName, string varName, int maxValue, string cycle)
		{
			if (maxValue == 0)
				return;
			rechargeables.Add(new Rechargeable(displayName, varName, maxValue, cycle));
		}

		public void AddLanguage(string language)
		{
			if (Languages == null)
				Languages = new List<string>();
			if (Languages.Contains(language))
				return;
			Languages.AddRange(language.Split(',').Select(x => x.Trim()).Except(Languages).Distinct());
		}

		public void AddLanguages(string languageStr)
		{
			string[] languages = languageStr.Split(',');

			foreach (string language in languages)
				AddLanguage(language);
		}

		[JsonIgnore]
		public List<string> Languages { get; set; }

		public string dieRollMessageThisRoll = string.Empty;

		public void AddDieRollMessage(string dieRollMessage)
		{
			dieRollMessageThisRoll = dieRollMessage;
		}

		public string dieRollEffectsThisRoll = string.Empty;

		public void AddDieRollEffects(string dieRollEffects)
		{
			if (!string.IsNullOrWhiteSpace(dieRollEffectsThisRoll))
				dieRollEffectsThisRoll += ";";
			dieRollEffectsThisRoll += dieRollEffects;
		}

		[JsonIgnore]
		public string additionalDiceThisRoll = string.Empty;

		public void AddDice(string diceStr)
		{
			additionalDice.Add(diceStr);
			additionalDiceThisRoll = string.Join(",", additionalDice);
		}

		public void AddSpell(string spellStr)
		{
			if (string.IsNullOrWhiteSpace(spellStr))
				return;
			int totalCharges = int.MaxValue;
			DndTimeSpan chargeResetSpan = DndTimeSpan.Zero;
			string spellName = spellStr;
			string itemName = string.Empty;
			DndTimeSpan rechargesAt = DndTimeSpan.FromSeconds(0);  // midnight;
			string durationStr = string.Empty;
			if (spellName.Has("("))
			{
				spellName = spellName.EverythingBefore("(");
				if (HasSpell(spellName))
					return;

				string parameterStr = spellStr.EverythingBetween("(", ")");
				var parameters = parameterStr.Split(',');

				for (int i = 0; i < parameters.Length; i++)
				{
					var parameter = parameters[i].Trim();
					if (string.IsNullOrWhiteSpace(parameter))
						continue;

					if (i == 0)
						itemName = parameter.EverythingBetween("\"", "\"");
					else if (i == 1)
					{
						var chargeDetails = parameter.Split('/');
						if (chargeDetails.Length == 2)
						{
							// TODO: Add support for both max charges and a variable recharge (e.g., "1d6 + 4") rolled at dawn.
							int.TryParse(chargeDetails[0], out totalCharges);
							durationStr = chargeDetails[1];
							chargeResetSpan = DndTimeSpan.FromDurationStr(durationStr);
							if (durationStr == "dawn")
								rechargesAt = DndTimeSpan.FromHours(6);  // 6:00 am
						}
					}
				}
			}
			else if (HasSpell(spellName))
				return;
			KnownSpell knownSpell = new KnownSpell();
			knownSpell.SpellName = spellName;
			knownSpell.RechargesAt = rechargesAt.GetTimeSpan();
			knownSpell.TotalCharges = totalCharges;
			knownSpell.ResetSpan = chargeResetSpan;
			knownSpell.ItemName = itemName;
			knownSpell.Player = this;
			if (knownSpell.RechargesAt == TimeSpan.Zero && durationStr.HasSomething())
			{
				AddRechargeable(itemName, DndUtils.ToVarName(itemName), totalCharges, durationStr);
			}


			knownSpell.ChargesRemaining = totalCharges;  // setter Requires both ItemName and Player to be valid before setting.
			KnownSpells.Add(knownSpell);
		}

		[JsonIgnore]
		public List<string> additionalDice { get; set; } = new List<string>();

		[JsonIgnore]
		public List<KnownSpell> KnownSpells { get; private set; } = new List<KnownSpell>();

		[JsonIgnore]
		public List<CarriedWeapon> CarriedWeapons { get; private set; } = new List<CarriedWeapon>();

		[JsonIgnore]
		public List<CarriedAmmunition> CarriedAmmunition { get; private set; } = new List<CarriedAmmunition>();

		public string firstName
		{
			get
			{
				return DndUtils.GetFirstName(name);
			}
		}

		[JsonIgnore]
		public bool lastRollWasSuccessful { get; set; }

		public void AddShortcutToQueue(string shortcutName, bool rollImmediately)
		{
			Game.AddShortcutToQueue(this, shortcutName, rollImmediately);
		}

		[JsonIgnore]
		protected List<KnownSpell> temporarySpells = new List<KnownSpell>();

		public bool HasSpell(string spellName)
		{
			return KnownSpells.FirstOrDefault(x => x.SpellName == spellName) != null ||
				 temporarySpells.FirstOrDefault(x => x.SpellName == spellName) != null;
		}

		public int GetRemainingChargesOnItem(string itemName)
		{
			string varName = DndUtils.ToVarName(itemName);
			Rechargeable rechargeable = rechargeables.FirstOrDefault(x => x.VarName == varName);
			if (rechargeable != null)
				return rechargeable.ChargesRemaining;
			return 0;
		}

		public void SetRemainingChargesOnItem(string itemName, int value)
		{
			string varName = DndUtils.ToVarName(itemName);
			Rechargeable rechargeable = rechargeables.FirstOrDefault(x => x.VarName == varName);
			if (rechargeable != null && rechargeable.ChargesRemaining != value)
			{
				int oldChargesUsed = rechargeable.ChargesUsed;
				rechargeable.SetRemainingCharges(value);
				OnStateChanged(this, new StateChangedEventArgs(rechargeable.VarName, oldChargesUsed, rechargeable.ChargesUsed, true));
			}
		}

		public event PickAmmunitionEventHandler PickAmmunition;

		protected virtual void OnPickAmmunition(object sender, PickAmmunitionEventArgs ea)
		{
			PickAmmunition?.Invoke(sender, ea);
		}

		public event CastedSpellEventHandler SpellDispelled;

		protected virtual void OnSpellDispelled(object sender, CastedSpellEventArgs ea)
		{
			SpellDispelled?.Invoke(sender, ea);
		}

		public event MessageEventHandler RequestMessageToAll;

		protected virtual void OnRequestMessageToAll(string message)
		{
			RequestMessageToAll?.Invoke(this, new MessageEventArgs(message));
		}

		/// <summary>
		/// Sets D & D alarms to trigger recharges for items with recharges, like wands.
		/// </summary>
		public void SetTimeBasedEvents()
		{
			foreach (KnownSpell knownSpell in KnownSpells)
			{
				if (knownSpell.TotalCharges > 0 && knownSpell.TotalCharges != int.MaxValue)
				{
					if (knownSpell.ResetSpan.GetTimeSpan().TotalDays == 1)
					{
						DndAlarm dndAlarm = Game.Clock.CreateDailyAlarm("Recharge:" + knownSpell.ItemName, knownSpell.RechargesAt.Hours, knownSpell.RechargesAt.Minutes, knownSpell.RechargesAt.Seconds, this);
						dndAlarm.AlarmFired += DndAlarm_RechargeItem;
						AddRechargeable(knownSpell.ItemName, DndUtils.ToVarName(knownSpell.ItemName), knownSpell.TotalCharges, "1 day");
					}
				}
			}
		}

		private void DndAlarm_RechargeItem(object sender, DndTimeEventArgs ea)
		{
			if (ea.Alarm.Name.StartsWith("Recharge:"))
			{
				string itemName = ea.Alarm.Name.EverythingAfter("Recharge:");
				KnownSpell item = KnownSpells.FirstOrDefault(x => x.ItemName == itemName);
				if (item != null)
				{
					item.ChargesRemaining = item.TotalCharges;
				}
				OnRequestMessageToDungeonMaster($"{firstName}'s {item.ItemName} recharged ({item.TotalCharges}).");
			}
		}

		void TriggerMagicSavingThrow()
		{
			foreach (Magic magic in receivedMagic)
				magic.TriggerRecipientSaves(this);
		}

		void TriggerMagicAttack()
		{
			foreach (Magic magic in receivedMagic)
				magic.TriggerRecipientAttacks(this);
		}

		public void RollingSavingThrowNow()
		{
			TriggerMagicSavingThrow();
		}

		public void CreatureAttacksNow()
		{
			TriggerMagicAttack();
		}

		public void RemoveMagic(Magic magic)
		{
			receivedMagic.Remove(magic);
		}
		public bool usesMagicThisRoll = false;

		// TODO: When magic ammunition is loaded, set this to true before firing!
		public bool usesMagicAmmunitionThisRoll = false;
		public bool ammunitionOnFire = false;

		public int targetedCreatureHitPoints = 0;  // TODO: Implement this + test cases.

		[JsonIgnore]
		public bool hitWasCritical = false;  // TODO: Implement this + test cases.


		// TODO: Call after a successful roll.
		public void ResetPlayerRollBasedState()
		{
			lastRollWasSuccessful = false;
			usesMagicThisRoll = false;
			usesMagicAmmunitionThisRoll = false;
			ammunitionOnFire = false;
			targetedCreatureHitPoints = 0;
			hitWasCritical = false;
			additionalDice = new List<string>();
			overrideReplaceDamageDice = string.Empty;
			additionalDiceThisRoll = string.Empty;
			trailingEffectsThisRoll = string.Empty;
			dieRollEffectsThisRoll = string.Empty;
			dieRollMessageThisRoll = string.Empty;
		}
	}
}