using System;
using System.Linq;

namespace DndCore
{
	[Flags]
	public enum SpellType
	{
		None = 0,
		MeleeSpell = 1,
		RangedSpell = 2,
		SavingThrowSpell = 4,
		HitBonusSpell = 8,
		StartNextTurnSpell = 16,
		DamageSpell = 32,
		HealingSpell = 64,
		HpCapacitySpell = 128,
		OtherSpell = 256,
		NeedToRollDiceToCast = MeleeSpell | RangedSpell
	}
}
