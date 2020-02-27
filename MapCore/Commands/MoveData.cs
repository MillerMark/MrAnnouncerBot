using System;
using System.Linq;

namespace MapCore
{
	public class MoveData
	{
		public double DeltaX { get; set; }
		public double DeltaY { get; set; }

		public MoveData(double deltaX, double deltaY)
		{
			DeltaX = deltaX;
			DeltaY = deltaY;
		}

		public MoveData()
		{

		}
	}
}
