using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified die roll effects to the next roll for the active player.")]
	[Param(1, typeof(string), "dieRollEffects", "The name of the die roll effect (from the DieRollEffects Google sheet) to add.", ParameterIs.Required)]
	public class AddDieRollEffectsFunction : DndFunction
	{
		public override string Name => "AddDieRollEffects";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1);

			string dieRollEffects = evaluator.Evaluate<string>(args[0]);

			player.AddDieRollEffects(dieRollEffects);

			return null;
		}
	}
}
