using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class CreateGroupEventArgs : EventArgs
	{
		public List<IStampProperties> Stamps { get; set; }
		public IStampGroup Group { get; set; }

		public CreateGroupEventArgs(List<IStampProperties> stamps)
		{
			Stamps = stamps;
		}
		public CreateGroupEventArgs()
		{

		}
	}
}
