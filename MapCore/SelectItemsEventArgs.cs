using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class SelectItemsEventArgs : EventArgs
	{
		public List<Guid> ItemsGuids { get; set; }
		public SelectItemsEventArgs(List<Guid> itemsGuids)
		{
			ItemsGuids = itemsGuids;
		}
		public SelectItemsEventArgs()
		{

		}
	}
}
