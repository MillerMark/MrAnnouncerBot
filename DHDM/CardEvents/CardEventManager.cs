//#define profiling
using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public static class CardEventManager
	{
		static Timer timer;
		static List<CardEventQueue> allCardEventQueues = new List<CardEventQueue>();

		static CardEventManager()
		{
			timer = new Timer(800);
			timer.Elapsed += Timer_Elapsed;
			timer.Start();
		}

		private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			foreach (CardEventQueue cardEventQueue in allCardEventQueues)
			{
				cardEventQueue.DequeueIfPossible();
			}
		}

		public static void QueueCardEvent(QueueEffectEventArgs ea, IObsManager obsManager, IDungeonMasterApp iDungeonMasterApp)
		{
			CardEventQueue queue = allCardEventQueues.FirstOrDefault(x => x.Name == ea.cardEventName);
			if (queue == null)
				allCardEventQueues.Add(new CardEventQueue(ea, obsManager, iDungeonMasterApp));
			else
				queue.QueueEvent(ea);
		}
	}
}
