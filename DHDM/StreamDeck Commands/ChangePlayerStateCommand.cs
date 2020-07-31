using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ChangePlayerStateCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.ChangePlayerStateCommand(command, data);
		}

		string command;
		string data;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ChangePlayerState\s+(\w+)\s+(\w+)");
			if (match.Success)
			{
				command = match.Groups[1].Value;
				data = match.Groups[2].Value;
				return true;
			}

			match = Regex.Match(message, @"^ChangePlayerState\s+(\w+)");
			if (match.Success)
			{
				command = match.Groups[1].Value;
				data = "";
				return true;
			}


			return false;
		}
	}
}
