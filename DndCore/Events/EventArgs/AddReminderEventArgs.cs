using System;

namespace DndCore
{
	public class AddReminderEventArgs : EventArgs
	{
		public AddReminderEventArgs(string reminder, string fromNowDuration)
		{
			Reminder = reminder;
			NowDuration = fromNowDuration;
		}

		public string NowDuration { get; set; }
		public string Reminder { get; set; }
	}
}
