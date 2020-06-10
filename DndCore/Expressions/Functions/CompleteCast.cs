using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Completes casting of the prepared spell for the active player.")]
	public class CompleteCast : DndFunction
	{
		public override string Name { get; set; } = "CompleteCast";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0);
			if (player != null)
				player.CompleteCast();
			return null;
		}
	}
}
