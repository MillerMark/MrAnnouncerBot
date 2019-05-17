using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace DndCore
{
	public static class History
	{
		static ObservableCollection<LogEntry> queuedEntries = new ObservableCollection<LogEntry>();

		static History()
		{

		}

		public static ObservableCollection<LogEntry> Entries { get; private set; } = new ObservableCollection<LogEntry>();
		public static DndTimeClock TimeClock { get; set; }

		public static void Log(string message)
		{
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
			foreach (LogEntry logEntry in queuedEntries)
			{
				Entries.Add(logEntry);
			}
		}

		public static event EventHandler LogUpdated;
	}
}

