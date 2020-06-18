using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified rechargeable to the active player.")]
	[Param(1, typeof(string), "rechargeableName", "The name of the rechargeable.", ParameterIs.Required)]
	[Param(2, typeof(string), "variableName", "The name of the variable associated with this rechargeable.", ParameterIs.Required)]
	[Param(3, typeof(int), "maxValue", "The number of charges this item has.", ParameterIs.Required)]
	[Param(4, typeof(string), "cycle", "The time required for this item to recharge.", ParameterIs.Required)]
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
