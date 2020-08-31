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
					inGameCreature.TakeHalfDamage(player, latestDamage, AttackKind.Magical);
				}
				// TODO: Also get players from the target and work with them.
			}
			return null;
		}
	}
}
