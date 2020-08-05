using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Clears any Windups added with AddWindup for this player.")]
	public class ClearWindupFunction : DndFunction
	{
		public static event NameEventHandler RequestClearWindup;

		public override string Name { get; set; } = "ClearWindup";

		public static void OnRequestClearWindup(object sender, NameEventArgs ea)
		{
			RequestClearWindup?.Invoke(sender, ea);
		}
		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0);
			if (player != null)
				OnRequestClearWindup(player, new NameEventArgs(AddWindupFunction.GetWindupName(player, spell)));
			return null;
		}
	}
}
