using DndCore;
using System;
using System.Linq;

namespace DHDM
{
	public class DamageConditionsViewModel : ViewModelBase
	{
		//private fields...
		RadioEnumList comparisonFilter;
		CheckEnumList creatureSizeFilter;
		int concurrentTargets;
		int escapeDC;
		CheckEnumList conditions;
		
		public RadioEnumList ComparisonFilter
		{
			get { return comparisonFilter; }
			set
			{
				if (comparisonFilter == value)
					return;
				comparisonFilter = value;
				OnPropertyChanged();
			}
		}
		
		public CheckEnumList CreatureSizeFilter
		{
			get { return creatureSizeFilter; }
			set
			{
				if (creatureSizeFilter == value)
					return;
				creatureSizeFilter = value;
				OnPropertyChanged();
			}
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

		public int EscapeDC
		{
			get { return escapeDC; }
			set
			{
				if (escapeDC == value)
					return;
				escapeDC = value;
				OnPropertyChanged();
			}
		}

		public int ConcurrentTargets
		{
			get { return concurrentTargets; }
			set
			{
				if (concurrentTargets == value)
					return;
				concurrentTargets = value;
				OnPropertyChanged();
			}
		}

		public DamageConditions DamageConditions
		{
			get
			{
				return GetDamageCondition();
			}
			set
			{
				SetFrom(value);
			}
		}
		
		DamageConditions GetDamageCondition()
		{
			return new DamageConditions((Conditions)conditions.Value, (ComparisonFilterOption)ComparisonFilter.Value, (CreatureSize)creatureSizeFilter.Value, escapeDC, concurrentTargets);
		}

		void SetFrom(DamageConditions damageConditions)
		{
			conditions.Value = damageConditions.Conditions;
			creatureSizeFilter.Value = damageConditions.CreatureSizeFilter;
			escapeDC = damageConditions.EscapeDC;
			concurrentTargets = damageConditions.ConcurrentTargets;
		}

		public DamageConditionsViewModel()
		{
			conditions = new CheckEnumList(typeof(Conditions), DndCore.Conditions.None, EnumListOption.Exclude);
			creatureSizeFilter = new CheckEnumList(typeof(CreatureSize));
			comparisonFilter = new RadioEnumList(typeof(ComparisonFilterOption), "ComparisonFilter");
		}
	}
}
