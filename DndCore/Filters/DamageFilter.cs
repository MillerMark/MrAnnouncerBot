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

		private bool MatchesDamage(DamageType damageType)
		{
			return (damageType & DamageType) == damageType;
		}

		private bool MatchesAttackKind(AttackKind attackKind)
		{
			return (attackKind & AttackKind) == attackKind;
		}
	}
}