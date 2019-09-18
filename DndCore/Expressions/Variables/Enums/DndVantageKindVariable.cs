using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndVantageKindVariable : DndVariable
	{
		public override bool Handles(string tokenName, Character player, CastedSpell castedSpell)
		{
			return Enum.TryParse(tokenName, out VantageKind result);
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			if (Enum.TryParse(variableName, out VantageKind result))
				return result;
			return null;
		}
	}
}

