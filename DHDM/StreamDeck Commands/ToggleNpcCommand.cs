using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ToggleNpcCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			if (int.TryParse(targetNumStr, out int targetNum))
				dungeonMasterApp.ToggleInGameCreature(targetNum);
		}

		string targetNumStr;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ToggleInGameCreature\s+(\d+)");
			if (match.Success)
			{
				targetNumStr = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
