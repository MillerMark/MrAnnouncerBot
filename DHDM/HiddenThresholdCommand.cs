using System;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class HiddenThresholdCommand : IDungeonMasterCommand
	{
		int hiddenThreshold;

		public void Execute(IDungeonMasterApp dungeonMasterApp, TwitchClient twitchClient, ChatMessage chatMessage)
		{
			dungeonMasterApp.SetHiddenThreshold(hiddenThreshold);
		}

		public bool Matches(string message)
		{
			return int.TryParse(message, out hiddenThreshold);
		}
	}
}
