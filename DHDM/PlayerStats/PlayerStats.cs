//#define profiling
using DndCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class PlayerStats
	{
		public List<DiceStackDto> DiceStack { get; set; } = new List<DiceStackDto>();
		public bool ReadyToRollDice { get; set; }
		public VantageKind Vantage { get; set; }
		public int PlayerId { get; set; }
		public int MainDieSides { get; set; } = 20;
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