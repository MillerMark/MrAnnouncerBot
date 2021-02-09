using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	// TODO: Implement or discard!
	public class ChangeTargetFunction : DndFunction
	{
		public override string Name { get; set; } = "ChangeTarget";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 2);

			return null;
		}
	}
}
