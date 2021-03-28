using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[ReturnType(typeof(int))]
	[Tooltip("Cuts a specified value in half, *rounding down*.")]
	[Param(1, typeof(string), "value", "The value to halve.", ParameterIs.Required)]
	public class HalfFunction : DndFunction
	{
		public override string Name { get; set; } = "Half";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1);
			double value = Expressions.GetDouble(args[0], player, target, spell);
			return Math.Floor(value / 2.0);
		}
	}
}
