using System;
using System.Linq;
using System.Windows;

namespace MapCore
{
	public class FloorSpace: Tile
	{
		public string Code { get; set; }
		public MapRegion Parent { get; set; }
		public FloorSpace(int column, int row, string code, IMapInterface iMap) : base(column, row, iMap)
		{
			Code = code;
		}
	}
}
