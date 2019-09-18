using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class AddDiceFunction : DndFunction
	{
		public override string Name => "AddDice";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player)
		{
			ExpectingArguments(args, 1);

			string diceStr = evaluator.Evaluate<string>(args[0]);

			player.AddDice(diceStr);

			return null;
		}
	}
}
