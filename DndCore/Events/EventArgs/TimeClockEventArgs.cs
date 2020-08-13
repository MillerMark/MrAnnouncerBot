using System;
using System.Linq;


namespace DndCore
{
	public class TimeClockEventArgs : EventArgs
	{
		public TimeClockEventArgs()
		{

		}

		public TimeSpan SpanSinceLastUpdate { get; set; }
		public int PreviousTurnIndex { get; set; }
	}
}

