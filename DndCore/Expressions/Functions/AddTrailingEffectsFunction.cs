using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class AddTrailingEffectsFunction : DndFunction
	{
		public override string Name => "AddTrailingEffects";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player)
		{
			ExpectingArguments(args, 1);

			string trailingEffects = evaluator.Evaluate<string>(args[0]);

			player.AddTailingEffects(trailingEffects);

			return null;
		}
	}
}
