using System;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ScrollCloseCommand : IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, TwitchClient twitchClient, ChatMessage chatMessage)
		{
			dungeonMasterApp.HideScroll();
		}

		public bool Matches(string message)
		{
			return message == "close scroll";
		}
	}
}
