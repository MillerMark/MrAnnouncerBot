using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	[Tooltip("Deactivates the specified Feature.")]
	[Param(1, typeof(string), "featureName", "The name of the feature to deactivate.", ParameterIs.Required)]
	public class DeactivateFeatureFunction : DndFunction
	{
		public override string Name { get; set; } = "DeactivateFeature";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Creature player, Target target, CastedSpell spell, DiceStoppedRollingData dice = null)
		{
			ExpectingArguments(args, 1);
			string featureName = args[0];
			if (featureName.StartsWith("\""))
				featureName = Expressions.GetStr(featureName);
			Feature feature = AllFeatures.Get(featureName);

			if (feature != null)
				feature.Deactivate("", player);

			return null;
		}
	}
}
