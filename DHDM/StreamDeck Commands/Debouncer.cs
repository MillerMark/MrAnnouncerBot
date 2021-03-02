using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public static class Debouncer
	{
		static Dictionary<int, DateTime> secondsSinceLastToggle;
		public static bool CanToggle(int targetNum)
		{
			if (secondsSinceLastToggle == null)
				secondsSinceLastToggle = new Dictionary<int, DateTime>();
			if (!secondsSinceLastToggle.ContainsKey(targetNum))
			{
				secondsSinceLastToggle.Add(targetNum, DateTime.Now);
				return true;
			}
			if (DateTime.Now - secondsSinceLastToggle[targetNum] < TimeSpan.FromMilliseconds(200))
			{
				return false;
				// De-bounce it. Could be a bug in the stream deck sending two commands at once.
			}
			secondsSinceLastToggle[targetNum] = DateTime.Now;
			return true;
		}
	}
}
