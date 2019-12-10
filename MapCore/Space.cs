using System;
using System.Linq;
using System.Windows;

namespace MapCore
{
	public class Space
	{
		public MapSpaceType Type { get; set; }
		public int Row { get; set; }
		public int Column { get; set; }
		public object Data { get; set; }
		public Space(int column, int row, MapSpaceType type, object data = null)
		{
			Data = data;
			Type = type;
			Column = column;
			Row = row;
		}
	}
}
