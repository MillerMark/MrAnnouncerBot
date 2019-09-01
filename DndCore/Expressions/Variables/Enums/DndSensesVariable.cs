using System;
using System.Linq;

namespace DndCore
{
	public class DndSensesVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out Senses result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out Senses result))
				return result;
			return null;
		}
	}
}

