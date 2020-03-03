using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class CreateGroupEventArgs : EventArgs
	{
		public List<IItemProperties> Stamps { get; set; }
		public IItemGroup Group { get; set; }

		public CreateGroupEventArgs(List<IItemProperties> stamps)
		{
			Stamps = stamps;
		}
		public CreateGroupEventArgs()
		{

		}
	}
}
