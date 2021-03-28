using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Dispels the casted spell for the active player.")]
	public class DispelFunction : DndFunction
	{
		public override string Name => "Dispel";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 0);
			CastedSpell castedSpell = Expressions.GetCastedSpell(evaluator.Variables);

			if (castedSpell != null && player != null)
				player.Dispel(castedSpell);

			return null;
		}
	}
}
