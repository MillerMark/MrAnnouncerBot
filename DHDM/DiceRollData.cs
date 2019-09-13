using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class DiceRollData
	{
		public int playerID { get; set; }
		public bool success { get; set; }
		public int roll { get; set; }
		public int hiddenThreshold { get; set; }
		public int damage { get; set; }
		public int health { get; set; }
		public int extra { get; set; }
		public List<PlayerRoll> multiplayerSummary { get; set; }
		public DiceRollType type { get; set; }
		public Skills skillCheck { get; set; }
		public Ability savingThrow { get; set; }
		public int bonus { get; set; }
		public string additionalDieRollMessage { get; set; }

		public DiceRollData()
		{

		}
	}

	public class PlayerRoll
	{
		public int roll;
		public string name;
		public int playerId;
		public int modifier;
	}
}
