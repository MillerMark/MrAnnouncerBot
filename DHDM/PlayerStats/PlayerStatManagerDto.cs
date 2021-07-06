//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class PlayerStatManagerDto
	{
		public string LatestCommand { get; set; }
		public string LatestData { get; set; }
		public bool RollingTheDiceNow { get; set; }
		public bool HideSpellScrolls { get; set; }
		public int ActiveTurnCreatureID { get; set; }  // Negative numbers are for in-game creatures (not players)
		public List<CreatureStats> Players { get; set; }
	}
}

// HubtasticBaseStation.ChangePlayerHealth(JsonConvert.SerializeObject(damageHealthChange));