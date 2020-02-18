using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class ReconstituteStampsEventArgs : EventArgs
	{
		public List<IStampProperties> Stamps { get; set; }
		public SerializedStamp SerializedStamp { get; set; }
		public ReconstituteStampsEventArgs()
		{

		}
	}
}
