using System;
using System.Linq;

namespace DndCore
{
	public class DndSpellTypeVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out SpellType result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out SpellType result))
				return result;
			return null;
		}
	}
}

