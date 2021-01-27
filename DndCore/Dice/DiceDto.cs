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
		public double Scale { get; set; } = 1;
		public string Label { get; set; }
		public string Data { get; set; }  // Any string sent down through this property will return when the roll is complete in multiplayerSummary's PlayerRoll.data property.
		public string PlayerName { get; set; }
		public VantageKind Vantage { get; set; } = VantageKind.Normal;
		public DamageType DamageType { get; set; } = DamageType.None;
		public DieCountsAs DieCountsAs { get; set; } = DieCountsAs.totalScore;
		public string BackColor { get; set; } = "#ebebeb";
		public string FontColor { get; set; } = "#000000";
		//public List<string> Effects { get; set; }
		// Trailing effects
		public DiceDto()
		{
			Quantity = 1;
		}
		public void SetRollDetails(DiceRollType type, string descriptor)
		{
			descriptor = descriptor.ToLower();
			DamageType = DndUtils.ToDamage(descriptor);
			if (DamageType != DamageType.None)
				DieCountsAs = DieCountsAs.damage;
			else if (descriptor.Contains("health") || type == DiceRollType.HealthOnly)
				DieCountsAs = DieCountsAs.health;
			else if (descriptor.Contains("inspiration") || type == DiceRollType.InspirationOnly)
				DieCountsAs = DieCountsAs.inspiration;
			else if (descriptor.Contains("extra") || type == DiceRollType.ExtraOnly)
				DieCountsAs = DieCountsAs.extra;
			else if (descriptor.Contains("bent luck") || type == DiceRollType.BendLuckAdd || type == DiceRollType.BendLuckSubtract)
				DieCountsAs = DieCountsAs.bentLuck;
			else if (descriptor.Contains("bonus"))
				DieCountsAs = DieCountsAs.bonus;
			else 
				DieCountsAs = DieCountsAs.totalScore;
		}
	}
}
