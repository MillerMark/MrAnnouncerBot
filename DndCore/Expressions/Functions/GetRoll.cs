using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class GetRoll : DndFunction
	{
		public static event GetRollEventHandler GetRollRequest;

		public static void OnGetRollRequest(object sender, GetRollEventArgs ea)
		{
			GetRollRequest?.Invoke(sender, ea);
		}
		public override string Name => "GetRoll";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target, CastedSpell spell)
		{
			ExpectingArguments(args, 1);

			GetRollEventArgs ea = new GetRollEventArgs((string)args[0]);
			OnGetRollRequest(this, ea);

			return ea.Result;
		}
	}
}
