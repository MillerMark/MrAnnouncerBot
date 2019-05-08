using System;
using System.Linq;

namespace DHDM
{
	public class DiceEventArgs : EventArgs
	{
		public DiceEventArgs()
		{
		}
		public DiceEventArgs(string diceData)
		{
			// TODO: JSON Deserialize
			DiceData = diceData;
		}

		public void SetDiceData(string diceData)
		{
			if (diceData == null)
				DiceRollData = null;
			else
				DiceRollData = Newtonsoft.Json.JsonConvert.DeserializeObject<DiceRollData>(diceData);
			DiceData = diceData;
		}

		public string DiceData { get; set; }
		public DiceRollData DiceRollData { get; set; }
	}
}
