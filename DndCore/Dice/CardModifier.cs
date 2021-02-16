using System;
using System.Linq;

namespace DndCore
{
	public class CardModifier
	{
		public int CreatureId { get; set; }
		public string BlameName { get; set; }
		public double Offset { get; set; }
		public double Multiplier { get; set; } = 1;
		public CardModifier()
		{

		}

		public CardModifier(int creatureId, string blameName, int offset, double multiplier = 1)
		{
			CreatureId = creatureId;
			BlameName = blameName;
			Offset = offset;
			Multiplier = multiplier;
		}
	}
}
