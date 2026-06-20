using System;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DndCore
{
	public static class History
	{
		static ObservableCollection<LogEntry> queuedEntries = new ObservableCollection<LogEntry>();
		static StringBuilder logBuilder = new StringBuilder();

		static History()
		{

		}

		public static ObservableCollection<LogEntry> Entries { get; private set; } = new ObservableCollection<LogEntry>();
		public static DndTimeClock TimeClock { get; set; }
		public static string LogText { get => logBuilder.ToString(); }

		public static void Log(string message)
		{
			logBuilder.Append(message).Append('\n');
			lock (queuedEntries)
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
			catch //(Exception ex)
			{
				
			}
			lock (queuedEntries)
				queuedEntries.Clear();
		}
		public static void Clear()
		{
			logBuilder.Clear();
			Entries.Clear();
			lock (queuedEntries)
				queuedEntries.Clear();
		}

		public static event EventHandler LogUpdated;
	}
}

