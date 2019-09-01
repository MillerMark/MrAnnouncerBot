using System;
using System.Linq;

namespace DndCore
{
	public class DndVantageKindVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out VantageKind result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out VantageKind result))
				return result;
			return null;
		}
	}
}

