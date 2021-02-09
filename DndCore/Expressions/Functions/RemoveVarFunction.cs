using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Removes the specified state variable.")]
	[Param(1, typeof(string), "varName", "The name of the state variable to remove.", ParameterIs.Required)]
	public class RemoveVarFunction : DndFunction
	{
		public override string Name => "RemoveVar";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string varName = args[0];
			player.RemoveStateVar(varName);
			return null;
		}
	}
}
