using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DispelFunction : DndFunction
	{
		public override string Name => "Dispel";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target, CastedSpell spell)
		{
			ExpectingArguments(args, 0);
			CastedSpell castedSpell = Expressions.GetCastedSpell(evaluator.Variables);

			if (castedSpell != null && player != null)
				player.Dispel(castedSpell);

			return null;
		}
	}
}
