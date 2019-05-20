using System;
using System.Linq;

namespace DndCore
{
	/*
		Exhaustion
		Some Special Abilities and environmental hazards, such as starvation and the long-­term Effects of freezing or scorching temperatures, can lead to a Special condition called exhaustion. Exhaustion is measured in six levels. An effect can give a creature one or more levels of exhaustion, as specified in the effect’s description.

		Exhaustion Effects
		Level	Effect
		1	Disadvantage on Ability Checks
		2	Speed halved
		3	Disadvantage on Attack rolls and Saving Throws
		4	Hit point maximum halved
		5	Speed reduced to 0
		6	Death */
	public enum ExhaustionLevels
	{
		level1DisadvantageOnAbilityChecks = 1,
		level2SpeedHalved = 2,
		level3DisadvantageOnAttackRollsAndSavingThrows = 3,
		level4HitPointMaximumHalved = 4,
		level5SpeedReducedToZero = 5,
		level6Death = 6
	}
}