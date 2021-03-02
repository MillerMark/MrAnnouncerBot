using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class StampedeCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.StampedeNow();
		}

		public bool Matches(string message)
		{
			return message == "Stampede";
		}
	}
}
