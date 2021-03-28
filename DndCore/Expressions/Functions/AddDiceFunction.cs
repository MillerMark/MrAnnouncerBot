﻿using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified dice to the next roll for the active player.")]
	[Param(1, typeof(string), "diceStr", "The dice to add (e.g., \"2d4(cold)\").", ParameterIs.Required)]
	public class AddDiceFunction : DndFunction
	{
		public override string Name => "AddDice";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1);

			string diceStr = evaluator.Evaluate<string>(args[0]);

			player.AddDice(diceStr);

			return null;
		}
	}
}
