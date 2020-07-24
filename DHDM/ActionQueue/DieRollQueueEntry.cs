//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class DieRollQueueEntry : ActionQueueEntry
	{
		public string DieStr { get; set; }
		public List<DiceDto> DiceDtos { get; set; } = new List<DiceDto>();
		public int HiddenThreshold { get; set; } = int.MinValue;
		public DiceRollType RollType { get; set; } = DiceRollType.None;

		public DieRollQueueEntry()
		{

		}
	}
}
