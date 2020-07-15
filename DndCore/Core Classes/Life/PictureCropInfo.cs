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

		public static PictureCropInfo FromStr(string imageCrop)
		{
			PictureCropInfo result = new PictureCropInfo();
			result.Width = MinWidth;
			if (string.IsNullOrWhiteSpace(imageCrop))
				return result;
			string[] arguments = imageCrop.Split(',');
			if (arguments.Length > 0)
			{
				result.X = MathUtils.GetDouble(arguments[0].Trim());
				if (arguments.Length > 1)
				{
					result.Y = MathUtils.GetDouble(arguments[1].Trim());
					if (arguments.Length > 2)
						result.Width = MathUtils.GetDouble(arguments[2].Trim());
				}
			}
			return result;
		}

		public string ToCSV()
		{
			return $"{X}, {Y}, {Width}";
		}
	}
}