using System;
using System.Linq;

namespace MapCore
{
	public class SerializedStampEventArgs : EventArgs
	{
		public SerializedItem Item { get; set; }
		public IItemProperties Properties { get; set; }
		public SerializedStampEventArgs()
		{

		}
	}
}
