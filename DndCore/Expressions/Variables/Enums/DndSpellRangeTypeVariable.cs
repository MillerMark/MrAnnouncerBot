using System;
using System.Linq;

namespace DndCore
{
	public class DndSpellRangeTypeVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out SpellRangeType result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out SpellRangeType result))
				return result;
			return null;
		}
	}
}

