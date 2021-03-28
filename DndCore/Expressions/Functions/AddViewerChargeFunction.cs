using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified number of specified charges to the specified Viewer.")]
	[Param(1, typeof(string), "userName", "The user name of the viewer who will receive the charge.", ParameterIs.Required)]
	[Param(2, typeof(string), "chargeName", "The name of the charge to add.", ParameterIs.Required)]
	[Param(3, typeof(int), "chargeCount", "The number of charges to add.", ParameterIs.Required)]
	public class AddViewerChargeFunction : DndFunction
	{
		public static event RequestAddViewerChargeEventHandler RequestAddViewerCharge;
		public override string Name => "AddViewerCharge";

		public static void OnRequestAddViewerCharge(RequestAddViewerChargeEventArgs ea)
		{
			RequestAddViewerCharge?.Invoke(null, ea);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 3);

			string userName = Expressions.GetStr(args[0], player, target, spell);
			string chargeName = Expressions.GetStr(args[1], player, target, spell);
			int chargeCount = Expressions.GetInt(args[2], player, target, spell);
			OnRequestAddViewerCharge(new RequestAddViewerChargeEventArgs(userName, chargeName, chargeCount));
			return null;
		}
	}
}
