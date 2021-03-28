using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	/// <summary>
	/// Determines if the specified feature is active.
	/// </summary>
	// TODO: Integrate this into the script editor's Code Completion.
	public class FeatureSatisfiedFunction : DndFunction
	{
		Feature feature;
		public override bool Handles(string tokenName, Creature player, CastedSpell castedSpell = null)
		{
			feature = AllFeatures.Get(tokenName);
			return feature != null;
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			if (feature == null)
				return null;

			return feature.ShouldActivateNow(args, player);
		}
	}
}
