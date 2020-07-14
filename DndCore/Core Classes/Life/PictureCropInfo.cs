using System;
using System.Linq;

namespace DndCore
{
	public class PictureCropInfo
	{
		public const double MinWidth = 104;
		public const double MinHeight = 90;
		public const double AspectRatio = MinWidth / MinHeight;  // Width/Height
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		// Aspect ratio is always 104:90, so no need for height as it can always be calculated from Width.
		public PictureCropInfo()
		{

		}

		public static double GetHeightFromWidth(double newWidth)
		{
			return newWidth / AspectRatio;
		}
	}
}