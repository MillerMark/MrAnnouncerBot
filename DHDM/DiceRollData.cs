using System;
using System.Linq;

namespace DHDM
{
	public class DiceRollData
	{
		public int playerID { get; set; }
		public bool success { get; set; }
		public int roll { get; set; }
		public int hiddenThreshold { get; set; }
		public int damage { get; set; }
		public DiceRollData()
		{

		}
	}
}
