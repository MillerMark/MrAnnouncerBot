using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndCreatureKindsVariable : DndVariable
	{
		public override bool Handles(string tokenName, Character player)
		{
			return Enum.TryParse(tokenName, out CreatureKinds result);
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			if (Enum.TryParse(variableName, out CreatureKinds result))
				return result;
			return null;
		}
	}
}

