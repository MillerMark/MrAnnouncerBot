using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DndCore;
using Newtonsoft.Json;

namespace DndCore
{
	public class ModViewModel : ListEntry
	{
		double absolute;
		bool addsAdvantage;
		bool addsDisadvantage;
		DamageFilterViewModel damageTypeFilter;
		RadioEnumList modAddAbilityModifier;
		CheckEnumList modConditions;
		int modifierLimit = 0;
		RadioEnumList modType;
		double multiplier = 1;
		double offset;
		DndTimeSpan repeats = DndTimeSpan.Zero;
		bool requiresConsumption;
		bool requiresEquipped;
		string targetName;
		Skills vantageSkillFilter;

		public ModViewModel()
		{
			//damageType = new EnumList(typeof(DamageType), DndCore.DamageType.None, EnumListOption.Exclude);
			//attackKind = new EnumList(typeof(AttackKind));
			damageTypeFilter = new DamageFilterViewModel();
			damageTypeFilter.DamageType.Value = DamageType.None;
			damageTypeFilter.AttackKind.Value = AttackKind.Any;
			modConditions = new CheckEnumList(typeof(Conditions), DndCore.Conditions.None, EnumListOption.Exclude);
			modConditions.Value = DndCore.Conditions.None;
			modType = new RadioEnumList(typeof(ModType), nameof(ModType));
			modType.Value = DndCore.ModType.incomingAttack;
			modAddAbilityModifier = new RadioEnumList(typeof(Ability), "AddModifier");
			modAddAbilityModifier.Value = Skills.none;
			repeats = DndTimeSpan.Never;
		}

		public ModViewModel(string name) : this()
		{
			Name = name;
		}

		public double Absolute
		{
			get { return absolute; }
			set
			{
				if (absolute == value)
					return;
				absolute = value;
				OnPropertyChanged();
			}
		}

		public Ability AddAbilityModifier
		{
			get { return (Ability)modAddAbilityModifier.Value; }
			set
			{
				Ability existingModType = (Ability)modAddAbilityModifier.Value;
				if (existingModType == value)
					return;

				modAddAbilityModifier.Value = value;
				OnPropertyChanged();
			}
		}

		public bool AddsAdvantage
		{
			get { return addsAdvantage; }
			set
			{
				if (addsAdvantage == value)
					return;
				addsAdvantage = value;
				OnPropertyChanged();
			}
		}

		public bool AddsDisadvantage
		{
			get { return addsDisadvantage; }
			set
			{
				if (addsDisadvantage == value)
					return;
				addsDisadvantage = value;
				OnPropertyChanged();
			}
		}

		public Conditions Conditions
		{
			get { return (Conditions)modConditions.Value; }
			set
			{
				Conditions existingConditions = (Conditions)modConditions.Value;
				if (existingConditions == value)
					return;

				modConditions.Value = value;
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

		[JsonIgnore]
		public Mod Mod
		{
			get { return GetMod(); }
			set
			{
				SetFromMod(value);
			}
		}

		[JsonIgnore]
		public RadioEnumList ModAddAbilityModifier
		{
			get { return modAddAbilityModifier; }
			set
			{
				if (modAddAbilityModifier == value)
					return;

				modAddAbilityModifier = value;
				OnPropertyChanged();
			}
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

		[JsonIgnore]
		public CheckEnumList ModConditions
		{
			get { return modConditions; }
			set
			{
				if (modConditions == value)
					return;
				modConditions = value;
				OnPropertyChanged();
			}
		}

		public int ModifierLimit  // < 1 for resistance, > 1 for vulnerability
		{
			get { return modifierLimit; }
			set
			{
				if (modifierLimit == value)
					return;
				modifierLimit = value;
				OnPropertyChanged();
			}
		}


		[JsonIgnore]
		public RadioEnumList ModType
		{
			get { return modType; }
			set
			{
				if (modType == value)
					return;

				modType = value;
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

		public DndTimeSpan Repeats
		{
			get { return repeats; }
			set
			{
				if (repeats.Equals(value))
					return;
				repeats = value;
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

		public ModType Type
		{
			get { return (ModType)modType.Value; }
			set
			{
				ModType existingModType = (ModType)modType.Value;
				if (existingModType == value)
					return;

				modType.Value = value;
				OnPropertyChanged();
			}
		}

		public Skills VantageSkillFilter
		{
			get { return vantageSkillFilter; }
			set
			{
				if (vantageSkillFilter == value)
					return;
				vantageSkillFilter = value;
				OnPropertyChanged();
			}
		}

		void DamageTypeFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnPropertyChanged(nameof(DamageFilter));
		}

		Mod GetMod()
		{
			Mod mod = new Mod();
			if (modConditions != null)
				mod.condition = (Conditions)Convert.ToInt32(modConditions.Value);
			else
				mod.condition = DndCore.Conditions.None;

			if (damageTypeFilter != null && damageTypeFilter.DamageType != null && damageTypeFilter.AttackKind != null)
				mod.damageTypeFilter = new DamageFilter((DamageType)Convert.ToInt32(damageTypeFilter.DamageType.Value),
					(AttackKind)Convert.ToInt32(damageTypeFilter.DamageType.Value));
			else
				mod.damageTypeFilter = new DamageFilter(DamageType.None, AttackKind.Any);

			mod.multiplier = multiplier;
			mod.addModifier = (Ability)Convert.ToInt32(damageTypeFilter.DamageType.Value);
			mod.modifierLimit = modifierLimit;
			mod.addsAdvantage = AddsAdvantage;
			mod.addsDisadvantage = AddsDisadvantage;
			mod.vantageSkillFilter = VantageSkillFilter;
			mod.absolute = Absolute;

			mod.offset = offset;
			mod.repeats = new DndTimeSpan(repeats.TimeMeasure, repeats.Count);
			//if (repeats == null)
			//	mod.repeats = DndTimeSpan.Never;
			//else if (mod.repeats == null)
			//	mod.repeats = new DndTimeSpan(repeats.TimeMeasure, repeats.Count);
			//else
			//{
			//	mod.repeats.TimeMeasure = repeats.TimeMeasure;
			//	mod.repeats.Count = repeats.Count;
			//}

			mod.requiresConsumption = requiresConsumption;
			mod.requiresEquipped = requiresEquipped;
			mod.targetName = TargetName;
			return mod;
		}

		void SetFromMod(Mod mod)
		{
			modConditions.Value = mod.condition;
			damageTypeFilter.DamageType.Value = mod.damageTypeFilter.DamageType;
			damageTypeFilter.AttackKind.Value = mod.damageTypeFilter.AttackKind;

			Multiplier = mod.multiplier;
			Offset = mod.offset;
			repeats.TimeMeasure = mod.repeats.TimeMeasure;
			repeats.Count = mod.repeats.Count;

			RequiresConsumption = mod.requiresConsumption;
			RequiresEquipped = mod.requiresEquipped;
			TargetName = mod.targetName;

			damageTypeFilter.DamageType.Value = mod.addModifier;
			modifierLimit = mod.modifierLimit;
			AddsAdvantage = mod.addsAdvantage;
			AddsDisadvantage = mod.addsDisadvantage;
			VantageSkillFilter = mod.vantageSkillFilter;
			Absolute = mod.absolute;
		}
	}
}
