﻿using System;
using System.Linq;
using DndCore;
using Streamloots;

namespace DHDM
{
	public class ViewerDieRollStoppedEventArgs : EventArgs
	{
		public ViewerDieRollStoppedEventArgs(CardDto card, RollResults stopRollingData)
		{
			StopRollingData = stopRollingData;
			Card = card;
		}
		public CardDto Card { get; set; }
		public RollResults StopRollingData { get; set; }
	}
}
