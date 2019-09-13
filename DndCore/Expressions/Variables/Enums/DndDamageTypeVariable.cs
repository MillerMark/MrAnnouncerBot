using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndDamageTypeVariable : DndVariable
	{
		public override bool Handles(string tokenName, Character player)
		{
			return Enum.TryParse(tokenName, out DamageType result);
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			if (Enum.TryParse(variableName, out DamageType result))
				return result;
			return null;
		}
	}
}

