using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndSensesVariable : DndVariable
	{
		public override bool Handles(string tokenName, Character player)
		{
			return Enum.TryParse(tokenName, out Senses result);
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			if (Enum.TryParse(variableName, out Senses result))
				return result;
			return null;
		}
	}
}

