using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class MapRegion
	{
		public List<Tile> Spaces { get; set; } = new List<Tile>();
		public double Width { get; private set; }
		public double Height { get; private set; }
	}
}
