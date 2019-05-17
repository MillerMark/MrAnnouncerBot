using System;
using System.Linq;
using DndCore.Enums;
using DndCore.ViewModels.Support;

namespace DndCore.ViewModels
{
	public class DamageConditionsViewModel : ViewModelBase
	{
		int concurrentTargets;
		CheckEnumList conditions;
		//private fields...
		CheckEnumList creatureSizeFilter;
		int escapeDC;

		public DamageConditionsViewModel()
		{
			conditions = new CheckEnumList(typeof(Conditions), Enums.Conditions.None, EnumListOption.Exclude);
			creatureSizeFilter = new CheckEnumList(typeof(CreatureSize));
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

		DamageConditions GetDamageCondition()
		{
			if (conditions != null && creatureSizeFilter != null)
				return new DamageConditions((Conditions)conditions.Value, (CreatureSize)creatureSizeFilter.Value, escapeDC, concurrentTargets);

			return new DamageConditions(Enums.Conditions.None, CreatureSize.Medium, escapeDC, concurrentTargets);
		}

		void SetFrom(DamageConditions damageConditions)
		{
			conditions.Value = damageConditions.Conditions;
			creatureSizeFilter.Value = damageConditions.CreatureSizeFilter;
			escapeDC = damageConditions.EscapeDC;
			concurrentTargets = damageConditions.ConcurrentTargets;
		}
	}
}
