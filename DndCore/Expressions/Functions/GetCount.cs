using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Returns the number of entries in the specified entity.")]
	[Param(1, typeof(Target), "targetInstance", "The Target instance to check.", ParameterIs.Required)]
	public class GetCount : DndFunction
	{
		public override string Name { get; set; } = "GetCount";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);
			Target targetInstance = evaluator.Evaluate<Target>(args[0]);  // evaluator.Evaluate call needed to get local variables.
			if (targetInstance != null)
				return targetInstance.Count;
			return 0;
		}
	}
}
