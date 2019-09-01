using System;
using System.Linq;

namespace DndCore
{
	public class DndModTypeVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out ModType result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out ModType result))
				return result;
			return null;
		}
	}
}

