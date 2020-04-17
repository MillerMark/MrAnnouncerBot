using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class ReplaceDamageDiceFunction : DndFunction
	{
		public override string Name => "ReplaceDamageDice";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target, CastedSpell spell)
		{
			ExpectingArguments(args, 1);

			string diceStr = evaluator.Evaluate<string>(args[0]);

			player.ReplaceDamageDice(diceStr);

			return null;
		}
	}
}
