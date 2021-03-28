using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Gives the target full damage from the last spell/magic attack.")]
	public class GiveTargetFullDamage : DndFunction
	{
		public override string Name { get; set; } = "GiveTargetFullDamage";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, RollResults dice = null)
		{
			ExpectingArguments(args, 0);
			return ApplyDamage(args, evaluator, player, target, 1);
		}

		public static object ApplyDamage(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, double multiplier = 1)
		{
			Dictionary<DamageType, int> latestDamage = Expressions.GetCustomData<Dictionary<DamageType, int>>(evaluator.Variables);
			if (latestDamage == null)
				return null;

			foreach (Creature creature in target.Creatures)
			{
				InGameCreature inGameCreature = AllInGameCreatures.GetByCreature(creature);
				if (inGameCreature != null)
				{
					inGameCreature.StartTakingDamage();
					if (inGameCreature != null)
						foreach (DamageType damageType in latestDamage.Keys)
						{
							// TODO: pass in AttackKind with the custom data
							ReportOnVulnerabilitiesAndResistance(player?.Game, creature, inGameCreature.Name, damageType);
							inGameCreature.TakeSomeDamage(player?.Game, damageType, AttackKind.Magical, (int)Math.Floor(latestDamage[damageType] * multiplier));
						}
					inGameCreature.FinishTakingDamage();
				}
				else
				{
					if (creature is Character thisPlayer)
					{
						foreach (DamageType damageType in latestDamage.Keys)
						{
							// TODO: pass in AttackKind with the custom data
							thisPlayer.TakeDamage(damageType, AttackKind.Any, latestDamage[damageType]);
							ReportOnVulnerabilitiesAndResistance(thisPlayer.Game, creature, thisPlayer.firstName, damageType);
						}
					}
				}
			}
			return null;
		}

		private static void ReportOnVulnerabilitiesAndResistance(DndGame game, Creature creature, string name, DamageType damageType)
		{
			if (creature.IsVulnerableTo(damageType, AttackKind.Magical))
				game?.TellDungeonMaster($"{name} is vulnerable to {damageType} damage.");
			else if (creature.IsResistantTo(damageType, AttackKind.Magical))
				game?.TellDungeonMaster($"{name} is resistant to {damageType} damage.");
		}
	}
}

