using System;
using System.Linq;

namespace DndCore
{
	public delegate void DndTimeEventHandler(object sender, DndTimeEventArgs ea);

	public class DndAlarm
	{
		public DndAlarm(DndTimeClock dndTimeClock, DateTime triggerTime, string name, int turnIndex, Character player, object data = null)
		{
			TurnIndex = turnIndex;
			Player = player;
			Data = data;
			Name = name;
			TriggerTime = triggerTime;
			SetTime = dndTimeClock.Time;
		}

		public DateTime SetTime { get; set; }
		public DateTime TriggerTime { get; set; }
		public int TurnIndex { get; set; }
		public string Name { get; set; }
		public object Data { get; set; }
		public Character Player { get; set; }
		public RoundSpecifier RoundSpecifier { get; set; } = RoundSpecifier.None;
		
		public void FireAlarm(DndTimeClock dndTimeClock)
		{
			AlarmFired?.Invoke(this, new DndTimeEventArgs(dndTimeClock, this));
		}

		public event DndTimeEventHandler AlarmFired;
	}
}

