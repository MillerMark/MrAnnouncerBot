using System;
using MapCore;

namespace MapCore
{
	public class WallData
	{
		public WallOrientation Orientation { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int WallLength { get; set; }

		public int StartColumn { get; set; }
		public int StartRow { get; set; }
		public int EndColumn { get; set; }
		public int EndRow { get; set; }
		public bool IsVertical
		{
			get
			{
				return StartColumn == EndColumn;
			}
		}

		public bool IsHorizontal
		{
			get
			{
				return StartRow == EndRow;
			}
		}

		public WallData(WallOrientation orientation)
		{
			Orientation = orientation;
		}
	}
}
