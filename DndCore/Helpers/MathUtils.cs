using System;
using System.Linq;

namespace DndCore
{
	public static class MathUtils
	{
		public static bool IsChecked(string entry)
		{
			if (entry == null)
				return false;
			return entry.ToLower().Trim() == "x";
		}

		public static double GetDouble(string str, double defaultValue = 0)
		{
			if (!string.IsNullOrEmpty(str) && double.TryParse(str, out double result))
				return result;
			return defaultValue;
		}
		public static int GetInt(string str, int defaultValue = 0)
		{
			if (!string.IsNullOrEmpty(str) && int.TryParse(str, out int result))
				return result;
			return defaultValue;
		}

		public static string BoolToStr(bool value)
		{
			if (value)
				return "x";
			return string.Empty;
		}

		static Random random = new Random();
		public static int RandomBetween(int min, int max)
		{
			int delta = max - min + 1;
			if (delta == 0)
				return min;
			return random.Next(delta) + min;
		}

		public static double Clamp(double value, double min, double max)
		{
			return Math.Min(Math.Max(value, min), max);
		}
	}
}
