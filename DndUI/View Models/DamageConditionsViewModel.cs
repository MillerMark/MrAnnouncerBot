using DndCore;
using System;
using System.Linq;

namespace DndUI
{
	public class DamageConditionsViewModel : ViewModelBase
	{
		//private fields...
		CheckEnumList creatureSizeFilter;
		int concurrentTargets;
		int escapeDC;
		CheckEnumList conditions;
		
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
			if (conditions != null && creatureSizeFilter != null)
				return new DamageConditions((Conditions)conditions.Value, (CreatureSize)creatureSizeFilter.Value, escapeDC, concurrentTargets);

			return new DamageConditions(DndCore.Conditions.None, CreatureSize.Medium, escapeDC, concurrentTargets);
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
		}
	}
}
