using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DeactivateFeatureFunction : DndFunction
	{
		public override string Name { get; set; } = "DeactivateFeature";

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player, Creature target, CastedSpell spell)
		{
			ExpectingArguments(args, 1);
			Feature feature = AllFeatures.Get(args[0]);

			if (feature != null)
				feature.Deactivate("", player);

			return null;
		}
	}
}
