﻿using System;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	/// <summary>
	/// Determines if the specified feature is active.
	/// </summary>
	public class FeatureSatisfiedFunction : DndFunction
	{
		Feature feature;
		public override bool Handles(string tokenName, Character player)
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