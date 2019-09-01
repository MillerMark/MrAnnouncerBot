using System;
using System.Linq;

namespace DndCore
{
	public class DndWeaponsVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out Weapons result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out Weapons result))
				return result;
			return null;
		}
	}
}

