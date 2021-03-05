//#define profiling
using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class PlayerStatManager
	{
		public string LatestCommand { get; set; }
		public string LatestData { get; set; }
		public bool RollingTheDiceNow { get; set; }
		public bool HideSpellScrolls { get; set; }
		public int ActiveTurnCreatureID { get; set; }  // Negative numbers are for in-game creatures (not players)
		

		List<CreatureStats> players;
		public List<CreatureStats> Players
		{
			get
			{
				if (players == null)
					players = new List<CreatureStats>();
				return players;
			}

			set
			{
				players = value;
			}
		}

		public PlayerStatManager()
		{

		}

		public bool AnyoneIsReadyToRoll => Players.FirstOrDefault(x => x.ReadyToRollDice) != null;
		

		public CreatureStats GetPlayerStats(int creatureId)
		{
			CreatureStats foundPlayer = Players.FirstOrDefault(x => x.CreatureId == creatureId);
			if (foundPlayer != null)
				return foundPlayer;

			CreatureStats playerState = new CreatureStats(creatureId);
			Players.Add(playerState);
			return playerState;
		}

		public void ToggleReadyRollD20(int playerId)
		{
			CreatureStats playerStats = GetPlayerStats(playerId);
			playerStats.DiceStack.Clear();
			playerStats.AddD20();

			if (playerStats.Vantage == VantageKind.Normal || !playerStats.ReadyToRollDice)
				playerStats.ReadyToRollDice = !playerStats.ReadyToRollDice;
			else
				playerStats.Vantage = VantageKind.Normal;
		}

		public void ToggleCondition(int playerId, Conditions conditions)
		{
			CreatureStats playerStats = GetPlayerStats(playerId);
			if (playerStats.Conditions.HasFlag(conditions))  // Bit is set.
				playerStats.Conditions &= ~conditions;  // clear the bit
			else
				playerStats.Conditions |= conditions;
		}

		public void ClearConditions(int playerId)
		{
			CreatureStats playerStats = GetPlayerStats(playerId);
			playerStats.Conditions = Conditions.None;
		}

		public void SetReadyRollDice(int playerId, bool newValue, DieRollDetails dieRollDetails)
		{
			CreatureStats playerState = GetPlayerStats(playerId);
			playerState.ReadyToRollDice = newValue;
			playerState.ClearDiceStack();
			foreach (Roll roll in dieRollDetails.Rolls)
				playerState.AddRoll(roll);
		}

		public void ReadyRollVantage(int playerId, VantageKind vantage)
		{
			CreatureStats playerState = GetPlayerStats(playerId);
			playerState.ReadyToRollDice = true;
			if (playerState.DiceStack.Count == 0)
				playerState.AddD20();
			playerState.Vantage = vantage;
		}

		public List<int> GetReadyToRollPlayerIds()
		{
			return Players.Where(x => x.ReadyToRollDice).Select(x => x.CreatureId).ToList();
		}
		
		public void ClearReadyToRollState()
		{
			foreach (CreatureStats playerStats in Players)
			{
				playerStats.ReadyToRollDice = false;
				playerStats.Vantage = VantageKind.Normal;
			}
		}
		
		public void ReadyRollDice(string data)
		{
			bool newReadyState = false;
			if (data == "All")
				newReadyState = true;
			foreach (CreatureStats playerStats in Players)
			{
				playerStats.ClearDiceStack();
				playerStats.Vantage = VantageKind.Normal;
				playerStats.ReadyToRollDice = newReadyState;
				if (playerStats.ReadyToRollDice)
					playerStats.AddD20();
			}
		}

		public void ClearAllActiveTurns()
		{
			ActiveTurnCreatureID = -1;
		}

		public void ClearAll()
		{
			ClearAllActiveTurns();
			Players = null;
		}

		public void ToggleTarget(int playerId)
		{
			CreatureStats playerStats = GetPlayerStats(playerId);
			playerStats.IsTargeted = !playerStats.IsTargeted;
		}

		public void ClearAllTargets()
		{
			foreach (CreatureStats playerStats in Players)
				playerStats.IsTargeted = false;
		}

		public void TargetAll()
		{
			foreach (CreatureStats playerStats in Players)
				playerStats.IsTargeted = true;
		}

		public List<CreatureStats> GetTargeted()
		{
			return Players.Where(x => x.IsTargeted).ToList();
		}

		public bool HasOnlyOnePlayerReadyToRollDice()
		{
			return Players.Count(x => x.ReadyToRollDice) == 1;
		}

		public int GetFirstPlayerIdWhoIsReadyToRoll()
		{
			return Players.FirstOrDefault(x => x.ReadyToRollDice).CreatureId;
		}
	}
}

// HubtasticBaseStation.ChangePlayerHealth(JsonConvert.SerializeObject(damageHealthChange));