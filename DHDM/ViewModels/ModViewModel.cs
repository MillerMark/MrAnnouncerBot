using DndCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHDM
{
	public class ModViewModel: ViewModelBase
	{
		DndTimeSpan repeats;
		double multiplier = 1;
		double offset;
		CheckEnumList conditions;
		DamageFilterViewModel damageTypeFilter;
		bool requiresConsumption;
		bool requiresEquipped;
		string targetName;
		RadioEnumList type;

		
		public Mod Mod
		{
			get { return GetMod(); }
			set
			{
				SetFromMod(value);
			}
		}
		Mod GetMod()
		{
			Mod mod = new Mod();
			if (conditions != null)
				mod.condition = (Conditions)conditions.Value;
			else
				mod.condition = DndCore.Conditions.None;

			if (damageTypeFilter != null)
				mod.damageTypeFilter = new DamageFilter((DamageType)damageTypeFilter.DamageType.Value,
					(AttackKind)damageTypeFilter.AttackKind.Value);
			else
				mod.damageTypeFilter = new DamageFilter(DamageType.None, AttackKind.Any);

			mod.multiplier = multiplier;
			mod.offset = offset;
			if (mod.repeats == null)
				mod.repeats = new DndTimeSpan(repeats.TimeMeasure, repeats.Count);
			else
			{
				mod.repeats.TimeMeasure = repeats.TimeMeasure;
				mod.repeats.Count = repeats.Count;
			}

			mod.requiresConsumption = requiresConsumption;
			mod.requiresEquipped = requiresEquipped;
			mod.targetName = targetName;
			return mod;
		}

		void SetFromMod(Mod mod)
		{
			conditions.Value = mod.condition;
			damageTypeFilter.DamageType.Value = mod.damageTypeFilter.DamageType;
			damageTypeFilter.AttackKind.Value = mod.damageTypeFilter.AttackKind;

			Multiplier = mod.multiplier;
			Offset = mod.offset;
			repeats.TimeMeasure = mod.repeats.TimeMeasure;
			repeats.Count = mod.repeats.Count;

			RequiresConsumption = mod.requiresConsumption;
			RequiresEquipped = mod.requiresEquipped;
			TargetName = mod.targetName;
		}


		public RadioEnumList ModType
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


		public string TargetName
		{
			get { return targetName; }
			set
			{
				if (targetName == value)
					return;
				targetName = value;
				OnPropertyChanged();
			}
		}


		public bool RequiresEquipped
		{
			get { return requiresEquipped; }
			set
			{
				if (requiresEquipped == value)
					return;
				requiresEquipped = value;
				OnPropertyChanged();
			}
		}


		public bool RequiresConsumption
		{
			get { return requiresConsumption; }
			set
			{
				if (requiresConsumption == value)
					return;
				requiresConsumption = value;
				OnPropertyChanged();
			}
		}


		public DamageFilterViewModel DamageTypeFilter
		{
			get { return damageTypeFilter; }
			set
			{
				if (damageTypeFilter == value)
					return;
				if (damageTypeFilter != null)
					damageTypeFilter.PropertyChanged -= DamageTypeFilter_PropertyChanged;

				damageTypeFilter = value;

				if (damageTypeFilter != null)
					damageTypeFilter.PropertyChanged += DamageTypeFilter_PropertyChanged;

				OnPropertyChanged();
			}
		}

		private void DamageTypeFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnPropertyChanged("DamageFilter");
		}

		//public EnumList DamageType
		//{
		//	get { return damageType; }
		//	set
		//	{
		//		if (damageType == value)
		//			return;
		//		damageType = value;
		//		OnPropertyChanged();
		//	}
		//}

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

		public ModViewModel()
		{
			//damageType = new EnumList(typeof(DamageType), DndCore.DamageType.None, EnumListOption.Exclude);
			//attackKind = new EnumList(typeof(AttackKind));
			damageTypeFilter = new DamageFilterViewModel();
			conditions = new CheckEnumList(typeof(Conditions), DndCore.Conditions.None, EnumListOption.Exclude);
			conditions.Value = DndCore.Conditions.None;
			type = new RadioEnumList(typeof(ModType), "ModType");
			type.Value = DndCore.ModType.incomingAttack;
			repeats = DndTimeSpan.Never;
		}


		public double Offset
		{
			get { return offset; }
			set
			{
				if (offset == value)
					return;
				offset = value;
				OnPropertyChanged();
			}
		}


		public double Multiplier  // < 1 for resistance, > 1 for vulnerability
		{
			get { return multiplier; }
			set
			{
				if (multiplier == value)
					return;
				multiplier = value;
				OnPropertyChanged();
			}
		}
		
		public DndTimeSpan Repeats
		{
			get { return repeats; }
			set
			{
				if (repeats == value)
					return;
				repeats = value;
				OnPropertyChanged();
			}
		}
	}
}
