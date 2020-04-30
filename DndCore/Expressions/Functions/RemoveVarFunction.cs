using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class RemoveVarFunction : DndFunction
	{
		public override string Name => "RemoveVar";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string varName = args[0];
			player.RemoveStateVar(varName);
			return null;
		}
	}
}
