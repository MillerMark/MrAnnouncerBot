using System;
using System.Linq;

namespace CommonCore
{
	public static class StrExtensions
	{
		public static double ToDouble(this string str)
		{
			if (double.TryParse(str.Trim(), out double result))
				return result;
			return 0;
		}

		public static decimal ToDecimal(this string str)
		{
			if (decimal.TryParse(str.Trim(), out decimal result))
				return result;
			return 0;
		}

		public static int ToInt(this string str, int defaultValue = 0)
		{
			if (int.TryParse(str.Trim(), out int result))
				return result;
			return defaultValue;
		}

		public static double GetFirstDouble(this string str, bool allowDecimals = true, double defaultValue = 0)
		{
			bool foundAtLeastOneDigit = false;
			bool lastCharWasMinus = false;
			bool isNegative = false;
			bool lastCharWasDot = false;
			bool hasDecimalPoint = false;
			string numberStr = string.Empty;
			for (int i = 0; i < str.Length; i++)
			{
				if (char.IsDigit(str[i]))
				{
					if (lastCharWasDot && !hasDecimalPoint)
					{
						numberStr += ".";
						hasDecimalPoint = true;
						lastCharWasDot = false;
					}
					foundAtLeastOneDigit = true;
					if (lastCharWasMinus)
					{
						isNegative = true;
						lastCharWasMinus = false;
					}
					numberStr += str[i];
				}
				else if (foundAtLeastOneDigit && !hasDecimalPoint && str[i] == '.' && allowDecimals)
				{
					if (lastCharWasDot)
						break;
					lastCharWasDot = true;
				}
				else
				{
					if (foundAtLeastOneDigit)
						break;
					if (str[i] == '-')
						lastCharWasMinus = true;
				}
			}
			int multiplier = isNegative ? -1 : 1;
			if (foundAtLeastOneDigit)
				return multiplier * double.Parse(numberStr);
			return defaultValue;
		}

		/// <summary>
		/// Returns true if the specified string is a case-insensitive match.
		/// </summary>
		public static bool SameLetters(this string str, string compareStr)
		{
			return string.Compare(str, compareStr, true) == 0;
		}

		public static int GetFirstInt(this string str, int defaultValue = 0)
		{
			return (int)Math.Floor(GetFirstDouble(str, false, defaultValue));
		}

		//public static string InitialCap(this string str)
		//{
		//	if (string.IsNullOrWhiteSpace(str))
		//		return str;
		//	return char.ToUpper(str[0]) + str.Substring(1);
		//}

		public static string EverythingAfter(this string str, string matchStr)
		{
			int pos = str.IndexOf(matchStr);
			if (pos >= 0)
			{
				return str.Substring(pos + matchStr.Length);
			}
			return string.Empty;
		}

		public static string EverythingAfterLast(this string str, string matchStr)
		{
			int pos = str.LastIndexOf(matchStr);
			if (pos >= 0)
			{
				return str.Substring(pos + matchStr.Length);
			}
			return string.Empty;
		}

		public static string InitialCap(this string str)
		{
			if (string.IsNullOrEmpty(str))
				return string.Empty;
			char firstChar = str[0];
			return Char.ToUpper(firstChar) + str.Substring(1);
		}

		public static string EverythingBeforeLast(this string str, string matchStr)
		{
			if (str == null)
				return string.Empty;
			int pos = str.LastIndexOf(matchStr);
			if (pos >= 0)
			{
				return str.Substring(0, pos);
			}
			return string.Empty;
		}

		public static string EverythingBefore(this string str, string matchStr)
		{
			if (str == null)
				return string.Empty;
			int pos = str.IndexOf(matchStr);
			if (pos >= 0)
			{
				return str.Substring(0, pos);
			}
			return string.Empty;
		}


		public static string EverythingBetween(this string str, string beginMatchStr, string endMatchStr)
		{
			return str.EverythingAfter(beginMatchStr).EverythingBeforeLast(endMatchStr);
		}


		public static string EverythingBetweenNarrow(this string str, string beginMatchStr, string endMatchStr)
		{
			return str.EverythingAfterLast(beginMatchStr).EverythingBefore(endMatchStr);
		}


		public static bool Has(this string str, string matchStr)
		{
			return str.IndexOf(matchStr) >= 0;
		}

		public static bool HasSomething(this string str)
		{
			return !string.IsNullOrWhiteSpace(str);
		}
	}
}

