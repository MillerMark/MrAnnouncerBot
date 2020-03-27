using System;
using System.Collections.Generic;
using System.Linq;

namespace DndCore
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
