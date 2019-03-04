using DndCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DndUI
{
	public class AttackViewModel : ViewModelBase
	{
		string name;
		CheckEnumList includeCreatures;
		CheckEnumList excludeCreatures;
		CheckEnumList excludeTargetSenses;
		CheckEnumList includeTargetSenses;
		RadioEnumList type;
		SavingThrowViewModel savingThrow;
		ObservableCollection<DamageViewModel> damages;
		ObservableCollection<DamageConditionsViewModel> filteredConditions;
		ObservableCollection<DamageViewModel> successfulSaveDamages;
		int targetLimit;
		double plusToHit;
		double rangeMax;
		double reachRange;
		string description;
		bool needsRecharging;
		DndTimeSpan lasts;
		DndTimeSpan recharges;
		RadioEnumList rechargeOdds;
		CheckEnumList conditions;

		public RadioEnumList RechargeOdds
		{
			get { return rechargeOdds; }
			set
			{
				if (rechargeOdds == value)
					return;
				rechargeOdds = value;
				OnPropertyChanged();
			}
		}

		public DndTimeSpan Recharges
		{
			get { return recharges; }
			set
			{
				if (recharges == value)
					return;
				recharges = value;
				OnPropertyChanged();
			}
		}

		public DndTimeSpan Lasts
		{
			get { return lasts; }
			set
			{
				if (lasts == value)
					return;
				lasts = value;
				OnPropertyChanged();
			}
		}

		public bool NeedsRecharging
		{
			get { return needsRecharging; }
			set
			{
				if (needsRecharging == value)
					return;
				needsRecharging = value;
				OnPropertyChanged();
			}
		}


		public string Name
		{
			get { return name; }
			set
			{
				if (name == value)
					return;
				name = value;
				OnPropertyChanged();
			}
		}


		public string Description
		{
			get { return description; }
			set
			{
				if (description == value)
					return;
				description = value;
				OnPropertyChanged();
			}
		}


		public double ReachRange
		{
			get { return reachRange; }
			set
			{
				if (reachRange == value)
					return;
				reachRange = value;
				OnPropertyChanged();
			}
		}


		public double RangeMax
		{
			get { return rangeMax; }
			set
			{
				if (rangeMax == value)
					return;
				rangeMax = value;
				OnPropertyChanged();
			}
		}


		public double PlusToHit
		{
			get { return plusToHit; }
			set
			{
				if (plusToHit == value)
					return;
				plusToHit = value;
				OnPropertyChanged();
			}
		}


		public int TargetLimit
		{
			get { return targetLimit; }
			set
			{
				if (targetLimit == value)
					return;
				targetLimit = value;
				OnPropertyChanged();
			}
		}


		public ObservableCollection<DamageViewModel> Damages
		{
			get { return damages; }
			set
			{
				if (damages == value)
					return;

				if (damages != null)
					damages.CollectionChanged -= Damages_CollectionChanged;

				damages = value;

				if (damages != null)
					damages.CollectionChanged += Damages_CollectionChanged;

				OnPropertyChanged();
			}
		}

		private void Damages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged("Damages");
		}

		public ObservableCollection<DamageConditionsViewModel> FilteredConditions
		{
			get { return filteredConditions; }
			set
			{
				if (filteredConditions == value)
					return;

				if (filteredConditions != null)
					filteredConditions.CollectionChanged -= FilteredConditions_CollectionChanged;

				filteredConditions = value;

				if (filteredConditions != null)
					filteredConditions.CollectionChanged += FilteredConditions_CollectionChanged;

				OnPropertyChanged();
			}
		}

		private void FilteredConditions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged("FilteredConditions");
		}

		public ObservableCollection<DamageViewModel> SuccessfulSaveDamages
		{
			get { return successfulSaveDamages; }
			set
			{
				if (successfulSaveDamages == value)
					return;

				if (successfulSaveDamages != null)
					successfulSaveDamages.CollectionChanged -= SuccessfulSaveDamages_CollectionChanged;

				successfulSaveDamages = value;

				if (successfulSaveDamages != null)
					successfulSaveDamages.CollectionChanged += SuccessfulSaveDamages_CollectionChanged;

				OnPropertyChanged();
			}
		}

		private void SuccessfulSaveDamages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged("SuccessfulSaveDamages");
		}

		public CheckEnumList Conditions
		{
			get { return conditions; }
			set
			{
				if (conditions == value)
					return;
				conditions = value;
				OnPropertyChanged();
			}
		}


		public SavingThrowViewModel SavingThrow
		{
			get { return savingThrow; }
			set
			{
				if (savingThrow == value)
					return;
				savingThrow = value;
				OnPropertyChanged();
			}
		}


		public RadioEnumList Type
		{
			get { return type; }
			set
			{
				if (type == value)
					return;
				type = value;
				OnPropertyChanged();
			}
		}


		public CheckEnumList IncludeTargetSenses
		{
			get { return includeTargetSenses; }
			set
			{
				if (includeTargetSenses == value)
					return;
				includeTargetSenses = value;
				OnPropertyChanged();
			}
		}


		public CheckEnumList ExcludeTargetSenses
		{
			get { return excludeTargetSenses; }
			set
			{
				if (excludeTargetSenses == value)
					return;
				excludeTargetSenses = value;
				OnPropertyChanged();
			}
		}


		public CheckEnumList IncludeCreatures
		{
			get { return includeCreatures; }
			set
			{
				if (includeCreatures == value)
					return;
				includeCreatures = value;
				OnPropertyChanged();
			}
		}

		public CheckEnumList ExcludeCreatures
		{
			get { return excludeCreatures; }
			set
			{
				if (excludeCreatures == value)
					return;
				excludeCreatures = value;
				OnPropertyChanged();
			}
		}

		public Attack Attack
		{
			get { return GetAttack(); }
			set
			{
				SetFromAttack(value);
			}
		}

		List<Damage> GetListOf(ObservableCollection<DamageViewModel> damages)
		{
			List<Damage> results = new List<Damage>();
			foreach (DamageViewModel damageViewModel in damages)
				results.Add(damageViewModel.Damage);
			return results;
		}

		List<DamageConditions> GetListOf(ObservableCollection<DamageConditionsViewModel> damages)
		{
			List<DamageConditions> results = new List<DamageConditions>();
			foreach (DamageConditionsViewModel damageViewModel in damages)
				results.Add(damageViewModel.DamageConditions);
			return results;
		}

		public Attack GetAttack()
		{
			Attack attack = new Attack(Name);
			if (Conditions != null)
				attack.conditions = (Conditions)Conditions.Value;
			
			attack.damages = GetListOf(Damages);
			attack.description = Description;

			if (ExcludeCreatures != null)
				attack.excludeCreatures = (CreatureKinds)ExcludeCreatures.Value;

			if (IncludeCreatures != null)
				attack.includeCreatures = (CreatureKinds)IncludeCreatures.Value;

			if (ExcludeTargetSenses != null)
				attack.excludeTargetSenses = (Senses)ExcludeTargetSenses.Value;

			if (IncludeTargetSenses != null)
				attack.includeTargetSenses = (Senses)IncludeTargetSenses.Value;

			attack.filteredConditions = GetListOf(FilteredConditions);
			attack.lasts = Lasts;
			attack.needsRecharging = NeedsRecharging;
			attack.plusToHit = PlusToHit;
			attack.rangeMax = RangeMax;
			attack.reachRange = ReachRange;

			if (RechargeOdds != null)
				attack.rechargeOdds = (RechargeOdds)RechargeOdds.Value;

			attack.recharges = Recharges;
			if (SavingThrow != null)
				attack.savingThrow = new SavingThrow(SavingThrow.Success, (Ability)SavingThrow.Ability.Value);
			attack.successfulSaveDamages = GetListOf(SuccessfulSaveDamages);
			attack.targetLimit = TargetLimit;
			return attack;
		}

		void SetFrom(ObservableCollection<DamageViewModel> targetList, List<Damage> sourceList)
		{
			targetList.Clear();
			foreach (Damage damage in sourceList)
			{
				DamageViewModel damageViewModel = new DamageViewModel();
				damageViewModel.Damage = damage;
				targetList.Add(damageViewModel);
			}
		}

		void SetFrom(ObservableCollection<DamageConditionsViewModel> targetList, List<DamageConditions> sourceList)
		{
			targetList.Clear();
			foreach (DamageConditions damageConditions in sourceList)
			{
				DamageConditionsViewModel damageConditionsViewModel = new DamageConditionsViewModel();
				damageConditionsViewModel.DamageConditions = damageConditions;
				targetList.Add(damageConditionsViewModel);
			}
		}
		public void SetFromAttack(Attack attack)
		{
			Name = attack.Name;
			conditions.Value = attack.conditions;
			type.Value = attack.type;
			SetFrom(Damages, attack.damages);
			Description = attack.description;
			ExcludeCreatures.Value = attack.excludeCreatures;
			IncludeCreatures.Value = attack.includeCreatures;
			ExcludeTargetSenses.Value = attack.excludeTargetSenses;
			IncludeTargetSenses.Value = attack.includeTargetSenses;

			SetFrom(FilteredConditions, attack.filteredConditions);
			Lasts = attack.lasts;
			NeedsRecharging = attack.needsRecharging;
			PlusToHit = attack.plusToHit;
			RangeMax = attack.rangeMax;
			ReachRange = attack.reachRange;
			RechargeOdds.Value = attack.rechargeOdds;
			Recharges = attack.recharges;
			SavingThrow.Ability.Value = attack.savingThrow.Ability;
			SavingThrow.Success = attack.savingThrow.Success;
			SetFrom(SuccessfulSaveDamages, attack.successfulSaveDamages);
			TargetLimit = attack.targetLimit;
		}

		public AttackViewModel()
		{
			conditions = new CheckEnumList(typeof(Conditions), DndCore.Conditions.None, EnumListOption.Exclude);
			excludeTargetSenses = new CheckEnumList(typeof(Senses), DndCore.Senses.None, EnumListOption.Exclude);
			includeTargetSenses = new CheckEnumList(typeof(Senses), DndCore.Senses.None, EnumListOption.Exclude);
			excludeCreatures = new CheckEnumList(typeof(CreatureKinds), DndCore.CreatureKinds.None, EnumListOption.Exclude);
			includeCreatures = new CheckEnumList(typeof(CreatureKinds), DndCore.CreatureKinds.None, EnumListOption.Exclude);

			Damages = new ObservableCollection<DamageViewModel>();
			SuccessfulSaveDamages = new ObservableCollection<DamageViewModel>();
			FilteredConditions = new ObservableCollection<DamageConditionsViewModel>();

			type = new RadioEnumList(typeof(AttackType), "AttackType", DndCore.AttackType.None, EnumListOption.Exclude);
			rechargeOdds = new RadioEnumList(typeof(RechargeOdds), "RechargeOdds", DndCore.RechargeOdds.ZeroInSix, EnumListOption.Exclude);
			type.Value = AttackType.Melee;
			savingThrow = new SavingThrowViewModel();
			targetLimit = 1;
			reachRange = 5;
			rangeMax = 30;
			recharges = DndTimeSpan.Never;
			lasts = DndTimeSpan.OneMinute;
		}
	}
}
