using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Sets (adds) the specified condition to the target.")]
	[Param(1, typeof(Conditions), "condition", "The condition to set.", ParameterIs.Required)]
	public class SetTargetCondition : DndFunction
	{
		public override string Name { get; set; } = "SetTargetCondition";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, RollResults dice = null)
		{
			ExpectingArguments(args, 1);
			// TODO: Get the condition to set.
			// TODO: Set the target's condition.

			return null;
		}
	}
}
