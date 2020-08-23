using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ToggleConditionCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string playerName, conditionStr;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.ToggleCondition(playerName, conditionStr);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ToggleCondition\s+(\w+)\s+(\w+)");
			if (match.Success)
			{
				playerName = match.Groups[1].Value;
				conditionStr = match.Groups[2].Value;
				return true;
			}

			return false;
		}
	}
}
