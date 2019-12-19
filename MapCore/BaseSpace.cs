using System;
using System.Linq;

namespace MapCore
{
	public class Tile
	{
		public object UIElementFloor { get; set; }
		public object UIElementOverlay { get; set; }
		public object SelectorPanel { get; set; }
		public int Row { get; set; }
		public int Column { get; set; }
		public SpaceType SpaceType { get; set; }
		public bool Selected { get; set; }

		public Tile(int column, int row)
		{
			Row = row;
			Column = column;
		}
		public void GetPixelCoordinates(out int left, out int top, out int right, out int bottom)
		{
			left = Column * Map.TileSizePx;
			top = Row * Map.TileSizePx;
			right = left + Map.TileSizePx - 1;
			bottom = top + Map.TileSizePx - 1;
		}
	}
}
