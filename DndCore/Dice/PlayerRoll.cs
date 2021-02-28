using System;
using System.Linq;

namespace DndCore
{
	public class PlayerRoll
	{
		public int rawTotalScore;
		public string name;
		public string data;
		public int id;
		public int skillSaveInitiativeScoreModifier;
		public int cardModifierDamageOffset;
		public int cardModifierDamageMultiplier = 1;
		public int cardModifierScoreOffset;
		public int cardModifierScoreMultiplier = 1;
		public string cardModifiersDamageStr { get; set; }
		public string cardModifiersScoreStr { get; set; }
		public bool success;
		public bool isCrit;
		public bool isCompleteFail;

		public int GetTotalRollValue()
		{
			return (skillSaveInitiativeScoreModifier + cardModifierScoreOffset + rawTotalScore) * cardModifierScoreMultiplier;
		}
	}
}
