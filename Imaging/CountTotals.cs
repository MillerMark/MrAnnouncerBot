using System;
using System.Drawing;

namespace Imaging
{
	public class CountTotals
	{
		public int Count { get; set; }
		public int XTotal { get; set; }
		public int YTotal { get; set; }
		
		public void Add(int x, int y)
		{
			XTotal += x;
			YTotal += y;
			Count++;
		}
	}
}
