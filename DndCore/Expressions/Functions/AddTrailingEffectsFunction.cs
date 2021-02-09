using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Adds the specified trailing effects to the active player's next die roll.")]
	[Param(1, typeof(string), "trailingEffects", "The list of trailing effects to add (separated by semicolons).", ParameterIs.Required, CompletionProviderNames.TrailingEffectName)]
	public class AddTrailingEffectsFunction : DndFunction
	{
		public override string Name => "AddTrailingEffects";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);

			string trailingEffects = evaluator.Evaluate<string>(args[0]);

			player.AddTrailingEffects(trailingEffects);

			return null;
		}
	}
}
