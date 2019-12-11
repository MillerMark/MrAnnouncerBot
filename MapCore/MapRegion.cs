using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class MapRegion
	{
		public List<FloorSpace> Spaces { get; set; } = new List<FloorSpace>();
		public double Width { get; private set; }
		public double Height { get; private set; }
	}
}
