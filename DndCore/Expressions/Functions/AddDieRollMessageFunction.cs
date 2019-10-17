using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class AddDieRollMessageFunction : DndFunction
	{
		public override string Name => "AddDieRollMessage";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target, CastedSpell spell)
		{
			ExpectingArguments(args, 1);

			string dieRollMessage = evaluator.Evaluate<string>(args[0]);

			player.AddDieRollMessage(dieRollMessage);

			return null;
		}
	}
}
