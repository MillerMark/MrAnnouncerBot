using System;
using System.Linq;

namespace MapCore
{
	public class SegmentAnalysis
	{
		public SegmentPosition Position { get; set; }
		public bool OverlapsLastSegment { get; set; }

		public SegmentAnalysis()
		{
			Position = SegmentPosition.OutsideRoom;
			OverlapsLastSegment = false;
		}
	}
}
