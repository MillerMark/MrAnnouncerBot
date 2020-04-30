using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
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
			AddReminderEventArgs ea = new AddReminderEventArgs(Expressions.GetStr(args[0], player, target, spell), Expressions.GetStr(args[1], player, target, spell));
			AddReminder(player, ea);
			return null;
		}
	}
}
