using System;
using System.Linq;

namespace MapCore
{
	public class BaseSpace
	{
		public int Row { get; set; }
		public int Column { get; set; }
		public SpaceType SpaceType { get; set; }
		public object SelectorPanel { get; set; }
		public bool Selected { get; set; }

		public BaseSpace(int column, int row)
		{
			Row = row;
			Column = column;
		}
		public void GetPixelCoordinates(out int left, out int top, out int right, out int bottom)
		{
			left = Column * Map.pixelsPerTile;
			top = Row * Map.pixelsPerTile;
			right = left + Map.pixelsPerTile - 1;
			bottom = top + Map.pixelsPerTile - 1;
		}
	}
}
