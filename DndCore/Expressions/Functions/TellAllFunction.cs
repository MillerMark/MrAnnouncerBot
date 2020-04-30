using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class TellAllFunction : DndFunction
	{
		public override string Name => "TellAll";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string message = evaluator.Evaluate<string>(args[0]);

			player.Game.TellAll(message);

			return null;
		}
	}
}
