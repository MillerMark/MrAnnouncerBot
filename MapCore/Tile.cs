using System;
using System.Linq;

namespace MapCore
{
	public class Tile
	{
		public IMapInterface Map { get; set; }
		// TODO: Hmm.. 120 is a UI=specific implementation. Maybe change to a static property? 
		public const int Width = 120;
		public const int Height = 120;
		public object UIElementFloor { get; set; }
		public object UIElementOverlay { get; set; }
		public object SelectorPanel { get; set; }
		public int Row { get; set; }
		public int Column { get; set; }
		public SpaceType SpaceType { get; set; }
		public bool Selected { get; set; }
		public int PixelX
		{
			get
			{
				return Column * Width;
			}
		}
		public int PixelY
		{
			get
			{
				return Row * Height;
			}
		}

		public Tile(int column, int row, IMapInterface iMap)
		{
			Map = iMap;
			Row = row;
			Column = column;
		}
		public void GetPixelCoordinates(out int left, out int top, out int right, out int bottom)
		{
			left = PixelX;
			top = PixelY;
			right = left + Width - 1;
			bottom = top + Height - 1;
		}
	}
}
