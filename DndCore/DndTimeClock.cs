using System;
using System.Linq;
using System.Threading;


namespace DndCore
{
	public class DndTimeClock
	{
		public const int Alturiak = 2;
		private const int AlturiakStart = 32;
		public const int Ches = 3;
		private const int ChesStart = 62;
		public const int Eleasis = 8;
		private const int EleasisStart = 214;
		public const int Eleint = 9;
		private const int EleintStart = 244;
		public const int Flamerule = 7;
		private const int FlameruleStart = 183;
		public const int Greengrass = -2;  // Holiday
		public const int GreengrassStart = 122;

		// Months & holidays...
		public const int Hammer = 1;

		private const int HammerStart = 1;
		public const int Highharvestide = -5;  // Holiday
		public const int HighharvestideStart = 274;
		public const int Kythorn = 6;
		private const int KythornStart = 153;
		public const int Marpenoth = 10;
		private const int MarpenothStart = 275;
		public const int Midsummer = -3;  // Holiday
		public const int MidsummerStart = 213;
		public const int Midwinter = -1;  // Holiday

		// Holidays...
		public const int MidwinterStart = 31;
		public const int Mirtul = 5;
		private const int MirtulStart = 123;
		public const int Nightal = 12;
		private const int NightalStart = 336;
		public const int Shieldmeet = -4;  // Leap Day/Holiday
		public const int ShieldmeetStart = 214;
		public const int Tarsakh = 4;
		private const int TarsakhStart = 92;
		public const int TheFeastOfTheMoon = -6;  // Holiday
		public const int TheFeastOfTheMoonStart = 335;
		public const int Uktar = 11;
		private const int UktarStart = 305;

		private readonly int[] MonthStartDays = { 0, HammerStart, AlturiakStart, ChesStart, TarsakhStart, MirtulStart, KythornStart, FlameruleStart, EleasisStart, EleintStart, MarpenothStart, UktarStart, NightalStart };
		TimeClockEventArgs timeClockEventArgs = new TimeClockEventArgs();

		static DndTimeClock()
		{
			Instance = new DndTimeClock();
		}

		public static DndTimeClock Instance { get; set; }
		public bool InCombat { get; set; }

		public DateTime Time { get; private set; }

		public void Advance(DndTimeSpan dndTimeSpan, bool reverseDirection = false)
		{
			Advance(dndTimeSpan.GetTimeSpan(), reverseDirection);
		}

		private void Advance(TimeSpan timeSpan, bool reverseDirection = false)
		{
			if (timeSpan == Timeout.InfiniteTimeSpan)
				throw new Exception("Cannot add infinity. COME ON!!!");
			if (reverseDirection)
				SetTime(Time - timeSpan);
			else
				SetTime(Time + timeSpan);
		}

		public void Advance(double milliseconds, bool reverseDirection = false)
		{
			Advance(TimeSpan.FromMilliseconds(milliseconds), reverseDirection);
		}

		public string AsDndDateString()
		{
			string yearStr = $", {Time.Year} DR";
			int dayOfYear = Time.DayOfYear;
			int monthOrHoliday = GetMonthOrHoliday(dayOfYear);
			string monthOrHolidayName = GetMonthOrHolidayName(monthOrHoliday);

			if (IsHoliday(monthOrHoliday))
				return monthOrHolidayName + yearStr;

			int dayOfMonth = GetDayOfMonth(dayOfYear, monthOrHoliday);
			string dayOfMonthSuffix = GetDaySuffix(dayOfMonth);

			return dayOfMonth + dayOfMonthSuffix + " of " + monthOrHolidayName + yearStr;
		}

		internal int GetDayOfMonth(int dayOfYear, int month)
		{
			int laterMonthsOffset = 0;
			int leapYearOffset = GetLeapYearOffset();

			if (month >= Eleasis)
				laterMonthsOffset = leapYearOffset;
			return dayOfYear - MonthStartDays[month] + 1 - laterMonthsOffset;
		}

		string GetDaySuffix(int dayOfMonth)
		{
			switch (dayOfMonth)
			{
				case 1:
				case 21:
					return "st";
				case 2:
				case 22:
					return "nd";
				case 3:
					return "rd";
				default:
					return "th";
			}
		}

		// Holidays...


		int GetLeapYearOffset()
		{
			if (Time.Year % 4 == 0)
				return 1;
			return 0;
		}

		public int GetMonthOrHoliday(int dayOfYear)
		{
			int leapYearOffset = GetLeapYearOffset();
			if (dayOfYear >= NightalStart + leapYearOffset)
				return Nightal;
			if (dayOfYear == TheFeastOfTheMoonStart + leapYearOffset)
				return TheFeastOfTheMoon;
			if (dayOfYear >= UktarStart + leapYearOffset)
				return Uktar;
			if (dayOfYear >= MarpenothStart + leapYearOffset)
				return Marpenoth;
			if (dayOfYear == HighharvestideStart + leapYearOffset)
				return Highharvestide;
			if (dayOfYear >= EleintStart + leapYearOffset)
				return Eleint;
			if (dayOfYear >= EleasisStart + leapYearOffset)
				return Eleasis;

			// Leap day (leapYearOffset only impacts months checked above)...
			if (leapYearOffset == 1 && dayOfYear == ShieldmeetStart)
				return Shieldmeet;


			if (dayOfYear == MidsummerStart)
				return Midsummer;
			if (dayOfYear >= FlameruleStart)
				return Flamerule;
			if (dayOfYear >= KythornStart)
				return Kythorn;
			if (dayOfYear >= MirtulStart)
				return Mirtul;

			if (dayOfYear == GreengrassStart)
				return Greengrass;

			if (dayOfYear >= TarsakhStart)
				return Tarsakh;

			if (dayOfYear >= ChesStart)
				return Ches;

			if (dayOfYear >= AlturiakStart)
				return Alturiak;

			if (dayOfYear >= HammerStart)
				return Hammer;

			return 0;
		}

		string GetMonthOrHolidayName(int monthOrHoliday)
		{
			switch (monthOrHoliday)
			{
				case Hammer:
					return "Hammer";
				case Alturiak:
					return "Alturiak";
				case Ches:
					return "Ches";
				case Tarsakh:
					return "Tarsakh";
				case Mirtul:
					return "Mirtul";
				case Kythorn:
					return "Kythorn";
				case Flamerule:
					return "Flamerule";
				case Eleasis:
					return "Eleasis";
				case Eleint:
					return "Eleint";
				case Marpenoth:
					return "Marpenoth";
				case Uktar:
					return "Uktar";
				case Nightal:
					return "Nightal";
				case Midwinter:
					return "Midwinter";
				case Greengrass:
					return "Greengrass";
				case Midsummer:
					return "Midsummer";
				case Highharvestide:
					return "Highharvestide";
				case TheFeastOfTheMoon:
					return "The Feast of the Moon";
				case Shieldmeet:
					return "Shieldmeet";
			}
			return string.Empty;
		}

		internal bool IsHoliday(int monthOrHoliday)
		{
			switch (monthOrHoliday)
			{
				case Midwinter:
				case Greengrass:
				case Midsummer:
				case Highharvestide:
				case TheFeastOfTheMoon:
				case Shieldmeet:
					return true;
			}
			return false;
		}

		protected virtual void OnTimeChanged(object sender, DateTime previousTime)
		{
			timeClockEventArgs.SpanSinceLastUpdate = Time - previousTime;
			TimeChanged?.Invoke(sender, timeClockEventArgs);
		}

		public void SetTime(DateTime time)
		{
			if (Time == time)
				return;
			DateTime previousTime = Time;
			Time = time;
			OnTimeChanged(this, previousTime);
		}

		public void SetTime(int year, int dayOfYear, int hour = 0, int minutes = 0, int seconds = 0)
		{
			SetTime(new DateTime(year, 1, 1).AddDays(dayOfYear - 1).AddHours(hour).AddMinutes(minutes).AddSeconds(seconds));
		}

		public delegate void TimeClockEventHandler(object sender, TimeClockEventArgs ea);
		public event TimeClockEventHandler TimeChanged;
	}
}

