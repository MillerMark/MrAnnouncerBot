//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public class CardEventQueue
	{
		public string Name { get; set; }
		public CardEvent ActiveCardEvent { get; set; }
		public Queue<CardEvent> QueueCardEvents { get; set; } = new Queue<CardEvent>();

		public CardEventQueue()
		{

		}

		public void QueueEvent(QueueEffectEventArgs ea, IObsManager obsManager, IDungeonMasterApp iDungeonMasterApp)
		{
			CardEvent cardEvent = CardEvent.Create(ea.CardEventName, ea.CardUserName, ea.Args, obsManager, iDungeonMasterApp);
			if (cardEvent == null)
			{
				System.Diagnostics.Debugger.Break();
				return;
			}

			QueueCardEvents.Enqueue(cardEvent);
		}

		void DequeueNow()
		{
			if (!QueueCardEvents.Any())
				return;
			ActiveCardEvent = QueueCardEvents.Dequeue();
			ActiveCardEvent.Activate();
		}

		public void DequeueIfPossible()
		{
			if (ActiveCardEvent != null)
				if (!ActiveCardEvent.IsDone)
					return;
				else
					ActiveCardEvent = null;

			if (!QueueCardEvents.Any())
				return;

			DequeueNow();
		}
		public void ConditionRoll(DiceRoll diceRoll)
		{
			if (ActiveCardEvent != null)
			{
				ActiveCardEvent.ConditionRoll(diceRoll);
			}
		}

		public CardEventQueue(QueueEffectEventArgs ea, IObsManager obsManager, IDungeonMasterApp iDungeonMasterApp)
		{
			Name = ea.CardEventName;
			QueueEvent(ea, obsManager, iDungeonMasterApp);
		}
	}
}
