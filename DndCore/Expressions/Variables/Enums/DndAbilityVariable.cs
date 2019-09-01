using System;
using System.Linq;

namespace DndCore
{
	public class DndAbilityVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out Ability result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out Ability result))
				return result;
			return null;
		}
	}
}

