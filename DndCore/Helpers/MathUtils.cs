using System;
using System.Linq;

namespace DndCore
{
	public static class MathUtils
	{
		public static bool IsChecked(string entry)
		{
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
	}
}
