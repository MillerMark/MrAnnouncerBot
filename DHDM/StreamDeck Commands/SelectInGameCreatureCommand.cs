using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class SelectInGameCreatureCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			if (directionStr == "Previous")
				dungeonMasterApp.SelectPreviousInGameCreature();
			else
				dungeonMasterApp.SelectNextInGameCreature();
		}

		string directionStr;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^SelectInGameCreature\s+(\w+)");
			if (match.Success)
			{
				directionStr = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
