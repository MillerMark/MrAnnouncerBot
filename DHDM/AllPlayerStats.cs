//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class AllPlayerStats
	{
		public string LatestCommand { get; set; }
		public string LatestData { get; set; }
		public bool RollingTheDiceNow { get; set; }

		List<PlayerStats> players;
		public List<PlayerStats> Players
		{
			get
			{
				if (players == null)
					players = new List<PlayerStats>();
				return players;
			}

			set
			{
				players = value;
			}
		}

		public AllPlayerStats()
		{

		}

		public bool AnyoneIsReadyToRoll => Players.FirstOrDefault(x => x.ReadyToRollDice) != null;

		public PlayerStats GetPlayerStats(int playerId)
		{
			PlayerStats foundPlayer = Players.FirstOrDefault(x => x.PlayerId == playerId);
			if (foundPlayer != null)
				return foundPlayer;

			PlayerStats playerState = new PlayerStats(playerId);
			Players.Add(playerState);
			return playerState;
		}

		public void ToggleReadyRollDice(int playerId)
		{
			PlayerStats playerState = GetPlayerStats(playerId);

			if (playerState.Vantage == VantageKind.Normal || !playerState.ReadyToRollDice)
				playerState.ReadyToRollDice = !playerState.ReadyToRollDice;
			else
				playerState.Vantage = VantageKind.Normal;
		}

		public void SetReadyRollDice(int playerId, bool newValue)
		{
			PlayerStats playerState = GetPlayerStats(playerId);
			playerState.ReadyToRollDice = newValue;
		}

		public void ReadyRollVantage(int playerId, VantageKind vantage)
		{
			PlayerStats playerState = GetPlayerStats(playerId);

			playerState.ReadyToRollDice = true;
			playerState.Vantage = vantage;
		}

		public List<int> GetReadyToRollPlayerIds()
		{
			return Players.Where(x => x.ReadyToRollDice).Select(x => x.PlayerId).ToList();
		}
		
		public void ClearReadyToRollState()
		{
			foreach (PlayerStats playerStats in Players)
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
			foreach (PlayerStats playerStats in Players)
			{
				playerStats.ReadyToRollDice = newReadyState;
			}
		}
	}
}

// HubtasticBaseStation.ChangePlayerHealth(JsonConvert.SerializeObject(damageHealthChange));