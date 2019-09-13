using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndTimePointVariable : DndVariable
	{
		public override bool Handles(string tokenName, Character player)
		{
			return Enum.TryParse(tokenName, out TimePoint result);
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			if (Enum.TryParse(variableName, out TimePoint result))
				return result;
			return null;
		}
	}
}

