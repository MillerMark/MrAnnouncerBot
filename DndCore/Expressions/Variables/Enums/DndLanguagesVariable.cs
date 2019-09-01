using System;
using System.Linq;

namespace DndCore
{
	public class DndLanguagesVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out Languages result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out Languages result))
				return result;
			return null;
		}
	}
}

