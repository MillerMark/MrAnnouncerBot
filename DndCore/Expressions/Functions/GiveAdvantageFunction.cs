using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class PlayerHasAdvantageFunction : DndFunction
	{
		public override string Name => "PlayerHasAdvantage";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0);

			player.GiveAdvantageThisRoll();
			return null;
		}
	}
}
