using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class GiveAdvantageFunction : DndFunction
	{
		public override string Name => "GiveAdvantage";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player)
		{
			ExpectingArguments(args, 0);

			player.GiveAdvantageThisRoll();
			return null;
		}
	}
}
