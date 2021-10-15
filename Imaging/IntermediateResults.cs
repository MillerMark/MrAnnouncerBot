using System;

namespace Imaging
{
	public class IntermediateResults
	{
		public CountTotals Yellow { get; set; } = new CountTotals();
		public CountTotals Green { get; set; } = new CountTotals();
		public CountTotals Red { get; set; } = new CountTotals();
		public CountTotals Blue { get; set; } = new CountTotals();
		public byte GreatestOpacity { get; set; }
		//public void FindCircleCenters(DirectBitmap bitmap)
		//{
		//	//Yellow.FindCircleCenters(bitmap);
		//	//Green.FindCircleCenters(bitmap);
		//	//Red.FindCircleCenters(bitmap);
		//	//Blue.FindCircleCenters(bitmap);
		//}
	}
}
