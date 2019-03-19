using System;
using System.Linq;

namespace DndCore
{
	public struct DndTimeSpan
	{
		public static readonly DndTimeSpan Zero = FromActions(0);
		public static readonly DndTimeSpan Never = Zero;
		public static readonly DndTimeSpan Forever = FromActions(int.MaxValue);
		public static readonly DndTimeSpan OneMinute = FromMinutes(1);

		public override bool Equals(object obj)
		{
			if (obj is DndTimeSpan compareSpan)
				return EqualsSpan(compareSpan);
			return base.Equals(obj);
		}

		public bool EqualsSpan(DndTimeSpan compareSpan)
		{
			return TimeMeasure == compareSpan.TimeMeasure && Count == compareSpan.Count;
		}

		public DndTimeSpan(TimeMeasure timeMeasure, int count)
		{
			Count = count;
			TimeMeasure = timeMeasure;
		}

		public static DndTimeSpan FromActions(int actionCount)
		{
			return new DndTimeSpan(TimeMeasure.actions, actionCount);
		}

		public static DndTimeSpan FromSeconds(int seconds)
		{
			return new DndTimeSpan(TimeMeasure.seconds, seconds);
		}

		public static DndTimeSpan FromMinutes(int minutes)
		{
			return new DndTimeSpan(TimeMeasure.minutes, minutes);
		}

		public static DndTimeSpan FromHours(int hours)
		{
			return new DndTimeSpan(TimeMeasure.hours, hours);
		}

		public static DndTimeSpan FromDays(int days)
		{
			return new DndTimeSpan(TimeMeasure.days, days);
		}

		public bool IsForever()
		{
			return TimeMeasure == TimeMeasure.actions && Count == int.MaxValue;
		}

		public TimeMeasure TimeMeasure { get; set; }
		public int Count { get; set; }
	}
}
