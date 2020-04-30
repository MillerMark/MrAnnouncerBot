using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public abstract class DndFunction: DndToken
	{
		//,  = null, CastedSpell spell = null
		public abstract object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice);

		protected void ExpectingArguments(List<string> args, int value)
		{
			if (args.Count != value)
				throw new Exception($"args.count ({args.Count}) does not match arguments sent ({value}).");
		}
		protected void ExpectingArguments(List<string> args, int minValue, int maxValue)
		{
			if (args.Count < minValue || args.Count > maxValue)
				throw new Exception($"args.count ({args.Count}) must be between {minValue} and {maxValue}.");
		}
	}
}

