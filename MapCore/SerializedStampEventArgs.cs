using System;
using System.Linq;

namespace MapCore
{
	public class SerializedStampEventArgs : EventArgs
	{
		public SerializedStamp Stamp { get; set; }
		public IItemProperties Properties { get; set; }
		public SerializedStampEventArgs()
		{

		}
	}
}
