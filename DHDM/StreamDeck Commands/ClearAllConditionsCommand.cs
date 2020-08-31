using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ClearAllConditionsCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string targetName;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.ClearAllConditions(targetName);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, $"^ClearAllConditions\\s+(\\w+)");
			if (match.Success)
			{
				targetName = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}
