using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public static class Debouncer
	{
		static Dictionary<string, DateTime> secondsSinceLastToggle;
		public static bool IsGood(string keyword, int digit)
		{
			if (secondsSinceLastToggle == null)
				secondsSinceLastToggle = new Dictionary<string, DateTime>();
			string key = $"{keyword}.{digit}";
			return IsGood(key);
		}

		public static bool IsGood(string key)
		{
			if (secondsSinceLastToggle == null)
				secondsSinceLastToggle = new Dictionary<string, DateTime>();
			if (!secondsSinceLastToggle.ContainsKey(key))
			{
				secondsSinceLastToggle.Add(key, DateTime.Now);
				return true;
			}

			double spanSinceLastKeyPressMs = (DateTime.Now - secondsSinceLastToggle[key]).TotalMilliseconds;
			if (spanSinceLastKeyPressMs < 110)
			{
				return false;
				// De-bounce it. Could be a bug in the stream deck sending two commands at once.
			}
			secondsSinceLastToggle[key] = DateTime.Now;
			return true;
		}
	}
}
