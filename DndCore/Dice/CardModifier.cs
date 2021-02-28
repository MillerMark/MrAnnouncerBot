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
		public CardModType CardModType { get; set; } = CardModType.TotalScorePlusDamage;
		public CardModifier()
		{

		}

		public CardModifier(int creatureId, string blameName, int offset, double multiplier = 1, CardModType cardModType = CardModType.TotalScorePlusDamage)
		{
			CreatureId = creatureId;
			BlameName = blameName;
			Offset = offset;
			Multiplier = multiplier;
			CardModType = cardModType;
		}
	}
}
