using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds a named Timer and calls the specified function after a specified number of milliseconds elapses.")]
	[Param(1, typeof(string), "timerName", "The name of the timer to set.", ParameterIs.Required)]
	[Param(2, typeof(int), "durationSeconds", "The number of seconds to wait until the timer expires.", ParameterIs.Required)]
	[Param(3, typeof(string), "functionToCall", "The function to call when the timer expires.", ParameterIs.Required)]
	public class AddTimerFunction : DndFunction
	{
		public override string Name { get; set; } = "AddTimer";

		
		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature creature, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 3);
			string timerName = Expressions.GetStr(args[0], creature, target, spell);
			int durationSeconds = Expressions.GetInt(args[1], creature, target, spell);
			string functionToCall = Expressions.GetStr(args[2], creature, target, spell);
			if (creature == null)
				return null;
			if (creature.Game == null)
				return null;
			DndAlarm dndAlarm = creature.Game.Clock.CreateAlarm(TimeSpan.FromSeconds(durationSeconds), timerName, creature, functionToCall);
			dndAlarm.AlarmFired += DndAlarm_AlarmFired;
			return null;
		}

		private void DndAlarm_AlarmFired(object sender, DndTimeEventArgs ea)
		{
			string functionToCall = (string)ea.Alarm.Data;
			Expressions.Do(functionToCall, ea.Alarm.Creature);
		}
	}
}
