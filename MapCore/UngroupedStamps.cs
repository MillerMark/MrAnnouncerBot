using System;
using System.Linq;
using System.Collections.Generic;
using MapCore;

namespace MapCore
{
	public class UngroupedStamps
	{
		public int StartingZOrder { get; set; }
		public List<IItemProperties> Stamps { get; set; } = new List<IItemProperties>();
		public IItemProperties StampGroup { get; set; }

		public UngroupedStamps()
		{

		}

		public UngroupedStamps(IItemProperties stampGroup)
		{
			StampGroup = stampGroup;
		}
	}
}

