using System;
using DndCore;
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
				StopRollingData = null;
			else
				try
				{
					StopRollingData = Newtonsoft.Json.JsonConvert.DeserializeObject<DiceStoppedRollingData>(diceData);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debugger.Break();
					Console.WriteLine(ex.Message);
				}
			DiceData = diceData;
		}

		public string DiceData { get; set; }
		public DiceStoppedRollingData StopRollingData { get; set; }
	}
}
