using System;
using System.Collections.Generic;
using System.Reflection;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Sets visibility of the active creature to the specified value.")]
	[Param(1, typeof(bool), "isVisible", "The creature's visibility. True for visible or false for invisible.", ParameterIs.Required)]

	public class SetCreatureVisibilityFunction : DndFunction
	{
		public override string Name => "SetCreatureVisibility";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature creature, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1);

			bool isVisible = Expressions.GetBool(args[0]);
			if (creature != null)
				creature.Visible = isVisible;
			return null;
		}
	}
}
