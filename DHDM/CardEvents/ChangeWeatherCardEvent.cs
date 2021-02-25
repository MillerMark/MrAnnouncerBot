//#define profiling
using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public class ChangeWeatherCardEvent : CardEvent
	{
		public ChangeWeatherCardEvent(object[] args) : base(args)
		{
		}

		public override void Activate(IObsManager obsManager, IDungeonMasterApp dungeonMasterApp)
		{
			string weatherKeyword = Expressions.GetStr((string)Args[1]);
			dungeonMasterApp.ShowWeather(weatherKeyword);
		}
	}
}
