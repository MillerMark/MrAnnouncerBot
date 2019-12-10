using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class Room
	{
		public List<RoomSegment> Segments { get; set; } = new List<RoomSegment>();
		public int RightColumn
		{
			get
			{
				return LeftColumn + Segments.Count - 1;
			}
		}
		public int LeftColumn { get; set; }
		public int StartRow
		{
			get
			{
				return GetTop();
			}
		}
		public int EndRow
		{
			get
			{
				return GetBottom();
			}
		}
		
		public int Height
		{
			get
			{
				return EndRow - StartRow + 1;
			}
		}

		public int Width
		{
			get
			{
				return RightColumn - LeftColumn + 1;
			}
		}

		public Room(int startColumn)
		{
			LeftColumn = startColumn;
		}

		int GetTop()
		{
			int maxTop = 0;
			foreach (RoomSegment roomSegment in Segments)
			{
				if (roomSegment.StartRow > maxTop)
					maxTop = roomSegment.StartRow;
			}
			return maxTop;
		}

		int GetBottom()
		{
			int minBottom = int.MaxValue;
			foreach (RoomSegment roomSegment in Segments)
			{
				if (roomSegment.EndRow < minBottom)
					minBottom = roomSegment.EndRow;
			}
			return minBottom;
		}

		bool IsAdjacentTo(RoomSegment segment)
		{
			int top = GetTop();
			int bottom = GetBottom();
			return top < segment.EndRow && segment.StartRow < bottom;
		}

		public SegmentAnalysis AnalyzeSegment(RoomSegment roomSegment)
		{
			SegmentAnalysis segmentAnalysis = new SegmentAnalysis();
			RoomSegment lastSegment = Segments.LastOrDefault();
			if (lastSegment == null)
				return segmentAnalysis;

			if (lastSegment.IsAdjacent(roomSegment))
			{
				segmentAnalysis.OverlapsLastSegment = true;
				if (IsAdjacentTo(roomSegment))
					segmentAnalysis.Position = SegmentPosition.OverlapsRoom;
				else
					segmentAnalysis.Position = SegmentPosition.OutsideRoom;

				return segmentAnalysis;
			}

			return segmentAnalysis;
		}

		public void AddSegment(RoomSegment segment)
		{
			Segments.Add(segment);
		}

		public void AddSegment(int startRow, int endRow, int column)
		{
			AddSegment(new RoomSegment(startRow, endRow, column));
		}
	}
}
