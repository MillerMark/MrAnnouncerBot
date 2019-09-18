using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndWeaponPropertiesVariable : DndVariable
	{
		public override bool Handles(string tokenName, Character player, CastedSpell castedSpell = null)
		{
			return Enum.TryParse(tokenName, out WeaponProperties result);
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			if (Enum.TryParse(variableName, out WeaponProperties result))
				return result;
			return null;
		}
	}
}

