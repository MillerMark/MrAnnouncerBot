using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
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
