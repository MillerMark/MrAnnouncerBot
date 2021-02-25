//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public class StampedeCardEvent : CardEvent
	{
		public StampedeCardEvent(object[] args) : base(args)
		{
		}

		public override void Activate(IObsManager obsManager, IDungeonMasterApp dungeonMasterApp)
		{
			// TODO: Start the video... hook game events, 
		}
	}
}
