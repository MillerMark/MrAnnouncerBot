using System;
using System.Linq;

namespace DndCore
{
	public class DndRechargeOddsVariable : DndVariable
	{
		public override bool Handles(string tokenName)
		{
			return Enum.TryParse(tokenName, true, out RechargeOdds result);
		}

		public override object GetValue(string variableName, Character player)
		{
			if (Enum.TryParse(variableName, true, out RechargeOdds result))
				return result;
			return null;
		}
	}
}

