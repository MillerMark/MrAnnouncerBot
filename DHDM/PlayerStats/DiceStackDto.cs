//#define profiling
using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public class DiceStackDto
	{
		public DiceStackDto()
		{
		}

		public DiceStackDto(Roll roll)
		{
			NumSides = roll.Sides;
			Multiplier = (int)Math.Floor(roll.Count);
			DamageType = DndUtils.ToDamage(roll.Descriptor);
		}

		public int NumSides { get; set; }
		public double HueShift { get; set; }
		public int Multiplier { get; set; } = 1;
		public DamageType DamageType { get; set; } = DamageType.None;
	}
}

// HubtasticBaseStation.ChangePlayerHealth(JsonConvert.SerializeObject(damageHealthChange));