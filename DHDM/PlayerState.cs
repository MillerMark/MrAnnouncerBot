//#define profiling
using DndCore;
using System;
using System.Linq;

namespace DHDM
{
	public class PlayerStats
	{
		public bool ReadyToRollDice { get; set; }
		public VantageKind Vantage { get; set; }
		public int PlayerId { get; set; }
		public PlayerStats()
		{

		}

		public PlayerStats(int playerId)
		{
			PlayerId = playerId;
		}
	}
}

// HubtasticBaseStation.ChangePlayerHealth(JsonConvert.SerializeObject(damageHealthChange));