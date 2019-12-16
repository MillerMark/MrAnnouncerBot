using System;
using System.Linq;
using System.Windows;

namespace MapCore
{
	public class FloorSpace: Tile
	{
		public object Data { get; set; }
		public string Code { get; set; }
		public MapRegion Parent { get; set; }
		public FloorSpace(int column, int row, string code, object data = null): base(column, row)
		{
			Code = code;
			Data = data;
		}
	}
}
