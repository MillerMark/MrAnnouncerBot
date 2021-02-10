using System;
using System.Linq;

namespace DndCore
{
	public class DndDailyAlarm : DndAlarm
	{

		public DndDailyAlarm(DndTimeClock dndTimeClock, DateTime triggerTime, string name, Creature creature, object data = null) : base(dndTimeClock, triggerTime, name, -1, creature, data)
		{
		}
		public int DayOfYearLastTriggered { get; set; }
	}
}

