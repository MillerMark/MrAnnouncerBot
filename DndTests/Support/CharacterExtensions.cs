using System;
using DndCore;

namespace DndTests
{
	public static class CharacterExtensions
	{
		public static void HitsTarget(this Creature creature)
		{
			const int hiddenThreshold = 12;
			const int successfulAttackRoll = 12;
			const int damage = 7;
			creature.ReadyRollDice(DiceRollType.Attack, "1d20(:score),1d6(:damage)", hiddenThreshold);
			creature.DieRollStopped(successfulAttackRoll, damage);  // Hits target!
		}

		public static void MissesTarget(this Creature creature)
		{
			const int hiddenThreshold = 12;
			const int failedAttackRoll = 11;
			const int damage = 7;
			creature.ReadyRollDice(DiceRollType.Attack, "1d20(:score),1d6(:damage)", hiddenThreshold);
			creature.DieRollStopped(failedAttackRoll, damage);  // Fails to hit.
		}

		public static void Hits(this Creature creature, Creature target, PlayerActionShortcut weapon)
		{
			creature.PrepareAttack(target, weapon);
			creature.HitsTarget();
		}

		public static void Misses(this Creature creature, Creature target, PlayerActionShortcut weapon)
		{
			creature.PrepareAttack(target, weapon);
			creature.MissesTarget();
		}

		public static void Misses(this Creature creature, Creature target, string attackName)
		{
			creature.PrepareAttack(target, creature.GetAttack(attackName));
			creature.MissesTarget();
		}
	}
}
