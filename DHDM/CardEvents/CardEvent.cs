//#define profiling
using DndCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DHDM
{
	public abstract class CardEvent
	{
		public string UserName { get; set; }
		public object[] Args { get; set; }
		public bool IsDone { get; set; }
		public IObsManager ObsManager { get; set; }
		public IDungeonMasterApp DungeonMasterApp { get; set; }
		public virtual void ConditionRoll(DiceRoll diceRoll)
		{
			
		}
		public CardEvent()
		{

		}

		public CardEvent(params object[] data)
		{
			Args = data;
		}
		
		public abstract void Activate();

		public static CardEvent Create(string cardEventName, string cardUserName, object[] args, IObsManager obsManager, IDungeonMasterApp iDungeonMasterApp)
		{
			// TODO: Create an instance of the correct CardEvent descendant.
			// TODO: Consider an elegant architecture.
			CardEvent result = null;

			if (cardEventName == "Weather")
				result = new ChangeWeatherCardEvent(args);
			else if (cardEventName == "Stampede")
				result = new StampedeCardEvent(args);

			if (result != null)
			{
				result.UserName = cardUserName;
				result.ObsManager = obsManager;
				result.DungeonMasterApp = iDungeonMasterApp;
			}
			return result;
		}
	}
}
