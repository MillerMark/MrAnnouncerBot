using System;
using System.Linq;

namespace DndCore
{
	public class DiceDto
	{
		public int Quantity { get; set; }
		public int Sides { get; set; }
		public int CreatureId { get; set; }
		public bool IsMagic { get; set; }
		public double Modifier { get; set; }
		public string Label { get; set; }

		public VantageKind Vantage { get; set; } = VantageKind.Normal;
		public DamageType DamageType { get; set; } = DamageType.None;
		public string BackColor { get; set; } = "#ebebeb";
		public string FontColor { get; set; } = "#000000";
		//public List<string> Effects { get; set; }
		// Trailing effects
		public DiceDto()
		{

		}
	}
}
