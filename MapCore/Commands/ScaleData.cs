using System;
using System.Linq;

namespace MapCore
{
	public class ScaleData
	{
		public double ScaleFactor { get; set; }
		public ScaleData(double scaleFactor)
		{
			ScaleFactor = scaleFactor;
		}
		public ScaleData()
		{

		}
	}
}
