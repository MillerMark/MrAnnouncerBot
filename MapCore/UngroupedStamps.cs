using System;
using System.Linq;
using System.Collections.Generic;
using MapCore;

namespace MapCore
{
	public class UngroupedStamps
	{
		public int StartingZOrder { get; set; }
		public List<IStampProperties> Stamps { get; set; } = new List<IStampProperties>();
		public IStampProperties StampGroup { get; set; }

		public UngroupedStamps()
		{

		}

		public UngroupedStamps(IStampProperties stampGroup)
		{
			StampGroup = stampGroup;
		}
	}
}

