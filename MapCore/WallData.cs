using System;
using MapCore;

namespace MapCore
{
	public class WallData
	{
		public WallOrientation Orientation { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
		public double WallLength { get; set; }

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
