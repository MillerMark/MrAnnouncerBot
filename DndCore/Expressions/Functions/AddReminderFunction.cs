using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Shows a message in the DM chat window at the specified time.")]
	[Param(1, typeof(string), "reminder", "The message to show the DM.")]
	[Param(2, typeof(string), "fromNowDuration", "The amount of time to wait until the reminder is shown (e.g., \"1 round\", \"end of turn\", etc.).")]
	public class AddReminderFunction : DndFunction
	{
		public static event AddReminderEventHandler AddReminderRequest;
		public static void AddReminder(object sender, AddReminderEventArgs ea)
		{
			AddReminderRequest?.Invoke(sender, ea);
		}
		public override string Name { get; set; } = "AddReminder";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 2);
			string reminder = Expressions.GetStr(args[0], player, target, spell);
			string fromNowDuration = Expressions.GetStr(args[1], player, target, spell);
			AddReminderEventArgs ea = new AddReminderEventArgs(reminder, fromNowDuration);
			AddReminder(player, ea);
			return null;
		}
	}
}
