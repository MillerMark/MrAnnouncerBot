using System;
using System.Linq;

namespace DndCore
{
	public class DndAttackTypeVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out AttackType result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out AttackType result))
				return result;
			return null;
		}
	}
}

