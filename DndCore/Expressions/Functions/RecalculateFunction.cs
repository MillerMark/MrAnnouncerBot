using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Recalculates the specified recalc option.")]
	[Param(1, typeof(RecalcOptions), "recalcOptions", "The option to recalculate (can be TurnBasedState, ActionBasedState, or Resistance).", ParameterIs.Required)]
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
