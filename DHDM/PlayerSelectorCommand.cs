using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class PlayerSelectorCommand : IDungeonMasterCommand
	{
		string playerInitial;

		public void Execute(IDungeonMasterApp dungeonMasterApp, TwitchClient twitchClient, ChatMessage chatMessage)
		{
			dungeonMasterApp.SelectCharacter(dungeonMasterApp.GetPlayerIdFromNameStart(playerInitial));
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^[flmaw]$");
			if (match.Success)
			{
				playerInitial = match.Groups[0].Value;
			}
			return match.Success;
		}
	}
}
