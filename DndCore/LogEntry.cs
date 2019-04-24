using System;
using System.Linq;
using System.Threading;

namespace DndCore
{
	public class LogEntry
	{
		public string Message { get; set; }
		public DateTime ActualTime { get; set; }
		public DateTime DndTime { get; set; }

		public LogEntry(string message)
		{
			Message = message;
			ActualTime = DateTime.Now;
			DndTime = DndTimeClock.Instance.Time;
		}
	}
}

