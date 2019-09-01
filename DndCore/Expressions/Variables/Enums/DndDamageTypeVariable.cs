using System;
using System.Linq;

namespace DndCore
{
	public class DndDamageTypeVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out DamageType result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out DamageType result))
				return result;
			return null;
		}
	}
}

