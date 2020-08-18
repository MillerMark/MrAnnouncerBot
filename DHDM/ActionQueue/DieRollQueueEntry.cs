//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class DieRollQueueEntry : ActionQueueEntry
	{
		public List<DiceDto> DiceDtos { get; set; } = new List<DiceDto>();
		public int HiddenThreshold { get; set; } = int.MinValue;
		public DiceRollType RollType { get; set; } = DiceRollType.None;
		public RollScope RollScope { get; set; } = RollScope.ActivePlayer;
		public bool IsMagic { get; set; }


		public DieRollQueueEntry()
		{

		}

		protected void GetPlayerDieColor(Character player, out string backgroundColor, out string textColor)
		{
			backgroundColor = "#d0d0d0";
			textColor = "#ffffff";
			if (player != null)
			{
				backgroundColor = player.dieBackColor;
				textColor = player.dieFontColor;
			}
		}

		public virtual void PrepareRoll(DiceRoll diceRoll)
		{
			diceRoll.RollScope = RollScope;
			diceRoll.DiceDtos = DiceDtos;
			diceRoll.HiddenThreshold = HiddenThreshold;
			diceRoll.IsMagic = IsMagic;
		}
	}
}
