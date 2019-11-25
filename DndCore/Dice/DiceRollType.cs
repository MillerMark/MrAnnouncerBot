using System;
using System.Linq;

namespace DndCore
{
	public enum DiceRollType
	{
		None,
		SkillCheck,
		Attack,
		SavingThrow,
		FlatD20,
		DeathSavingThrow,
		PercentageRoll,
		WildMagic,
		BendLuckAdd,
		BendLuckSubtract,
		LuckRollLow,
		LuckRollHigh,
		DamageOnly,
		HealthOnly,
		ExtraOnly,
		ChaosBolt,
		Initiative,
		WildMagicD20Check,
		InspirationOnly,
		AddOnDice,
		NonCombatInitiative,
		HPCapacity
	}

}
