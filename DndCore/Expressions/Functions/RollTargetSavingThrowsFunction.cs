using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Rolls Saving Throws for all targeted creatures.")]
	public class RollTargetSavingThrowsFunction : DndFunction
	{
		public const string CMD_RollTargetSavingThrows = "!!RollTargetSavingThrows";
		public override string Name => "RollTargetSavingThrows";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Target target, CastedSpell spell, DiceStoppedRollingData rollTargetSavingThrows = null)
		{
			ExpectingArguments(args, 0);

			player.ReadyRollDice(DiceRollType.None, CMD_RollTargetSavingThrows);

			player.RollDiceNow();

			return null;
		}
	}
}
