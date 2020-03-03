using System;
using System.Linq;
using MapCore;

namespace MapCore
{
	public static class ItemFactory
	{
		public static IItemProperties Clone(this IItemProperties item, double x = 0, double y = 0)
		{
			if (item is IStampProperties stamp)
				return stamp.Copy(x, y);
			else
				return item.Copy(x, y);
		}
	}
}

