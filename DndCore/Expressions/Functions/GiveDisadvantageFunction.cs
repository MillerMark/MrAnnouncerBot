using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Gives disadvantage to the active player's next roll.")]
	public class GiveDisadvantageFunction : DndFunction
	{
		public override string Name => "GiveDisadvantage";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0);

			player.GiveDisadvantageThisRoll();
			return null;
		}
	}
}
