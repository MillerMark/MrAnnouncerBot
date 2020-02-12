using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class SelectStampsEventArgs : EventArgs
	{
		public List<Guid> StampGuids { get; set; }
		public SelectStampsEventArgs(List<Guid> stampGuids)
		{
			StampGuids = stampGuids;
		}
		public SelectStampsEventArgs()
		{

		}
	}
}
