using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

namespace DndCore
{
	public static class AllDndItems
	{
		static List<DndItem> allItems;

		static void Load()
		{
			allItems = GoogleSheets.Get<DndItem>();
		}

		public static List<DndItem> AllItems
		{
			get
			{
				if (allItems == null)
				{
					Load();
				}
				return allItems;
			}
		}

		public static void Invalidate()
		{
			allItems = null;
		}

		public static DndItem Get(string itemName)
		{
			return AllItems.FirstOrDefault(x => x.Name == itemName);
		}
	}
}