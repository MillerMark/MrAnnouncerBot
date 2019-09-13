using System;
using System.Linq;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	//TODO: Wil says move all of these into an ancestor class and descendants only specify the type.
	public class DndAbilityVariable : DndVariable
	{
		public override bool Handles(string tokenName, Character player)
		{
			return Enum.TryParse(tokenName, out Ability result);
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			if (Enum.TryParse(variableName, out Ability result))
				return result;
			return null;
		}
	}
}

