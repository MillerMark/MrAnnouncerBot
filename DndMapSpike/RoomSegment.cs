using System;
using System.Linq;

namespace DndMapSpike
{
	public class RoomSegment
	{
		public int StartRow { get; set; }
		public int EndRow { get; set; }
		public int Column { get; set; }

		public RoomSegment(int startRow, int endRow, int column)
		{
			Column = column;
			StartRow = startRow;
			EndRow = endRow;
		}
		public RoomSegment()
		{

		}
		public bool Matches(RoomSegment compareRoomSegment)
		{
			return StartRow == compareRoomSegment.StartRow && EndRow == compareRoomSegment.EndRow;
		}
	}
}
