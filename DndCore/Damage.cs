using System;
using System.Linq;

namespace DndCore
{
	public class Damage
	{
		// TODO: add filtering - includeFilter, excludeFilter
		// for body sizes
		public Damage(DamageType damageType, AttackKind attackKind, string damageRoll, TimePoint damageHits = TimePoint.Immediately, TimePoint saveOpportunity = TimePoint.None)
		{
			SaveOpportunity = saveOpportunity;
			AttackKind = attackKind;
			DamageType = damageType;
			DamageRoll = damageRoll;
			DamageHits = damageHits;
		}

		public void ApplyTo(Character player)
		{
			player.TakeDamage(DamageType, AttackKind,  GetDamageRoll());
		}

		public double GetDamageRoll()
		{
			if (DamageRoll == "1d6")
				return 3.5;
			if (DamageRoll == "2d6+2")
				return 9;
			return 0;
		}

		public string DamageRoll { get; set; }
		public DamageType DamageType { get; set; }
		public AttackKind AttackKind { get; set; }
		public TimePoint DamageHits { get; set; }
		public TimePoint SaveOpportunity { get; set; }
	}
}
