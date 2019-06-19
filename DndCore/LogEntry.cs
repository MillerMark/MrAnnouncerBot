using System;
using System.Linq;
using System.Threading;

namespace DndCore
{
	public class LogEntry
	{
		public LogEntry(string message)
		{
			Message = message;
			ActualTime = DateTime.Now;
			DndTime = DndTimeClock.Instance.Time;
		}

		public LogEntry(string message, DateTime actualTime, DateTime dndTime)
		{
			Message = message;
			ActualTime = actualTime;
			DndTime = dndTime;
		}

		public DateTime ActualTime { get; set; }
		public DateTime DndTime { get; set; }
		public string Message { get; set; }
	}

	public class RollEntry
	{
		
		public RollEntry()
		{
			
		}
	}
}

