using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndProperty : DndVariable
	{
		public override bool Handles(string tokenName, Character player, CastedSpell castedSpell)
		{
			PropertyDto property = AllProperties.Get(tokenName);
			if (property != null)
				return true;

			if (knownProps.ContainsKey(tokenName))
				return true;

			return false;
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			PropertyDto property = AllProperties.Get(variableName);
			if (property != null)
				return evaluator.Evaluate(property.Expression);
			if (knownProps.ContainsKey(variableName))
				return knownProps[variableName];
			return null;
		}

		static Dictionary<string, object> knownProps = new Dictionary<string, object>();

		public static void Set(string propertyName, object value)
		{
			if (knownProps.ContainsKey(propertyName))
				knownProps[propertyName] = value;
			else
				knownProps.Add(propertyName, value);
		}
	}
}

