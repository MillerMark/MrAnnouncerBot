using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Removes the specified timer.")]
	[Param(1, typeof(string), "timerName", "The name of the timer to remove.", ParameterIs.Required)]
	public class RemoveTimerFunction : DndFunction
	{
		public override string Name { get; set; } = "RemoveTimer";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1);
			string timerName = Expressions.GetStr(args[0], player, target, spell);

			if (player == null)
				return null;
			if (player.Game == null)
				return null;
			player.Game.Clock.RemoveAlarm(timerName);
			return null;
		}
	}
}
