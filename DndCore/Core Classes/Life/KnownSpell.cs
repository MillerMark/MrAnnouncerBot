using System;
using System.Linq;

namespace DndCore
{
	public class KnownSpell
	{
		public string SpellName { get; set; }
		public string ItemName { get; set; }
		public int ChargesRemaining { get; set; } = int.MaxValue;
		public int TotalCharges { get; set; } = int.MaxValue;
		public DndTimeSpan ResetSpan { get; set; }

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
	}
}