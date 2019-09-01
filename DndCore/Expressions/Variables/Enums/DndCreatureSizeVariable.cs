using System;
using System.Linq;

namespace DndCore
{
	public class DndCreatureSizeVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out CreatureSize result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out CreatureSize result))
				return result;
			return null;
		}
	}
}

