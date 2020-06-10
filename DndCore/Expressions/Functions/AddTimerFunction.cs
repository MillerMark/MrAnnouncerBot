using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds a named Timer and calls the specified function after a specified number of milliseconds elapses.")]
	[Param(1, typeof(string), "timerName", "The name of the timer to set.")]
	[Param(2, typeof(int), "durationSeconds", "The number of seconds to wait until the timer expires.")]
	[Param(3, typeof(string), "functionToCall", "The function to call when the timer expires.")]
	public class AddTimerFunction : DndFunction
	{
		public override string Name { get; set; } = "AddTimer";

		
		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 3);
			string timerName = Expressions.GetStr(args[0], player, target, spell);
			int durationSeconds = Expressions.GetInt(args[1], player, target, spell);
			string functionToCall = Expressions.GetStr(args[2], player, target, spell);
			if (player == null)
				return null;
			if (player.Game == null)
				return null;
			DndAlarm dndAlarm = player.Game.Clock.CreateAlarm(TimeSpan.FromSeconds(durationSeconds), timerName, player, functionToCall);
			dndAlarm.AlarmFired += DndAlarm_AlarmFired;
			return null;
		}

		private void DndAlarm_AlarmFired(object sender, DndTimeEventArgs ea)
		{
			string functionToCall = (string)ea.Alarm.Data;
			Expressions.Do(functionToCall, ea.Alarm.Player);
		}
	}
}
