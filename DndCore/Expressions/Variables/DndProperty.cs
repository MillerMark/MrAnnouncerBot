using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndProperty : DndVariable
	{
		public override bool Handles(string tokenName, Character player)
		{
			PropertyDto property = AllProperties.Get(tokenName);
			return property != null;
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			PropertyDto property = AllProperties.Get(variableName);
			return evaluator.Evaluate(property.Expression);
		}
	}
}

