using DndCore;
using System;
using System.Linq;

namespace DndCore
{
	public class DamageViewModel : ViewModelBase
	{
		string damageRoll;
		RadioEnumList saveOpportunity;
		RadioEnumList damageHits;
		RadioEnumList attackKind;
		RadioEnumList damageType;

		public string DamageRoll
		{
			get { return damageRoll; }
			set
			{
				if (damageRoll == value)
					return;
				damageRoll = value;
				OnPropertyChanged();
			}
		}


		public RadioEnumList DamageType
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


		public RadioEnumList DamageHits
		{
			get { return damageHits; }
			set
			{
				if (damageHits == value)
					return;
				damageHits = value;
				OnPropertyChanged();
			}
		}


		public RadioEnumList SaveOpportunity
		{
			get { return saveOpportunity; }
			set
			{
				if (saveOpportunity == value)
					return;
				saveOpportunity = value;
				OnPropertyChanged();
			}
		}

		public Damage Damage
		{
			get
			{
				return GetDamage();
			}
			set
			{
				SetFromDamage(value);
			}
		}
		

		public Damage GetDamage()
		{
			if (damageType == null || attackKind == null || DamageRoll == null || damageHits == null || saveOpportunity == null)
				return new Damage(DndCore.DamageType.None, DndCore.AttackKind.Any, "");
			else
				return new Damage((DamageType)damageType.Value, (AttackKind)attackKind.Value, DamageRoll, (TimePoint)damageHits.Value, (TimePoint)saveOpportunity.Value);
		}

		public void SetFromDamage(Damage damage)
		{
			damageType.Value = damage.DamageType;
			attackKind.Value = damage.AttackKind;
			DamageRoll = damage.DamageRoll;
			damageHits.Value = damage.DamageHits;
			saveOpportunity.Value = damage.SaveOpportunity;
		}

		public DamageViewModel()
		{
			attackKind = new RadioEnumList(typeof(AttackKind), "AttackKind");
			saveOpportunity = new RadioEnumList(typeof(TimePoint), "SaveOpportunity");
			damageHits = new RadioEnumList(typeof(TimePoint), "DamageHits");
			damageType = new RadioEnumList(typeof(DamageType), "DamageType");
		}
	}
}
