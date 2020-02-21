using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using DndCore;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class PlayerSelectorCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string playerInitial;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SelectCharacter(dungeonMasterApp.GetPlayerIdFromNameStart(playerInitial));
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, $"^{RegexConstants.PlayerFirstInitials}$");
			if (match.Success)
			{
				playerInitial = match.Groups[0].Value;
			}
			return match.Success;
		}
	}
}
