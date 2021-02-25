//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class CardEventQueue
	{
		readonly IObsManager obsManager;
		public string Name { get; set; }
		public CardEvent ActiveCardEvent { get; set; }
		public Queue<CardEvent> QueueCardEvents { get; set; } = new Queue<CardEvent>();
		public IDungeonMasterApp dungeonMasterApp { get; set; }

		public CardEventQueue()
		{

		}

		public void QueueEvent(QueueEffectEventArgs ea)
		{
			CardEvent cardEvent = CardEvent.Create(ea.cardEventName, ea.Args);
			if (cardEvent == null)
			{
				System.Diagnostics.Debugger.Break();
				return;
			}

			QueueCardEvents.Enqueue(cardEvent);
		}

		void DequeueNow()
		{
			ActiveCardEvent = QueueCardEvents.Dequeue();
			ActiveCardEvent.Activate(obsManager, dungeonMasterApp);
			ActiveCardEvent = null;
		}

		public void DequeueIfPossible()
		{
			if (QueueCardEvents.Count == 0)
				return;
			if (ActiveCardEvent == null || ActiveCardEvent.IsDone)
				DequeueNow();
		}

		public CardEventQueue(QueueEffectEventArgs ea, IObsManager obsManager, IDungeonMasterApp iDungeonMasterApp)
		{
			dungeonMasterApp = iDungeonMasterApp;
			this.obsManager = obsManager;
			Name = ea.cardEventName;
			QueueEvent(ea);
		}
	}
}
