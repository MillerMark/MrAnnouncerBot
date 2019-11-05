using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace DndCore
{
	public class DndTimeClock
	{

		// Months & holidays...
		public const int Hammer = 1;
		const int HammerStart = 1;
		public const int Alturiak = 2;
		const int AlturiakStart = 32;
		public const int Ches = 3;
		const int ChesStart = 62;
		public const int Eleasis = 8;
		const int EleasisStart = 214;
		public const int Eleint = 9;
		const int EleintStart = 244;
		public const int Flamerule = 7;
		const int FlameruleStart = 183;
		public const int Greengrass = -2;  // Holiday
		public const int GreengrassStart = 122;
		public const int Highharvestide = -5;  // Holiday
		public const int HighharvestideStart = 274;
		public const int Kythorn = 6;
		const int KythornStart = 153;
		public const int Marpenoth = 10;
		const int MarpenothStart = 275;
		public const int Midsummer = -3;  // Holiday
		public const int MidsummerStart = 213;
		public const int Midwinter = -1;  // Holiday
		public const int MidwinterStart = 31;
		public const int Mirtul = 5;
		const int MirtulStart = 123;
		public const int Nightal = 12;
		const int NightalStart = 336;
		public const int Shieldmeet = -4;  // Leap Day/Holiday
		public const int ShieldmeetStart = 214;
		public const int Tarsakh = 4;
		const int TarsakhStart = 92;
		public const int TheFeastOfTheMoon = -6;  // Holiday
		public const int TheFeastOfTheMoonStart = 335;
		public const int Uktar = 11;
		const int UktarStart = 305;

		readonly int[] MonthStartDays = { 0, HammerStart, AlturiakStart, ChesStart, TarsakhStart, MirtulStart, KythornStart, FlameruleStart, EleasisStart, EleintStart, MarpenothStart, UktarStart, NightalStart };
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

		void Advance(TimeSpan timeSpan, bool reverseDirection = false)
		{
			if (timeSpan == Timeout.InfiniteTimeSpan)
				throw new Exception("Cannot add infinity. COME ON!!!");
			if (reverseDirection)  // Not fully supported.
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

		List<DndAlarm> alarmsToRemove = new List<DndAlarm>();
		bool triggeringAlarms;
		void TriggerAlarms(DateTime futureTime)
		{
			triggeringAlarms = true;
			try
			{
				for (int i = 0; i < alarms.Count; i++)
				{
					DndAlarm alarm = alarms[i];
					if (alarm.TriggerTime > futureTime)
						break;

					TriggerAlarm(alarm);
				}

				for (int i = 0; i < dailyAlarms.Count; i++)
				{
					DndDailyAlarm dailyAlarm = dailyAlarms[i];
					int daysFromNow = (futureTime - dailyAlarm.TriggerTime).Days;
					if (daysFromNow != 0)
						dailyAlarm.TriggerTime = dailyAlarm.TriggerTime.AddDays(daysFromNow);
					if (dailyAlarm.TriggerTime > futureTime || dailyAlarm.DayOfYearLastTriggered == dailyAlarm.TriggerTime.DayOfYear)
						break;
					dailyAlarm.DayOfYearLastTriggered = dailyAlarm.TriggerTime.DayOfYear;
					TriggerAlarm(dailyAlarm);
				}
			}
			finally
			{
				triggeringAlarms = false;
			}

			RemoveExpiredAlarms();
		}

		private void TriggerAlarm(DndAlarm alarm)
		{
			if (alarm.TriggerTime <= Time)
				return;

			Time = alarm.TriggerTime;
			try
			{
				alarm.FireAlarm(this);
			}
			catch (Exception ex)
			{
				// TODO: Fire an OnException event handler....
			}
			alarmsToRemove.Add(alarm);
		}

		private void RemoveExpiredAlarms()
		{
			foreach (DndAlarm alarmToRemove in alarmsToRemove)
			{
				alarms.Remove(alarmToRemove);
			}
		}

		void ReengagePreviouslyTriggeredAlarms()
		{
			
		}
		public void SetTime(DateTime time)
		{
			if (Time == time)
				return;
			DateTime previousTime = Time;

			if (time > Time)  // Moving forward
				TriggerAlarms(time);
			else
				ReengagePreviouslyTriggeredAlarms();
			Time = time;
			OnTimeChanged(this, previousTime);
		}

		public void SetTime(int year, int dayOfYear, int hour = 0, int minutes = 0, int seconds = 0)
		{
			SetTime(new DateTime(year, 1, 1).AddDays(dayOfYear - 1).AddHours(hour).AddMinutes(minutes).AddSeconds(seconds));
		}

		public event TimeClockEventHandler TimeChanged;

		List<DndAlarm> alarms = new List<DndAlarm>();
		List<DndDailyAlarm> dailyAlarms = new List<DndDailyAlarm>();

		public DndAlarm CreateAlarm(TimeSpan fromNow, string name, Character player = null, object data = null)
		{
			if (fromNow.TotalSeconds <= 0)
				return null;

			DndAlarm dndAlarm = new DndAlarm(this, Time + fromNow, name, player, data);
			alarms.Add(dndAlarm);
			alarms.Sort((x, y) => x.TriggerTime.CompareTo(y.TriggerTime));
			return dndAlarm;
		}

		public DndDailyAlarm CreateDailyAlarm(string name, int hours, int minutes = 0, int seconds = 0, Character player = null, object data = null)
		{
			DndDailyAlarm dndAlarm = new DndDailyAlarm(this, new DateTime(Time.Year, Time.Month, Time.Day, hours, minutes, seconds), name, player, data);
			dailyAlarms.Add(dndAlarm);
			dailyAlarms.Sort((x, y) => x.TriggerTime.CompareTo(y.TriggerTime));
			return dndAlarm;
		}

		public string AsFullDndDateTimeString()
		{
			return Time.ToString("H:mm:ss") + ", " + AsDndDateString();
		}
		
		public DndAlarm GetAlarm(string alarmName)
		{
			DndAlarm first = alarms.FirstOrDefault(x => x.Name == alarmName);
			if (first != null)
				return first;
			return dailyAlarms.FirstOrDefault(x => x.Name == alarmName);
		}

		public void ClearAllAlarms()
		{
			alarms = new List<DndAlarm>();
			dailyAlarms = new List<DndDailyAlarm>();
		}
		public void RemoveAlarm(string alarmName)
		{
			if (triggeringAlarms)
			{
				alarmsToRemove.AddRange(alarms.Where(x => x.Name == alarmName));
				alarmsToRemove.AddRange(dailyAlarms.Where(x => x.Name == alarmName));
			}
			else
			{
				alarms.RemoveAll(x => x.Name == alarmName);
				dailyAlarms.RemoveAll(x => x.Name == alarmName);
			}
		}
	}
}

