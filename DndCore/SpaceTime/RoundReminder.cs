using System;
using System.Linq;

namespace DndCore
{
	public class RoundReminder
	{
		public Creature Creature { get; set; }
		public TurnPoint TurnPoint { get; set; }
		public int RoundNumber { get; set; }
		public string ReminderMessage { get; set; }
		public RoundReminder()
		{

		}
	}
}
