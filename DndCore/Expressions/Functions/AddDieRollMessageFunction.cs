using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified message to the next die roll.")]
	[Param(1, typeof(string), "dieRollMessage", "The message to show.", ParameterIs.Required)]
	public class AddDieRollMessageFunction : DndFunction
	{
		public override string Name => "AddDieRollMessage";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string dieRollMessage = evaluator.Evaluate<string>(args[0]);

			player.AddDieRollMessage(dieRollMessage);

			return null;
		}
	}
}
