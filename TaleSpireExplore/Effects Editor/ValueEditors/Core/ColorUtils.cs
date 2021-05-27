using System;
using System.Linq;
using TaleSpireCore;

namespace TaleSpireExplore
{
	public static class ColorUtils
	{
		public static UnityEngine.Color ToUnityColor(System.Drawing.Color color, float multiplier = 1f)
		{
			return new UnityEngine.Color(color.R / 255f * multiplier, color.G / 255f * multiplier, color.B / 255f * multiplier);
		}

		public static System.Drawing.Color ToSysDrawColor(UnityEngine.Color color)
		{
			Talespire.Log.Debug($"ToSysDrawColor/color = {color.r}, {color.g}, {color.b}");
			return System.Drawing.Color.FromArgb((int)Math.Round(Math.Min(color.r, 1) * 255), (int)Math.Round(Math.Min(color.g, 1) * 255), (int)Math.Round(Math.Min(color.b, 1) * 255));
		}
	}
}
