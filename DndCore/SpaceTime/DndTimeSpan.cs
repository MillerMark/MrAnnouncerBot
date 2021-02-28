using System;
using System.Linq;
using System.Threading;

namespace DndCore
{
	public enum TurnSpecifier
	{
		None,
		StartOfTurn,
		EndOfTurn
	}
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

		public static bool operator >(DndTimeSpan left, DndTimeSpan right)
		{
			return left.ToSeconds() > right.ToSeconds();
		}

		public static bool operator <(DndTimeSpan left, DndTimeSpan right)
		{
			return left.ToSeconds() < right.ToSeconds();
		}

		public static bool operator >=(DndTimeSpan left, DndTimeSpan right)
		{
			return left.ToSeconds() >= right.ToSeconds();
		}

		public static bool operator <=(DndTimeSpan left, DndTimeSpan right)
		{
			return left.ToSeconds() <= right.ToSeconds();
		}

		public DndTimeSpan(TimeMeasure timeMeasure, int count)
		{
			Count = count;
			TimeMeasure = timeMeasure;
			TurnSpecifier = TurnSpecifier.None;
		}

		public int Count { get; set; }

		public TimeMeasure TimeMeasure { get; set; }
		public TurnSpecifier TurnSpecifier { get; set; }

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

		public static DndTimeSpan FromTurns(int turns)
		{
			return new DndTimeSpan(TimeMeasure.turns, turns);
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

		///// <summary>
		///// Implicitly converts an instance of type DndTimeSpan to a new instance of type TimeSpan.
		///// </summary>
		///// <param name="obj">An instance of type DndTimeSpan to convert.</param>
		///// <returns>Returns a new instance of type TimeSpan, derived from the specified DndTimeSpan instance.</returns>
		//public static implicit operator TimeSpan(DndTimeSpan obj)
		//{
		//	return obj.GetTimeSpan();
		//}

		public TimeSpan GetTimeSpan()
		{
			switch (TimeMeasure)
			{
				case TimeMeasure.forever:
					return Timeout.InfiniteTimeSpan;
				case TimeMeasure.never:
					return TimeSpan.MinValue;
				case TimeMeasure.instant:
					return TimeSpan.FromSeconds(0);
				default:
					return TimeSpan.FromSeconds(ToSeconds());
			}
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

			if (duration == "short rest")
				return ShortRest;

			if (duration == "long rest")
				return LongRest;

			if (duration == "instantaneous")
				return Zero;

			if (duration.EndsWith("bonus action"))
				return OneBonusAction;

			if (duration.EndsWith(" action"))
				return FromActions(duration.GetFirstInt(1));

			if (duration.IndexOf("reaction") > 0)
				return OneReaction;

			if (duration.IndexOf("minute") > 0)
				return FromMinutes(duration.GetFirstInt(1));

			if (duration.IndexOf("hour") > 0)
				return FromHours(duration.GetFirstInt(1));

			if (duration.IndexOf("day") > 0 || duration.IndexOf("dawn") > 0)
				return FromDays(duration.GetFirstInt(1));

			if (duration.IndexOf("round") > 0)
			{
				DndTimeSpan dndTimeSpan = FromRounds(duration.GetFirstInt(1));
				dndTimeSpan.TurnSpecifier = DndUtils.GetTurnSpecifier(duration);
				return dndTimeSpan;
			}

			if (duration == "day" || duration == "dawn")
				return FromDays(1);

			if (duration == "hour")
				return FromHours(1);

			if (duration == "minute")
				return FromMinutes(1);

			if (duration == "second")
				return FromSeconds(1);

			if (duration == "turn")
				return FromTurns(1);

			return Zero;

		}

		private const double DurationSeconds = 1;
		private const double DurationInstant = 0d;
		private const double DurationActions = DurationSeconds / 10d;
		private const double DurationBonusActions = DurationSeconds / 100d;
		private const double DurationReactions = DurationSeconds / 1000d;
		private const double DurationRound = 6 * DurationSeconds;
		private const double DurationMinutes = DurationSeconds * 60;
		private const double DurationHours = DurationMinutes * 60;
		private const double DurationDays = DurationHours * 24;
		private const double DurationForever = double.MaxValue;
		private const double DurationNever = double.MinValue;

		public double ToSeconds()
		{
			switch (TimeMeasure)
			{
				case TimeMeasure.instant:
					return DurationInstant;
				case TimeMeasure.actions:
					return Count * DurationActions;
				case TimeMeasure.seconds:
					return Count * DurationSeconds;
				case TimeMeasure.minutes:
					return Count * DurationMinutes;
				case TimeMeasure.hours:
					return Count * DurationHours;
				case TimeMeasure.days:
					return Count * DurationDays;
				case TimeMeasure.forever:
					return DurationForever;
				case TimeMeasure.never:
					return DurationNever;
				case TimeMeasure.round:
					return Count * DurationRound;
				case TimeMeasure.bonusActions:
					return Count * DurationBonusActions;
				case TimeMeasure.reaction:
					return Count * DurationReactions;
			}
			return 0;
		}

		public static readonly DndTimeSpan Zero = FromActions(0);
		public static readonly DndTimeSpan OneAction = FromActions(1);
		public static readonly DndTimeSpan OneRound = FromRounds(1);
		public static readonly DndTimeSpan OneBonusAction = new DndTimeSpan(TimeMeasure.bonusActions, 1);
		public static readonly DndTimeSpan OneReaction = new DndTimeSpan(TimeMeasure.reaction, 1);


		// TODO: Consider swapping initialized values for Never and Unknown (with ripple effects).
		public static readonly DndTimeSpan Never = Zero;
		public static readonly DndTimeSpan Unknown = FromActions(int.MinValue);


		public static readonly DndTimeSpan Forever = FromActions(int.MaxValue);
		public static readonly DndTimeSpan OneMinute = FromMinutes(1);
	}
}
