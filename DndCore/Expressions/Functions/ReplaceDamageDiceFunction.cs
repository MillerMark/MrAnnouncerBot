using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class ReplaceDamageDiceFunction : DndFunction
	{
		public override string Name => "ReplaceDamageDice";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string diceStr = evaluator.Evaluate<string>(args[0]);

			player.ReplaceDamageDice(diceStr);

			return null;
		}
	}
}
