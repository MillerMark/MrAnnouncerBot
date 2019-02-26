using DndCore;
using System;
using System.Linq;

namespace DHDM
{
	public class DamageFilterViewModel : ViewModelBase
	{

		//private fields...
		CheckEnumList damageType;
		RadioEnumList attackKind;

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
		
		public DamageFilterViewModel()
		{
			damageType = new CheckEnumList(typeof(DamageType), DndCore.DamageType.None, EnumListOption.Exclude);
			attackKind = new RadioEnumList(typeof(AttackKind), "AttackKind");
		}
	}
}
