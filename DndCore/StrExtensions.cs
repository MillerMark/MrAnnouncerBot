using System;
using System.Linq;

namespace DndCore
{
	public static class StrExtensions
	{
		public static double ToDouble(this string str)
		{
			if (double.TryParse(str.Trim(), out double result))
				return result;
			return 0;
		}

		public static int ToInt(this string str)
		{
			if (int.TryParse(str.Trim(), out int result))
				return result;
			return 0;
		}
	}
}

