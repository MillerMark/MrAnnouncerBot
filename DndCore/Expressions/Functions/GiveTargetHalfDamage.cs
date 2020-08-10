using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Gives the target full damage from the last spell/magic attack.")]
	public class GiveTargetHalfDamage : DndFunction
	{
		public override string Name { get; set; } = "GiveTargetHalfDamage";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0);
			Dictionary<DamageType, int> latestDamage = Expressions.GetCustomData<Dictionary<DamageType, int>>(evaluator.Variables);
			if (latestDamage == null)
				return null;

			foreach (Creature creature in target.Creatures)
			{
				InGameCreature inGameCreature = AllInGameCreatures.GetByCreature(creature);
				if (inGameCreature != null)
				{
					inGameCreature.StartTakingDamage();
					foreach (DamageType damageType in latestDamage.Keys)
					{
						// TODO: pass in AttackKind with the custom data
						int damageTaken = DndUtils.HalveValue(latestDamage[damageType]);
						TakeSomeDamage(player, inGameCreature, damageType, AttackKind.Magical, damageTaken);
					}
					inGameCreature.FinishTakingDamage();
				}
				// TODO: Also get players from the target and work with them.
			}
			return null;
		}

		public static void TakeSomeDamage(Character player, InGameCreature inGameCreature, DamageType damageType, AttackKind attackKind, int amount)
		{
			double previousHP = inGameCreature.TotalHp;
			inGameCreature.TakeSomeDamage(damageType, attackKind, amount);
			double hpLost = previousHP - inGameCreature.TotalHp;
			if (hpLost == 0)
				return;

			string tempHpDetails = string.Empty;
			if (inGameCreature.Creature.tempHitPoints > 0)
				tempHpDetails = $" (tempHp: {inGameCreature.Creature.tempHitPoints})";

			string message;
			if (hpLost == 1)
				message = $"{inGameCreature.Name} just took 1 point of {damageType} damage. HP is now: {inGameCreature.Creature.HitPoints}/{inGameCreature.Creature.maxHitPoints}{tempHpDetails}";
			else
				message = $"{inGameCreature.Name} just took {hpLost} points of {damageType} damage. HP is now: {inGameCreature.Creature.HitPoints}/{inGameCreature.Creature.maxHitPoints}{tempHpDetails}";

			if (player == null)
				return;

			player.Game.TellDungeonMaster(message);
		}
	}
}
