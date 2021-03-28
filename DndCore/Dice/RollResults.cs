using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DndCore
{
	public class RollResults
	{
		public string rollId { get; set; }
		public string spellId { get; set; }
		public string viewer { get; set; }
		public int singleOwnerId { get; set; }
		public bool success { get; set; }
		public bool wasCriticalHit { get; set; }
		public string spellName { get; set; }
		public DiceGroup diceGroup { get; set; }
		public int roll { get; set; }
		public int hiddenThreshold { get; set; }
		public int rawDamageRoll { get; set; }
		public int totalDamagePlusModifiers { get; set; }
		public int damageModOffset { get; set; }
		public int damageModMultiplier { get; set; }
		public int health { get; set; }
		public int extra { get; set; }
		public List<PlayerRoll> multiplayerSummary { get; set; }
		public List<IndividualRoll> individualRolls { get; set; }

		public DiceRollType type { get; set; }
		public Skills skillCheck { get; set; }
		public Ability savingThrow { get; set; }
		public int bonus { get; set; }
		public string additionalDieRollMessage { get; set; }

		public RollResults()
		{

		}

		public Character GetSingleRollingPlayer()
		{
			if (multiplayerSummary == null)
				return AllPlayers.GetFromId(singleOwnerId);

			if (multiplayerSummary.Count == 1)
				return AllPlayers.GetFromId(multiplayerSummary[0].id);

			return null;
		}
	}
}
