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
		public string spellName { get; set; }
		public int roll { get; set; }
		public int hiddenThreshold { get; set; }
		public int damage { get; set; }
		public int health { get; set; }
		public int extra { get; set; }
		public List<PlayerRoll> multiplayerSummary { get; set; }
		public List<IndividualRoll> individualRolls { get; set; }

		public DiceRollType type { get; set; }
		public Skills skillCheck { get; set; }
		public Ability savingThrow { get; set; }
		public int bonus { get; set; }
		public string additionalDieRollMessage { get; set; }

		public DiceRollData()
		{

		}

		public Character GetSingleRollingPlayer()
		{
			if (multiplayerSummary == null)
				return AllPlayers.GetFromId(playerID);

			if (multiplayerSummary.Count == 1)
				return AllPlayers.GetFromId(multiplayerSummary[0].playerId);

			return null;
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
