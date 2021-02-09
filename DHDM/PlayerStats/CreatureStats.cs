//#define profiling
using DndCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class CreatureStats
	{
		public List<DiceStackDto> DiceStack { get; set; } = new List<DiceStackDto>();
		public bool ReadyToRollDice { get; set; }
		public VantageKind Vantage { get; set; }
		public Conditions Conditions { get; set; }
		public int CreatureId { get; set; }
		public string ConcentratedSpell { get; set; }
		public int ConcentratedSpellDurationSeconds { get; set; }
		public double PercentConcentrationComplete { get; set; }
		public bool JustBrokeConcentration { get; set; }
		public bool IsTargeted { get; set; }
		public CreatureStats()
		{

		}

		public CreatureStats(int creatureId)
		{
			CreatureId = creatureId;
		}

		public void AddRoll(Roll roll)
		{
			DiceStack.Add(new DiceStackDto(roll));
		}

		public void AddD20()
		{
			DiceStack.Add(new DiceStackDto() { NumSides = 20 });
		}
		public void ClearDiceStack()
		{
			DiceStack.Clear();
		}
	}
}

// HubtasticBaseStation.ChangePlayerHealth(JsonConvert.SerializeObject(damageHealthChange));