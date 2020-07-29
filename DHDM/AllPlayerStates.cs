//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class AllPlayerStats
	{
		public string LatestCommand { get; set; }
		public string LatestData { get; set; }
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

		PlayerStats GetPlayerState(int playerId)
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
			PlayerStats playerState = GetPlayerState(playerId);
			playerState.ReadyToRollDice = !playerState.ReadyToRollDice;
		}
	}
}

// HubtasticBaseStation.ChangePlayerHealth(JsonConvert.SerializeObject(damageHealthChange));