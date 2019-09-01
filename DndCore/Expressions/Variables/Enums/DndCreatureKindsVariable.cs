using System;
using System.Linq;

namespace DndCore
{
	public class DndCreatureKindsVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out CreatureKinds result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out CreatureKinds result))
				return result;
			return null;
		}
	}
}

