using System;
using System.Linq;

namespace DndCore
{
	public class DndTimePointVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out TimePoint result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out TimePoint result))
				return result;
			return null;
		}
	}
}

