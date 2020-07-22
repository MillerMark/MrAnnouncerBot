using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class TargetCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.TargetCommand(command);
		}

		string command;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^TargetCommand\s+(\w+)");
			if (match.Success)
			{
				command = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
