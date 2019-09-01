using System;
using System.Linq;

namespace DndCore
{
	public class DndSkillsVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out Skills result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out Skills result))
				return result;
			return null;
		}
	}
}

