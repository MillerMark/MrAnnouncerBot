using System;
using System.Linq;

namespace DndUI
{
	public static class StrUtils
	{
		public static string GetFirstName(string name)
		{
			if (name == null)
				return "No name";
			int spaceIndex = name.IndexOf(' ');
			if (spaceIndex < 0)
				return name;
			return name.Substring(0, spaceIndex);
		}
	}
}
