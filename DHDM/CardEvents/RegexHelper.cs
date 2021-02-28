//#define profiling
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DHDM
{
	public static class RegexHelper
	{
		public static T GetValue<T>(Match match, string groupName)
		{
			GroupCollection groups = match.Groups;
			Group group = groups[groupName];
			if (group == null)
				return default(T); ;

			string value = group.Value;

			if (string.IsNullOrEmpty(value))
				return default(T);

			if (typeof(T).Name == typeof(double).Name)
				if (double.TryParse(value, out double result))
					return (T)(object)result;

			return (T)(object)value;
		}
	}
}
