using System;
using System.Linq;

namespace DndCore
{
	public class DndTimeMeasureVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, out TimeMeasure result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, out TimeMeasure result))
				return result;
			return null;
		}
	}
}

