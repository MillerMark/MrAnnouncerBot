using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	/// <summary>
	/// Activates the specified feature.
	/// </summary>
	[Tooltip("Activates a specified Feature.")]
	[Param(1, typeof(string), "featureName", "The name of the Feature to activate.", ParameterIs.Required)]
	public class ActivateFeatureFunction : DndFunction
	{
		public override string Name { get; set; } = "ActivateFeature";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, RollResults dice = null)
		{
			ExpectingArguments(args, 1);
			Feature feature = AllFeatures.Get(args[0]);

			if (feature != null)
				feature.Activate("", player, true);

			return null;
		}
	}
}
