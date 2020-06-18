using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Gets the level of the active player for the specified class.")]
	[Param(1, typeof(string), "value", "The value to halve.", ParameterIs.Required)]
	public class LevelFunction : DndFunction
	{
		public override string Name => "Level";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string className = evaluator.Evaluate<string>(args[0]);

			return player.GetLevel(className);
		}
	}
}
