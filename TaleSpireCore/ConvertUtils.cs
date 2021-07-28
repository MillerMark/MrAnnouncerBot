using System;
using System.Linq;

namespace TaleSpireCore
{
	public static class ConvertUtils
	{
		static T GetElement<T>(string elementName) where T : struct
		{
			if (Enum.TryParse(elementName.Trim().Replace(" ", ""), true, out T result))
				return result;
			else
				return default;
		}

		public static WhatSide ToWhatSide(string whatSideStr)
		{
			return GetElement<WhatSide>(whatSideStr);
		}
	}
}
