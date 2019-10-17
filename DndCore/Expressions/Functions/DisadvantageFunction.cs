using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DisadvantageFunction : DndFunction
	{
		public override string Name => "GiveDisadvantage";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target, CastedSpell spell)
		{
			ExpectingArguments(args, 0);

			player.GiveDisadvantageThisRoll();
			return null;
		}
	}
}
