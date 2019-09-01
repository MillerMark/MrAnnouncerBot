using System;
using System.Linq;

namespace DndCore
{
	public class DndConditionsVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out Conditions result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out Conditions result))
				return result;
			return null;
		}
	}
}

