using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	/// <summary>
	/// Activates the specified feature.
	/// </summary>
	public class ActivateFeatureFunction : DndFunction
	{
		public override string Name { get; set; } = "ActivateFeature";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target, CastedSpell spell)
		{
			ExpectingArguments(args, 1);
			Feature feature = AllFeatures.Get(args[0]);

			if (feature != null)
				feature.Activate("", player);

			return null;
		}
	}
}
