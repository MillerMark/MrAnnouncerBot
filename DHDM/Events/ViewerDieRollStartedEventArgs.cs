using System;
using System.Linq;
using DndCore;
using Streamloots;

namespace DHDM
{
	public class ViewerDieRollStartedEventArgs : EventArgs
	{
		public ViewerDieRollStartedEventArgs(CardDto card, DiceRoll startRollingData)
		{
			StartRollingData = startRollingData;
			Card = card;
		}
		public CardDto Card { get; set; }
		public DiceRoll StartRollingData { get; set; }
	}
}
