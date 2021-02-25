//#define profiling
using DndCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DHDM
{
	public abstract class CardEvent
	{
		public object[] Args { get; set; }
		public bool IsDone { get; set; }
		public CardEvent()
		{

		}

		public CardEvent(params object[] data)
		{
			Args = data;
		}
		public abstract void Activate(IObsManager obsManager, IDungeonMasterApp dungeonMasterApp);

		public static CardEvent Create(string cardEventName, object[] args)
		{
			// TODO: Create an instance of the correct CardEvent descendant.
			// TODO: Consider an elegant architecture.
			if (cardEventName == "Weather")
				return new ChangeWeatherCardEvent(args);
			else if (cardEventName == "Stampede")
				return new StampedeCardEvent(args);
			return null;
		}
	}
}
