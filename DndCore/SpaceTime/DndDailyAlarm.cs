using System;
using System.Linq;

namespace DndCore
{
	public class DndDailyAlarm : DndAlarm
	{

		public DndDailyAlarm(DndTimeClock dndTimeClock, DateTime triggerTime, string name, Character player, object data = null) : base(dndTimeClock, triggerTime, name, player, data)
		{
		}
		public int DayOfYearLastTriggered { get; set; }
	}
}

