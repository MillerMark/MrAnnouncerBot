using System;
using System.Linq;

namespace DndCore
{
	public class KnownSpell
	{
		public string SpellName { get; set; }
		public string ItemName { get; set; }
		public int ChargesRemaining
		{
			get
			{
				if (Player == null)
					return 0;
				return Player.GetRemainingChargesOnItem(ItemName);
			}
			set
			{
				Player?.SetRemainingChargesOnItem(ItemName, value);
			}
		}
		public int TotalCharges { get; set; } = int.MaxValue;
		public DndTimeSpan ResetSpan { get; set; }
		public TimeSpan RechargesAt { get; set; }
		public Character Player { get; set; }

		public KnownSpell()
		{
		}

		public bool CanCast()
		{
			return TotalCharges == int.MaxValue || ChargesRemaining > 0;
		}

		public void Cast()
		{
			if (TotalCharges < int.MaxValue)
				if (ChargesRemaining > 0)
					ChargesRemaining--;
		}

		public bool CanBeRecharged()
		{
			return TotalCharges < int.MaxValue;
		}

		public bool HasAnyCharges()
		{
			return ChargesRemaining > 0;
		}
		public void TestEvaluateAllExpressions(Character character)
		{
			// TODO: Do this.
		}
	}
}