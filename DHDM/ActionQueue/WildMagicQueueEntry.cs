//#define profiling
using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public class WildMagicQueueEntry : DieRollQueueEntry
	{
		public WildMagicQueueEntry()
		{
			RollType = DiceRollType.WildMagic;
			RollScope = RollScope.ActivePlayer;
		}

		public override void PrepareRoll(DiceRoll diceRoll)
		{
			base.PrepareRoll(diceRoll);
		}
	}
}
