using System;
using System.Linq;
using DndCore.Enums;
using DndCore.ViewModels.Support;

namespace DndCore.ViewModels
{
	public class DamageFilterViewModel : ViewModelBase
	{
		RadioEnumList attackKind;

		//private fields...
		CheckEnumList damageType;

		public DamageFilterViewModel()
		{
			damageType = new CheckEnumList(typeof(DamageType), Enums.DamageType.None, EnumListOption.Exclude);
			attackKind = new RadioEnumList(typeof(AttackKind), nameof(AttackKind));
		}

		public RadioEnumList AttackKind
		{
			get { return attackKind; }
			set
			{
				if (attackKind == value)
					return;
				attackKind = value;
				OnPropertyChanged();
			}
		}


		public CheckEnumList DamageType
		{
			get { return damageType; }
			set
			{
				if (damageType == value)
					return;
				damageType = value;
				OnPropertyChanged();
			}
		}
	}
}
