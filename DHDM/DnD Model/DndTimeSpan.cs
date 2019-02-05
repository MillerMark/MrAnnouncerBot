using System;
using System.Linq;

namespace DHDM
{
	public class DndTimeSpan
	{
		public static readonly DndTimeSpan Zero = DndTimeSpan.FromActions(0);
		public static readonly DndTimeSpan Infinity = DndTimeSpan.FromActions(int.MaxValue);

		public DndTimeSpan(TimeMeasure timeMeasure, int count)
		{

		}

		static DndTimeSpan FromActions(int actionCount)
		{
			return new DndTimeSpan(TimeMeasure.actions, actionCount);
		}

		static DndTimeSpan FromSeconds(int seconds)
		{
			return new DndTimeSpan(TimeMeasure.seconds, seconds);
		}

		static DndTimeSpan FromMinutes(int minutes)
		{
			return new DndTimeSpan(TimeMeasure.seconds, minutes * 60);
		}

		static DndTimeSpan FromHours(int hours)
		{
			return DndTimeSpan.FromMinutes(hours * 60);
		}
	}
}
