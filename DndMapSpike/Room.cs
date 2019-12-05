using System;
using System.Linq;
using System.Collections.Generic;

namespace DndMapSpike
{
	public class Room
	{
		public List<RoomSegment> Segments { get; set; } = new List<RoomSegment>();
		public int RightColumn
		{
			get
			{
				return StartColumn + Segments.Count - 1;
			}
		}
		public int StartColumn { get; set; }

		public Room(int startColumn)
		{
			StartColumn = startColumn;
		}

		public bool SegmentExtends(RoomSegment roomSegment)
		{
			RoomSegment lastSegment = Segments.LastOrDefault();
			if (lastSegment == null)
				return false;

			if (lastSegment.Matches(roomSegment))
			{
				Segments.Add(roomSegment);
				return true;
			}
			return false;
		}
	}
}
