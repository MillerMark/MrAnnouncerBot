using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	/// <summary>
	/// Gets the specified property of the player.
	/// </summary>
	public class AllFeaturesFunction : DndFunction
	{
		Feature feature;
		public override bool Handles(string tokenName)
		{
			feature = AllFeatures.Get(tokenName);
			return feature != null;
		}

		public override object Evaluate(List<string> args, ExpressionEvaluator evaluator, Character player)
		{
			if (feature == null)
				return null;

			return feature.ConditionsSatisfied(args, player);
		}
	}
}
