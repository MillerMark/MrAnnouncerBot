using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class BreakConcentrationCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string playerInitial;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.BreakConcentration(dungeonMasterApp.GetPlayerIdFromName(playerInitial));
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, $"^bc ({RegexConstants.PlayerFirstInitials})$");
			if (match.Success)
			{
				playerInitial = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}
