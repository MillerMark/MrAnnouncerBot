using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DndCore;

namespace DndCore
{
	public class AttackViewModel : ListEntry
	{
		ObservableCollection<DamageViewModel> damages;
		string description;
		DndTimeSpan lasts;
		bool needsRecharging;
		double plusToHit;
		double rangeMax;
		double reachRange;
		RadioEnumList rechargeOdds;
		DndTimeSpan recharges;
		//ObservableCollection<DamageConditionsViewModel> filteredConditions;
		ObservableCollection<DamageViewModel> successfulSaveDamages;
		int targetLimit;
		RadioEnumList type;

		public AttackViewModel()
		{
			Damages = new ObservableCollection<DamageViewModel>();
			SuccessfulSaveDamages = new ObservableCollection<DamageViewModel>();


			type = new RadioEnumList(typeof(AttackType), nameof(AttackType), AttackType.None, EnumListOption.Exclude);
			rechargeOdds = new RadioEnumList(typeof(RechargeOdds), nameof(RechargeOdds), DndCore.RechargeOdds.ZeroInSix, EnumListOption.Exclude);
			type.Value = AttackType.Melee;
			targetLimit = 1;
			reachRange = 5;
			rangeMax = 30;
			recharges = DndTimeSpan.Never;
			lasts = DndTimeSpan.OneMinute;
		}

		public Attack Attack
		{
			get { return GetAttack(); }
			set
			{
				SetFromAttack(value);
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

		public DndTimeSpan Lasts
		{
			get { return lasts; }
			set
			{
				if (lasts.Equals(value))
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
				if (recharges.Equals(value))
					return;
				recharges = value;
				OnPropertyChanged();
			}
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

		void Damages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(Damages));
		}

		public Attack GetAttack()
		{
			Attack attack = new Attack(Name);


			attack.damages = GetListOf(Damages);
			attack.description = Description;

			attack.lasts = Lasts;
			attack.needsRecharging = NeedsRecharging;
			attack.plusToHit = PlusToHit;
			attack.rangeMax = RangeMax;
			attack.reachRange = ReachRange;

			if (RechargeOdds != null)
				attack.rechargeOdds = (RechargeOdds)RechargeOdds.Value;

			attack.recharges = Recharges;
			attack.successfulSaveDamages = GetListOf(SuccessfulSaveDamages);
			attack.targetLimit = TargetLimit;
			return attack;
		}

		List<Damage> GetListOf(ObservableCollection<DamageViewModel> damages)
		{
			List<Damage> results = new List<Damage>();
			if (damages != null)
				foreach (DamageViewModel damageViewModel in damages)
					results.Add(damageViewModel.Damage);
			return results;
		}

		List<DamageConditions> GetListOf(ObservableCollection<DamageConditionsViewModel> damages)
		{
			List<DamageConditions> results = new List<DamageConditions>();
			if (damages != null)
				foreach (DamageConditionsViewModel damageViewModel in damages)
					results.Add(damageViewModel.DamageConditions);
			return results;
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

		//void SetFrom(ObservableCollection<DamageConditionsViewModel> targetList, List<DamageConditions> sourceList)
		//{
		//	targetList.Clear();
		//	foreach (DamageConditions damageConditions in sourceList)
		//	{
		//		DamageConditionsViewModel damageConditionsViewModel = new DamageConditionsViewModel();
		//		damageConditionsViewModel.DamageConditions = damageConditions;
		//		targetList.Add(damageConditionsViewModel);
		//	}
		//}
		public void SetFromAttack(Attack attack)
		{
			Name = attack.Name;
			type.Value = attack.type;
			SetFrom(Damages, attack.damages);
			Description = attack.description;

			Lasts = attack.lasts;
			NeedsRecharging = attack.needsRecharging;
			PlusToHit = attack.plusToHit;
			RangeMax = attack.rangeMax;
			ReachRange = attack.reachRange;
			RechargeOdds.Value = attack.rechargeOdds;
			Recharges = attack.recharges;
			SetFrom(SuccessfulSaveDamages, attack.successfulSaveDamages);
			TargetLimit = attack.targetLimit;
		}

		void SuccessfulSaveDamages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(SuccessfulSaveDamages));
		}
	}
}
