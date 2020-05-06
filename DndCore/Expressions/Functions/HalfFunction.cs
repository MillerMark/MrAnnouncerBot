using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class HalfFunction : DndFunction
	{
		public override string Name { get; set; } = "Half";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);
			double value = Expressions.GetDouble(args[0], player, target, spell);
			return Math.Floor(value / 2.0);
		}
	}
}
