using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class InGameUICommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string command;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.InGameUICommand(command);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^InGameUI\s+(\w+)");
			if (match.Success)
			{
				command = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}
