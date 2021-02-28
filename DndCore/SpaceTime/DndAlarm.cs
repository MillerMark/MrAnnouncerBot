using System;
using System.Linq;

namespace DndCore
{
	public delegate void DndTimeEventHandler(object sender, DndTimeEventArgs ea);

	public class DndAlarm
	{
		public DndAlarm(DndTimeClock dndTimeClock, DateTime triggerTime, string name, int turnIndex, Creature creature, object data = null)
		{
			TurnIndex = turnIndex;
			Creature = creature;
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
		public Creature Creature { get; set; }
		public TurnSpecifier TurnSpecifier { get; set; } = TurnSpecifier.None;
		
		public void FireAlarm(DndTimeClock dndTimeClock)
		{
			AlarmFired?.Invoke(this, new DndTimeEventArgs(dndTimeClock, this));
		}

		public event DndTimeEventHandler AlarmFired;
	}
}

