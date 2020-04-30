using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class AddRechargeableFunction : DndFunction
	{
		public override string Name => "AddRechargeable";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 4);

			string rechargeableName = Expressions.GetStr(args[0], player, target, spell);
			string variableName = Expressions.GetStr(args[1], player, target, spell);
			int maxValue = Expressions.GetInt(args[2], player, target, spell);
			string cycle = Expressions.GetStr(args[3], player, target, spell);

			player.AddRechargeable(rechargeableName, variableName, maxValue, cycle);

			return null;
		}
	}
}
