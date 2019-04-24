using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DndCore
{
	public static class History
	{
		static ObservableCollection<LogEntry> entries = new ObservableCollection<LogEntry>();

		public static ObservableCollection<LogEntry> Entries { get => entries; private set => entries = value; }

		public static void Log(string message)
		{
			entries.Add(new LogEntry(message));
		}

		public static void Log(LogEntry entry)
		{
			entries.Add(entry);
		}

		static History()
		{

		}
	}
}

