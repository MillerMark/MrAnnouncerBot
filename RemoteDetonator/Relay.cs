using System;
using System.Linq;

namespace RemoteDetonator
{
	public class Relay
	{
		public Relay()
		{

		}
		public string Name { get; set; }
		public string Text { get; set; }
		public int Index { get; set; }
		public bool Latched { get; set; }
	}
}
