using System;
using System.Linq;
using DndCore;

namespace DndCore
{
	public class DamageViewModel : ViewModelBase
	{
		RadioEnumList attackKind;
		CheckEnumList conditions;
		CheckEnumList creatureSizeFilter;
		RadioEnumList damageHits;
		// TODO: Add conditions and filters recently added to Damage.
		string damageRoll;
		RadioEnumList damageType;
		CheckEnumList includeCreatures;
		CheckEnumList includeTargetSenses;
		RadioEnumList saveOpportunity;
		CheckEnumList savingThrowAbility;
		int savingThrowSuccess;

		public DamageViewModel()
		{
			attackKind = new RadioEnumList(typeof(AttackKind), nameof(AttackKind));
			saveOpportunity = new RadioEnumList(typeof(TimePoint), nameof(SaveOpportunity));
			damageHits = new RadioEnumList(typeof(TimePoint), nameof(DamageHits));
			damageType = new RadioEnumList(typeof(DamageType), nameof(DamageType));
			includeTargetSenses = new CheckEnumList(typeof(Senses));
			savingThrowAbility = new CheckEnumList(typeof(Ability));
			includeCreatures = new CheckEnumList(typeof(CreatureKinds));
			conditions = new CheckEnumList(typeof(Conditions));
			creatureSizeFilter = new CheckEnumList(typeof(CreatureSize));
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

		public CheckEnumList SavingThrowAbility
		{
			get { return savingThrowAbility; }
			set
			{
				if (savingThrowAbility == value)
					return;
				savingThrowAbility = value;
				OnPropertyChanged();
			}
		}

		public int SavingThrowSuccess
		{
			get { return savingThrowSuccess; }
			set
			{
				if (savingThrowSuccess == value)
					return;
				savingThrowSuccess = value;
				OnPropertyChanged();
			}
		}


		public Damage GetDamage()
		{
			Damage damage;
			if (damageType == null || attackKind == null || DamageRoll == null || damageHits == null || saveOpportunity == null)
				damage = new Damage(DndCore.DamageType.None, DndCore.AttackKind.Any, string.Empty);
			else
				damage = new Damage((DamageType)damageType.Value, (AttackKind)attackKind.Value, DamageRoll, (TimePoint)damageHits.Value, (TimePoint)saveOpportunity.Value);

			damage.SavingThrowSuccess = SavingThrowSuccess;
			damage.SavingThrowAbility = (Ability)SavingThrowAbility.Value;

			if (Conditions != null)
				damage.Conditions = (Conditions)Conditions.Value;
			if (IncludeCreatures != null)
				damage.IncludeCreatures = (CreatureKinds)IncludeCreatures.Value;
			if (IncludeTargetSenses != null)
				damage.IncludeTargetSenses = (Senses)IncludeTargetSenses.Value;
			if (CreatureSizeFilter != null)
				damage.IncludeCreatureSizes = (CreatureSize)CreatureSizeFilter.Value;

			return damage;
		}

		public void SetFromDamage(Damage damage)
		{
			damageType.Value = damage.DamageType;
			attackKind.Value = damage.AttackKind;
			DamageRoll = damage.DamageRoll;
			SavingThrowSuccess = damage.SavingThrowSuccess;
			SavingThrowAbility.Value = damage.SavingThrowAbility;
			damageHits.Value = damage.DamageHits;
			saveOpportunity.Value = damage.SaveOpportunity;

			if (Conditions != null)
				Conditions.Value = damage.Conditions;
			if (IncludeCreatures != null)
				IncludeCreatures.Value = damage.IncludeCreatures;
			if (IncludeTargetSenses != null)
				IncludeTargetSenses.Value = damage.IncludeTargetSenses;
			if (CreatureSizeFilter != null)
				CreatureSizeFilter.Value = damage.IncludeCreatureSizes;
		}
	}
}
