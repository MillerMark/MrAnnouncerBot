//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class WealthChange
	{
		public List<int> PlayerIds = new List<int>();
		public Coins Coins { get; set; }
		public WealthChange()
		{
			Coins = new Coins();
		}
	}
}
