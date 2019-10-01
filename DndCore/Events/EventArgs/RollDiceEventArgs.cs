using System;
using System.Linq;

namespace DndCore
{
	public class RollDiceEventArgs : EventArgs
	{
		public RollDiceEventArgs(string diceRollStr)
		{
			DiceRollStr = diceRollStr;
		}

		public string DiceRollStr { get; set; }
	}
}