using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Gets the value of last roll with the specified name.")]
	[Param(1, typeof(string), "rollName", "The saved name of the roll to check. For example, RollDice(\"1d8(superiority:BarbarianWildSurge)\") will roll the dice and save the result of the roll with the name \"BarbarianWildSurge\".", ParameterIs.Required)]
	public class GetRoll : DndFunction
	{
		public static event GetRollEventHandler GetRollRequest;

		public static void OnGetRollRequest(object sender, GetRollEventArgs ea)
		{
			GetRollRequest?.Invoke(sender, ea);
		}
		public override string Name => "GetRoll";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1);

			GetRollEventArgs ea = new GetRollEventArgs((string)args[0]);
			OnGetRollRequest(this, ea);

			return ea.Result;
		}
	}
}
