using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ToggleTargetCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			if (int.TryParse(targetNumStr, out int targetNum))
				dungeonMasterApp.ToggleTarget(targetNum);
		}

		string targetNumStr;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ToggleTarget\s+(\d+)");
			if (match.Success)
			{
				targetNumStr = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
