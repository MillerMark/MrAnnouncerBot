using System;
using System.Linq;

namespace DndCore
{
	public class DndTimeEventArgs : EventArgs
	{
		public DndTimeClock TimeClock { get; set; }
		public DndAlarm Alarm { get; set; }
		public DndTimeEventArgs(DndTimeClock timeClock, DndAlarm alarm)
		{
			TimeClock = timeClock;
			Alarm = alarm;
		}
	}
}

