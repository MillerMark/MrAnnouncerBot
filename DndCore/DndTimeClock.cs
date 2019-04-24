using System;
using System.Linq;
using System.Threading;


namespace DndCore
{
	public class DndTimeClock
	{
		public event EventHandler TimeChanged;

		protected virtual void OnTimeChanged(object sender, EventArgs e)
		{
			TimeChanged?.Invoke(sender, e);
		}

		//private fields...
		static DndTimeClock instance;

		public static DndTimeClock Instance
		{
			get { return instance; }
			set
			{
				instance = value;
			}
		}

		static DndTimeClock()
		{
			instance = new DndTimeClock();
		}

		public DateTime Time { get; private set; }

		public void SetTime(DateTime time)
		{
			if (Time == time)
				return;
			Time = time;
			OnTimeChanged(this, EventArgs.Empty);
		}

		public void Advance(DndTimeSpan dndTimeSpan)
		{
			TimeSpan timeSpan = dndTimeSpan.GetTimeSpan();
			if (timeSpan == Timeout.InfiniteTimeSpan)
				throw new Exception("Cannot add infinity. COME ON!!!");
			SetTime(Time + timeSpan);
		}
	}
}

