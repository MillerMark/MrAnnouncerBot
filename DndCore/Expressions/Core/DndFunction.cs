using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;
using System.Threading.Tasks;

namespace DndCore
{
	public abstract class DndFunction: DndToken
	{
		public abstract object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature creature, Target target, CastedSpell spell, RollResults dice);

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

		public bool IsAsync { get; set; } = false;
	}
}

