using System;
using System.Linq;

namespace DndCore
{
	public class DndWeaponPropertiesVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out WeaponProperties result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out WeaponProperties result))
				return result;
			return null;
		}
	}
}

