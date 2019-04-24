using System;
using System.Linq;
using System.Threading;


namespace DndCore
{
	public class DndTimeClock
	{
		public event EventHandler TimeChanged;

		protected virtual void OnTimeChanged(object sender, EventArgs e)
		{
			TimeChanged?.Invoke(sender, e);
		}

		//private fields...
		static DndTimeClock instance;

		public static DndTimeClock Instance
		{
			get { return instance; }
			set
			{
				instance = value;
			}
		}

		static DndTimeClock()
		{
			instance = new DndTimeClock();
		}

		public DateTime Time { get; private set; }

		public void SetTime(DateTime time)
		{
			if (Time == time)
				return;
			Time = time;
			OnTimeChanged(this, EventArgs.Empty);
		}

		public void Advance(DndTimeSpan dndTimeSpan)
		{
			TimeSpan timeSpan = dndTimeSpan.GetTimeSpan();
			if (timeSpan == Timeout.InfiniteTimeSpan)
				throw new Exception("Cannot add infinity. COME ON!!!");
			SetTime(Time + timeSpan);
		}

		private const int HammerStart = 1;
		private const int AlturiakStart = 32;
		private const int ChesStart = 62;
		private const int TarsakhStart = 92;
		private const int MirtulStart = 123;
		private const int KythornStart = 153;
		private const int FlameruleStart = 183;
		private const int EleasisStart = 214;
		private const int EleintStart = 244;
		private const int MarpenothStart = 275;
		private const int UktarStart = 305;
		private const int NightalStart = 336;

		private readonly int[] MonthStartDays = { 0, HammerStart, AlturiakStart, ChesStart, TarsakhStart, MirtulStart, KythornStart, FlameruleStart, EleasisStart, EleintStart, MarpenothStart, UktarStart, NightalStart };

		// Holidays...
		public const int MidwinterStart = 31;
		public const int GreengrassStart = 122;
		public const int MidsummerStart = 213;
		public const int ShieldmeetStart = 214;
		public const int HighharvestideStart = 274;
		public const int TheFeastOfTheMoonStart = 335;

		// Months & holidays...
		public const int Hammer = 1;
		public const int Midwinter = -1;  // Holiday
		public const int Alturiak = 2;
		public const int Ches = 3;
		public const int Tarsakh = 4;
		public const int Greengrass = -2;  // Holiday
		public const int Mirtul = 5;
		public const int Kythorn = 6;
		public const int Flamerule = 7;
		public const int Midsummer = -3;  // Holiday
		public const int Shieldmeet = -4;  // Leap Day/Holiday
		public const int Eleasis = 8;
		public const int Eleint = 9;
		public const int Highharvestide = -5;  // Holiday
		public const int Marpenoth = 10;
		public const int Uktar = 11;
		public const int TheFeastOfTheMoon = -6;  // Holiday
		public const int Nightal = 12;

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

		internal int GetDayOfMonth(int dayOfYear, int month)
		{
			int leapYearOffset = 0;
			if (month >= Eleasis)
				leapYearOffset = 1;
			return dayOfYear - MonthStartDays[month] + 1 - leapYearOffset;
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

		public void SetTime(int year, int dayOfYear, int hour = 0, int minutes = 0, int seconds = 0)
		{
			SetTime(new DateTime(year, 1, 1).AddDays(dayOfYear - 1).AddHours(hour).AddMinutes(minutes).AddSeconds(seconds));
		}
	}
}

