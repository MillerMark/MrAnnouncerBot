using System;
using System.Linq;
using System.Collections.Generic;

namespace DndMapSpike
{
	public class UngroupedStamps
	{
		public int StartingZOrder { get; set; }
		public List<IStamp> Stamps { get; set; } = new List<IStamp>();
		public StampGroup StampGroup { get; set; }

		public UngroupedStamps()
		{

		}

		public UngroupedStamps(StampGroup stampGroup)
		{
			StampGroup = stampGroup;
		}
	}
}

