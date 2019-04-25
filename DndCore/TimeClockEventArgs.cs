using System;
using System.Linq;


namespace DndCore
{
	public class TimeClockEventArgs : EventArgs
	{
		public TimeSpan SpanSinceLastUpdate { get; set; }
		public TimeClockEventArgs()
		{

		}
	}
}

