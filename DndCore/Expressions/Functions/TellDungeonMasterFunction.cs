using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class TellDungeonMasterFunction : DndFunction
	{
		public override string Name => "TellDm";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string message = evaluator.Evaluate<string>(args[0]);

			player.Game.TellDungeonMaster(message);

			return null;
		}
	}
}
