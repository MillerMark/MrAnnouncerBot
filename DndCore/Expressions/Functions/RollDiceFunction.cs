using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class RollDiceFunction : DndFunction
	{
		public override string Name => "RollDice";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target, CastedSpell spell)
		{
			ExpectingArguments(args, 1);

			string diceStr = evaluator.Evaluate<string>(args[0]);

			player.ReadyRollDice(DiceRollType.None, diceStr);

			player.RollDiceNow();

			return null;
		}
	}
}
