using System;
using System.Linq;
using System.Windows.Media;

namespace DHDM
{
	public static class ColorExtensionMethods
	{
		public static float GetHue(this Color c) => System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B).GetHue();

		public static float GetBrightness(this Color c) => System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B).GetBrightness();

		public static float GetSaturation(this Color c) => System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B).GetSaturation();
	}
}
