using System;
using System.Linq;

namespace DndMapSpike
{
	public class BaseStamp
	{
		public bool Visible { get; set; } = true;
		public int X { get; set; }

		public int Y { get; set; }

		public int ZOrder { get; set; } = -1;

		public void SwapXY()
		{
			int oldX = X;
			X = Y;
			Y = oldX;
		}
		public bool HasNoZOrder()
		{
			return ZOrder == -1;
		}

		public void ResetZOrder()
		{
			ZOrder = -1;
		}



		public BaseStamp()
		{

		}
	}
}

