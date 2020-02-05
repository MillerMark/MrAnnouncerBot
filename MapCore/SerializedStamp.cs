using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class SerializedStamp : BaseStampProperties
	{
		public List<SerializedStamp> Children { get; set; }
		public override int Height { get; set; }
		public override int Width { get; set; }

		public SerializedStamp()
		{

		}
	}
}
