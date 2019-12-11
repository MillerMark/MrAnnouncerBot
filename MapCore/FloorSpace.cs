using System;
using System.Linq;
using System.Windows;

namespace MapCore
{
	public class FloorSpace
	{
		public int Row { get; set; }
		public int Column { get; set; }
		public object Data { get; set; }
		public SpaceType SpaceType { get; set; }
		public MapRegion Parent { get; set; }
		public string Code { get; set; }
		public FloorSpace(int column, int row, string code, object data = null)
		{
			Code = code;
			Data = data;
			Column = column;
			Row = row;
		}
	}
}
