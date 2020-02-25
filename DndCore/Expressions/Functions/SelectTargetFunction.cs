using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class SelectTargetFunction : DndFunction
	{
		public override string Name { get; set; } = "SelectTarget";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target, CastedSpell spell)
		{
			ExpectingArguments(args, 0);

			player.ActiveTarget = player.Game.GetPlayerFromId(0);

			return null;
		}
	}
}
