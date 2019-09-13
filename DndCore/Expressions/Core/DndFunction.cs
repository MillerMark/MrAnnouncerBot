using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public abstract class DndFunction: DndToken
	{
		public abstract object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player);

		protected void ExpectingArguments(List<string> args, int value)
		{
			if (args.Count != value)
				throw new Exception($"args.count ({args.Count}) does not match arguments sent ({value}).");
		}
	}
}

