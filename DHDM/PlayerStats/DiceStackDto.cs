//#define profiling
using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public class DiceStackDto
	{
		public int NumSides { get; set; }
		public double HueShift { get; set; }
		public int Multiplier { get; set; }
		public DamageType DamageType { get; set; }
	}
}

// HubtasticBaseStation.ChangePlayerHealth(JsonConvert.SerializeObject(damageHealthChange));