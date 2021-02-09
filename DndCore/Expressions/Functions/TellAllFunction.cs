using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Sends a specified message to both viewers and the dungeon master.")]
	[Param(1, typeof(string), "message", "The message to send.", ParameterIs.Required)]
	public class TellAllFunction : DndFunction
	{
		public override string Name => "TellAll";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string message = evaluator.Evaluate<string>(args[0]);

			player.Game.TellAll(message);

			return null;
		}
	}
}
