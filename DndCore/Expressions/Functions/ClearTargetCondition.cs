using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Clears (removes) the specified condition to the target.")]
	[Param(1, typeof(Conditions), "condition", "The condition to clear.", ParameterIs.Required)]
	public class ClearTargetCondition : DndFunction
	{
		public override string Name { get; set; } = "ClearTargetCondition";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);
			// TODO: Get the condition to clear.
			// TODO: Clear the target's condition.

			return null;
		}
	}
}
