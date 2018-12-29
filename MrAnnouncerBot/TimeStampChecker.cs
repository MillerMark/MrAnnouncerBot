using System;
using BotCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MrAnnouncerBot
{
	public class TimeStampChecker
	{
		const string STR_TimeStampsStorage = "TimeStamps.json";
		Dictionary<string, DateTime> timeStamps = new Dictionary<string, DateTime>();

		public Dictionary<string, DateTime> TimeStamps { get => timeStamps; set => timeStamps = value; }

		public void Add(string fileName)
		{
			timeStamps.Add(fileName, File.GetLastWriteTime(fileName));
		}

		public void Save()
		{
			AppData.Save(STR_TimeStampsStorage, this);
		}

		public static TimeStampChecker Load()
		{
			TimeStampChecker timeStamps = AppData.Load<TimeStampChecker>(STR_TimeStampsStorage);
			return timeStamps;
		}

		public bool NoChanges()
		{
			foreach (string key in timeStamps.Keys)
				if (timeStamps[key] != File.GetLastWriteTime(key))
					return false;
			return true;
		}
	}
}