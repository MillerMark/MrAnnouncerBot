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
			int targetNum = ToggleNpcCommand.GetInGameCreatureTargetNumber(targetNumStr);
			if (targetNum == int.MinValue)
				return;
			dungeonMasterApp.ToggleTarget(targetNum);
		}

		string targetNumStr;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ToggleTarget\s+([\w\.\{\}]+)");
			if (match.Success)
			{
				targetNumStr = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
