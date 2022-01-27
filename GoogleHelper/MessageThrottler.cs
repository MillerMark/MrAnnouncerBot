using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;
using System.Reflection;

namespace GoogleHelper
{
	public class MessageThrottler<T> where T: class
	{
		List<string> tabNamesSeenSoFar = new List<string>();
		Timer timer = new Timer();

		object messageLock = new object();
		Dictionary<string, Queue<T>> messages = new Dictionary<string, Queue<T>>();

		DateTime lastBurstTime = DateTime.MinValue;
		readonly TimeSpan minTimeBetweenBursts;
		string defaultTabName = "No Name";
		string sheetNameName;

		public MessageThrottler(TimeSpan minTimeBetweenBursts)
		{
			this.minTimeBetweenBursts = minTimeBetweenBursts;
			timer.Elapsed += Timer_Elapsed;
			TabNameAttribute tabNameAttribute = typeof(T).GetCustomAttribute<TabNameAttribute>();
			if (tabNameAttribute != null)
				defaultTabName = tabNameAttribute.TabName;
			else
				defaultTabName = typeof(T).Name;

			SheetNameAttribute sheetNameAttribute = typeof(T).GetCustomAttribute<SheetNameAttribute>();
			sheetNameName = sheetNameAttribute.SheetName;
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			timer.Enabled = false;
			SendAllMessages();
		}

		void SendAllMessages()
		{
			lastBurstTime = DateTime.Now;
			lock (messageLock)
			{
				foreach (string tabName in messages.Keys)
				{
					string tabKey = $"{sheetNameName}.{tabName}";
					if (!tabNamesSeenSoFar.Contains(tabKey))
					{
						GoogleSheets.MakeSureTabExists<T>(tabName);
						tabNamesSeenSoFar.Add(tabKey);
					}

					if (messages[tabName].Any())
						GoogleSheets.AppendRows<T>(messages[tabName].ToArray(), tabName);
					messages[tabName].Clear();
				}
			}
		}

		public void AppendRow(T t, string tabName = null)
		{
			// ![](51A66B8E14E5482732D8778BFA4B00EC.png)

			if (tabName == null)
				tabName = defaultTabName;
			
			lock (messageLock)
			{
				if (!messages.ContainsKey(tabName))
					messages[tabName] = new Queue<T>();
				messages[tabName].Enqueue(t);
			}

			DateTime now = DateTime.Now;
			TimeSpan timeSinceLastBurst = now - lastBurstTime;
			bool firstBurst = lastBurstTime == DateTime.MinValue;
			if (firstBurst || timeSinceLastBurst > minTimeBetweenBursts)
				SendAllMessages();
			else if (!timer.Enabled)
			{
				DateTime nextScheduledBurstTime = lastBurstTime + minTimeBetweenBursts;
				TimeSpan timeTillNextBurst = nextScheduledBurstTime - now;
				timer.Interval = timeTillNextBurst.TotalMilliseconds;
				timer.Enabled = true;
			}
		}

		public void FlushAllMessages()
		{
			SendAllMessages();
		}
	}
}

