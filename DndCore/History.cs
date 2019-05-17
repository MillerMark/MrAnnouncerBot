using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace DndCore
{
	public static class History
	{
		static ObservableCollection<LogEntry> entries = new ObservableCollection<LogEntry>();
		static ObservableCollection<LogEntry> queuedEntries = new ObservableCollection<LogEntry>();

		static History()
		{

		}

		public static ObservableCollection<LogEntry> Entries { get => entries; private set => entries = value; }
		public static DndTimeClock TimeClock { get; set; }

		public static void Log(string message)
		{
			queuedEntries.Add(new LogEntry(message, DateTime.Now, TimeClock.Time));
			OnLogUpdated(null, EventArgs.Empty);
		}

		public static void Log(LogEntry entry)
		{
			entries.Add(entry);
		}

		public static void OnLogUpdated(object sender, EventArgs e)
		{
			LogUpdated?.Invoke(sender, e);
		}

		public static void UpdateQueuedEntries()
		{
			foreach (LogEntry logEntry in queuedEntries)
			{
				entries.Add(logEntry);
			}
		}

		public static event EventHandler LogUpdated;
	}
}

