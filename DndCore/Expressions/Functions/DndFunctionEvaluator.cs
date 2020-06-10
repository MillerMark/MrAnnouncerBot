using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	// TODO: support code completion with Tooltip 
	public class DndFunctionEvaluator : DndFunction
	{
		FunctionDto function;
		public override bool Handles(string tokenName, Character player, CastedSpell castedSpell)
		{
			function = AllFunctions.Get(tokenName);
			return function != null;
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			if (function == null)
				return null;
			return evaluator.Evaluate(DndUtils.InjectParameters(function.Expression, function.Parameters, args));
		}
	}
}

