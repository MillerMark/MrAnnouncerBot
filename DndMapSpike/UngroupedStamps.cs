using System;
using System.Linq;
using System.Collections.Generic;
using MapCore;

namespace DndMapSpike
{
	public class UngroupedStamps
	{
		public int StartingZOrder { get; set; }
		public List<IStampProperties> Stamps { get; set; } = new List<IStampProperties>();
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

