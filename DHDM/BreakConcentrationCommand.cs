using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class BreakConcentrationCommand : IDungeonMasterCommand
	{
		int playerId;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.BreakConcentration(playerId);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, $"^bc [{RegexConstants.PlayerFirstInitials}]$");
			if (match.Success)
			{
				if (int.TryParse(match.Groups[1].Value, out playerId))
					return true;
			}
			return false;
		}
	}
}
