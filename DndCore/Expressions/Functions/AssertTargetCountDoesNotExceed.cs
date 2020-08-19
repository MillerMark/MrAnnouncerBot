using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{

	[Tooltip("Ensures the target count is less than the specified value.")]
	[Param(1, typeof(Target), "targetInstance", "The Target instance to check.", ParameterIs.Required)]
	[Param(2, typeof(int), "maxTargets", "The maximum number of targets allowed.", ParameterIs.Required)]
	[Param(3, typeof(string), "displayText", "The text to display if targets exceeds maximum count.", ParameterIs.Required)]
	public class AssertTargetCountDoesNotExceed : DndFunction
	{
		public override string Name { get; set; } = "AssertTargetCountDoesNotExceed";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 3);
			Target targetInstance = evaluator.Evaluate<Target>(args[0]);  // evaluator.Evaluate call needed to get local variables.
			int maxTargets = Expressions.GetInt(args[1].Trim(), player, target, spell, dice);
			string displayText = Expressions.GetStr(args[2], player, target, spell, dice);
			// TODO: consider moving "Too many targets!" out to calling script code.
			Validation.AssertTrue(targetInstance.Count <= maxTargets, displayText, "Too many targets!");
			return null;
		}
	}
}
