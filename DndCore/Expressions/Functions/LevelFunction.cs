using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[ReturnType(typeof(int))]
	[Tooltip("Gets the level of the active player for the specified class.")]
	[Param(1, typeof(string), "class", "The class from which to check the level", ParameterIs.Required)]
	public class LevelFunction : DndFunction
	{
		public override string Name => "Level";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature creature, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string className = evaluator.Evaluate<string>(args[0]);
			if (creature is Character player)
				return player.GetLevel(className);
			return creature.Level;
		}
	}
}
