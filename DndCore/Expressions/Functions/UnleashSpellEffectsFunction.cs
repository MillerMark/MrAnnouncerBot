using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Unleashes any backed-up spell effects (visual and audible) for the active player.")]
	public class UnleashSpellEffectsFunction : DndFunction
	{
		public static event EventHandler<int> RequestUnleashSpellEffects;
		public override string Name { get; set; } = "UnleashSpellEffects";
		
		public static void OnRequestUnleashSpellEffects(object sender, int e)
		{
			RequestUnleashSpellEffects?.Invoke(sender, e);
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target = null, CastedSpell spell = null, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 0);
			if (player != null)
				OnRequestUnleashSpellEffects(player, player.playerID);

			return null;
		}
	}
}
