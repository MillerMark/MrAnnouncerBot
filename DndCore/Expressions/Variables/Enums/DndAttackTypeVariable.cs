using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndAttackTypeVariable : DndVariable
	{
		public override bool Handles(string tokenName, Character player, CastedSpell castedSpell)
		{
			return Enum.TryParse(tokenName, out AttackType result);
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			if (Enum.TryParse(variableName, out AttackType result))
				return result;
			return null;
		}
	}
}

