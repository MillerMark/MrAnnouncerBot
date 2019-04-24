using System;
using System.Linq;
using System.Threading;

namespace DndCore
{
	public struct DndTimeSpan
	{
		public static readonly DndTimeSpan Zero = FromActions(0);
		public static readonly DndTimeSpan Never = Zero;
		public static readonly DndTimeSpan Forever = FromActions(int.MaxValue);
		public static readonly DndTimeSpan OneMinute = FromMinutes(1);

		public TimeSpan GetTimeSpan()
		{
			switch (TimeMeasure)
			{
				case TimeMeasure.round:
					return TimeSpan.FromSeconds(6);
				case TimeMeasure.seconds:
					return TimeSpan.FromSeconds(Count);
				case TimeMeasure.minutes:
					return TimeSpan.FromMinutes(Count);
				case TimeMeasure.hours:
					return TimeSpan.FromHours(Count);
				case TimeMeasure.days:
					return TimeSpan.FromDays(Count);
				case TimeMeasure.forever:
					return Timeout.InfiniteTimeSpan;
			}
			return TimeSpan.Zero;
		}

		public bool Equals(DndTimeSpan other)
		{
			return TimeMeasure.Equals(other.TimeMeasure) && Count.Equals(other.Count);
		}

		public override bool Equals(object obj)
		{
			if (obj is DndTimeSpan dndTimeSpan)
			{
				return Equals(dndTimeSpan);
			}

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = 47;
				hashCode = (hashCode * 53) ^ (int)TimeMeasure;
				hashCode = (hashCode * 53) ^ Count.GetHashCode();
				return hashCode;
			}
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

		public static DndTimeSpan FromRounds(int rounds)
		{
			return new DndTimeSpan(TimeMeasure.round, rounds);
		}

		public bool IsForever()
		{
			return TimeMeasure == TimeMeasure.actions && Count == int.MaxValue;
		}
		

		public TimeMeasure TimeMeasure { get; set; }
		public int Count { get; set; }
	}
}
