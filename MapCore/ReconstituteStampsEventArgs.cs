using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class ReconstituteItemsEventArgs : EventArgs
	{
		public List<IItemProperties> Items { get; set; }
		public SerializedItem SerializedItem { get; set; }
		public ReconstituteItemsEventArgs()
		{

		}
	}
}
