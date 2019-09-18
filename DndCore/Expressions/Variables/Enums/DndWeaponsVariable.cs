using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndWeaponsVariable : DndVariable
	{
		public override bool Handles(string tokenName, Character player, CastedSpell castedSpell = null)
		{
			return Enum.TryParse(tokenName, out Weapons result);
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			if (Enum.TryParse(variableName, out Weapons result))
				return result;
			return null;
		}
	}
}

