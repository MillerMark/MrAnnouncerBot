using System;
using System.Linq;

namespace TaleSpireExplore
{
	public static class ColorUtils
	{
		public static UnityEngine.Color ToUnityColor(System.Drawing.Color color)
		{
			return new UnityEngine.Color(color.R / 255f, color.G / 255f, color.B / 255f);
		}

		public static System.Drawing.Color ToSysDrawColor(UnityEngine.Color color)
		{
			return System.Drawing.Color.FromArgb((int)Math.Round(Math.Max(color.r, 1) * 255), (int)Math.Round(Math.Max(color.g, 1) * 255), (int)Math.Round(Math.Max(color.b, 1) * 255));
		}
	}
}
