using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace DndCore
{
	public static class History
	{
		static ObservableCollection<LogEntry> queuedEntries = new ObservableCollection<LogEntry>();
		static string logText = string.Empty;

		static History()
		{

		}

		public static ObservableCollection<LogEntry> Entries { get; private set; } = new ObservableCollection<LogEntry>();
		public static DndTimeClock TimeClock { get; set; }
		public static string LogText { get => logText; }

		public static void Log(string message)
		{
			logText += message + "\n";
			queuedEntries.Add(new LogEntry(message, DateTime.Now, TimeClock.Time));
			OnLogUpdated(null, EventArgs.Empty);
		}

		public static void Log(LogEntry entry)
		{
			Entries.Add(entry);
		}

		public static void OnLogUpdated(object sender, EventArgs e)
		{
			LogUpdated?.Invoke(sender, e);
		}

		public static void UpdateQueuedEntries()
		{
			try
			{
				lock (queuedEntries)
				{
					foreach (LogEntry logEntry in queuedEntries.ToList())
					{
						Entries.Add(logEntry);
					}
				}
			}
			catch (Exception ex)
			{
				
			}
			queuedEntries.Clear();
		}
		public static void Clear()
		{
			logText = "";
			Entries.Clear();
			queuedEntries.Clear();
		}

		public static event EventHandler LogUpdated;
	}
}

