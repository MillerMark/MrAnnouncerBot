using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Replaces the damage dice for the active player's next roll.")]
	[Param(1, typeof(string), "diceStr", "The new damage die string to roll (e.g., \"3d6(fire)\").", ParameterIs.Required)]
	public class ReplaceDamageDiceFunction : DndFunction
	{
		public override string Name => "ReplaceDamageDice";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string diceStr = evaluator.Evaluate<string>(args[0]);

			player.ReplaceDamageDice(diceStr);

			return null;
		}
	}
}
