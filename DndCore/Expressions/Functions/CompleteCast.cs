using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class CompleteCast : DndFunction
	{
		public override string Name { get; set; } = "CompleteCast";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target = null, CastedSpell spell = null)
		{
			ExpectingArguments(args, 0);
			if (player != null)
				player.CompleteCast();
			return null;
		}
	}
}
