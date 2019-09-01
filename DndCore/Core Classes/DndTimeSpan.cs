using System;
using System.Linq;
using System.Threading;

namespace DndCore
{
	public struct DndTimeSpan
	{
		public static readonly DndTimeSpan ShortRest = DndTimeSpan.FromHours(2);
		public static readonly DndTimeSpan LongRest = DndTimeSpan.FromHours(8);

		public static bool operator ==(DndTimeSpan left, DndTimeSpan right)
		{
			if ((object)left == null)
				return (object)right == null;
			else
				return left.Equals(right);
		}
		public static bool operator !=(DndTimeSpan left, DndTimeSpan right)
		{
			return !(left == right);
		}

		public DndTimeSpan(TimeMeasure timeMeasure, int count)
		{
			Count = count;
			TimeMeasure = timeMeasure;
		}

		public int Count { get; set; }

		public TimeMeasure TimeMeasure { get; set; }

		public static DndTimeSpan FromActions(int actionCount)
		{
			return new DndTimeSpan(TimeMeasure.actions, actionCount);
		}

		public static DndTimeSpan FromBonusActions(int bonusActionCount)
		{
			return new DndTimeSpan(TimeMeasure.bonusActions, bonusActionCount);
		}

		public static DndTimeSpan FromDays(int days)
		{
			return new DndTimeSpan(TimeMeasure.days, days);
		}

		public static DndTimeSpan FromHours(int hours)
		{
			return new DndTimeSpan(TimeMeasure.hours, hours);
		}

		public static DndTimeSpan FromMinutes(int minutes)
		{
			return new DndTimeSpan(TimeMeasure.minutes, minutes);
		}

		public static DndTimeSpan FromRounds(int rounds)
		{
			return new DndTimeSpan(TimeMeasure.round, rounds);
		}

		public static DndTimeSpan FromSeconds(int seconds)
		{
			return new DndTimeSpan(TimeMeasure.seconds, seconds);
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

		public bool IsForever()
		{
			return TimeMeasure == TimeMeasure.actions && Count == int.MaxValue;
		}

		public bool IsUnknown()
		{
			return TimeMeasure == TimeMeasure.actions && Count == int.MinValue;
		}

		public bool IsZero()
		{
			return TimeMeasure == TimeMeasure.instant || Count == 0;
		}
		
		/// <summary>
		/// Returns true if the duration is known and has a finite positive value.
		/// </summary>
		/// <returns></returns>
		public bool HasValue()
		{
			return !IsForever() && !IsUnknown() && !IsZero() && Count > 0;
		}
		
		public static DndTimeSpan FromString(string spanStr)
		{
			string spanStrLowerCase = spanStr.ToLower();
			if (spanStrLowerCase == "short rest")
				return ShortRest;
			if (spanStrLowerCase == "long rest")
				return LongRest;
			return Forever;
		}

		public static DndTimeSpan FromDurationStr(string durationStr)
		{
			string duration = durationStr.ToLower();

			if (duration == "instantaneous")
				return Zero;

			if (duration.EndsWith(" bonus action"))
				return OneBonusAction;

			if (duration.EndsWith(" action"))
				return OneAction;

			if (duration.IndexOf("reaction") > 0)
				return OneReaction;

			if (duration.IndexOf("minute") > 0)
				return FromMinutes(duration.GetFirstInt());

			if (duration.IndexOf("hour") > 0)
				return FromHours(duration.GetFirstInt());

			if (duration.IndexOf("day") > 0)
				return FromDays(duration.GetFirstInt());

			if (duration.IndexOf("round") > 0)
				return FromRounds(duration.GetFirstInt());

			return Zero;

		}

		public static readonly DndTimeSpan Zero = FromActions(0);
		public static readonly DndTimeSpan OneAction = FromActions(1);
		public static readonly DndTimeSpan OneBonusAction = new DndTimeSpan(TimeMeasure.bonusActions, 1);
		public static readonly DndTimeSpan OneReaction = new DndTimeSpan(TimeMeasure.reaction, 1);
		public static readonly DndTimeSpan Never = Zero;
		public static readonly DndTimeSpan Forever = FromActions(int.MaxValue);
		public static readonly DndTimeSpan Unknown = FromActions(int.MinValue);
		public static readonly DndTimeSpan OneMinute = FromMinutes(1);
	}
}
