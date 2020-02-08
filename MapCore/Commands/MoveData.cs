using System;
using System.Linq;

namespace MapCore
{
	public class MoveData
	{
		public int DeltaX { get; set; }
		public int DeltaY { get; set; }

		public MoveData(int deltaX, int deltaY)
		{
			DeltaX = deltaX;
			DeltaY = deltaY;
		}

		public MoveData()
		{

		}
	}
}
