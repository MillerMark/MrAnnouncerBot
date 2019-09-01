using System;
using System.Linq;

namespace DndCore
{
	public class Damage
	{
		// TODO: add filtering - includeFilter, excludeFilter
		// for body sizes
		public Damage(DamageType damageType, AttackKind attackKind, string damageRoll, TimePoint damageHits = TimePoint.Immediately, TimePoint saveOpportunity = TimePoint.None, Conditions conditions = Conditions.None, int savingThrowSuccess = int.MaxValue, Ability savingThrowAbility = Ability.none)
		{
			SavingThrowAbility = savingThrowAbility;
			SavingThrowSuccess = savingThrowSuccess;
			SaveOpportunity = saveOpportunity;
			AttackKind = attackKind;
			DamageType = damageType;
			DamageRoll = damageRoll;
			DamageHits = damageHits;
			Conditions = conditions;
			IncludeTargetSenses = Senses.None;
			IncludeCreatureSizes = CreatureSizes.All;
			IncludeCreatures = CreatureKinds.None;
		}

		public AttackKind AttackKind { get; set; }
		public Conditions Conditions { get; set; }
		public TimePoint DamageHits { get; set; }

		public string DamageRoll { get; set; }
		public DamageType DamageType { get; set; }
		public CreatureKinds IncludeCreatures { get; set; }
		public CreatureSize IncludeCreatureSizes { get; set; }

		public Senses IncludeTargetSenses { get; set; }
		public TimePoint SaveOpportunity { get; set; }
		public Ability SavingThrowAbility { get; set; }
		public int SavingThrowSuccess { get; set; }

		public void ApplyTo(Character player)
		{
			player.TakeDamage(DamageType, AttackKind, GetDamageRoll());
		}

		public void ExcludeCreatureKinds(CreatureKinds creatureKinds)
		{
			const CreatureKinds allCreatureKinds = CreatureKinds.Aberrations |
				CreatureKinds.Beasts |
				CreatureKinds.Celestials |
				CreatureKinds.Constructs |
				CreatureKinds.Dragons |
				CreatureKinds.Elemental |
				CreatureKinds.Fey |
				CreatureKinds.Fiends |
				CreatureKinds.Giants |
				CreatureKinds.Humanoids |
				CreatureKinds.Monstrosities |
				CreatureKinds.Oozes |
				CreatureKinds.Plants |
				CreatureKinds.Undead;

			IncludeCreatures = allCreatureKinds & ~creatureKinds;
		}

		public double GetDamageRoll()
		{
			if (DamageRoll == "1d6")
				return 3.5;
			if (DamageRoll == "2d6+2")
				return 9;
			return 0;
		}

		public bool Saves(int savingThrow)
		{
			return savingThrow >= SavingThrowSuccess;
		}

	}
}
