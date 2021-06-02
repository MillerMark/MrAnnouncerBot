using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class TargetTaleSpireCommands : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string targetingCommand;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.TaleSpireTarget(targetingCommand);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^TaleSpire Target\s+(\w+)");
			if (match.Success)
			{
				targetingCommand = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}
