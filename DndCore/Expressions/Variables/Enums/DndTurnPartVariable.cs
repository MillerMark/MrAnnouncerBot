using System;
using System.Linq;

namespace DndCore
{
	public class DndTurnPartVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out TurnPart result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out TurnPart result))
				return result;
			return null;
		}
	}
}

