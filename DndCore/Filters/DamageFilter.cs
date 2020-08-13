using System;
using System.Linq;

namespace DndCore
{
	public class DamageFilter
	{
		public DamageFilter(DamageType damageType, AttackKind attackKind)
		{
			AttackKind = attackKind;
			DamageType = damageType;
		}

		public AttackKind AttackKind { get; set; }
		public DamageType DamageType { get; set; }

		public bool Matches(DamageType damageType, AttackKind attackKind)
		{
			return MatchesDamage(damageType) && MatchesAttackKind(attackKind);
		}

		bool MatchesAttackKind(AttackKind attackKind)
		{
			return (attackKind & AttackKind) == attackKind;
		}

		bool MatchesDamage(DamageType damageType)
		{
			if (damageType == DamageType.None)  // No filter can match no damage type.
				return false;
			return (damageType & DamageType) == damageType;
		}
	}
}