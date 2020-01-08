using System;
using System.Linq;

namespace DndMapSpike
{
	public class ResizeTracker
	{
		public Stamp Stamp { get; set; }
		public SizeDirection Direction { get; set; }
		public ResizeTracker(Stamp stamp, SizeDirection sizeDirection)
		{
			Direction = sizeDirection;
			Stamp = stamp;
		}
	}
}

