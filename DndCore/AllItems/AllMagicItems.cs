using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DndCore
{
	public static class AllMagicItems
	{
		static List<MagicItem> magicItems;
		public static List<MagicItem> MagicItems
		{
			get
			{
				if (magicItems == null)
					LoadItems();
				return magicItems;
			}
			set => magicItems = value;
		}

		public static void LoadItems()
		{
			MagicItems = GoogleSheets.Get<MagicItem>();
		}

		public static MagicItem Get(string magicItemName)
		{
			return MagicItems.FirstOrDefault(x => x.Name == magicItemName);
		}
		public static void Invalidate()
		{
			magicItems = null;
		}
	}
}
