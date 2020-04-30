using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class RecalculateFunction : DndFunction
	{
		public override string Name => "Recalculate";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			RecalcOptions recalcOptions = evaluator.Evaluate<RecalcOptions>(args[0]);
			player.Recalculate(recalcOptions);
			return null;
		}
	}
}
