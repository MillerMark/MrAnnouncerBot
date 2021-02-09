using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Tosses the dice for the specified roll immediately.")]
	[Param(1, typeof(string), "diceStr", "The die string to roll (e.g., \"4d8(force)\").", ParameterIs.Required)]
	public class RollDiceFunction : DndFunction
	{
		public override string Name => "RollDice";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string diceStr = evaluator.Evaluate<string>(args[0]);

			player.ReadyRollDice(DiceRollType.None, diceStr);

			player.RollDiceNow();

			return null;
		}
	}
}
