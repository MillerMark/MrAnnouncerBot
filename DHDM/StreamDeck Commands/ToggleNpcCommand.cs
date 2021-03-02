using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ToggleNpcCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			int targetNum = GetInGameCreatureTargetNumber(targetNumStr);
			if (targetNum == int.MinValue)
				return;

			dungeonMasterApp.ToggleInGameCreature(targetNum);
		}

		public static int GetInGameCreatureTargetNumber(string targetNumStr)
		{
			int targetNum;
			if (targetNumStr == "{creature.count}")
				targetNum = (int)DigitManager.GetValue("creature");
			else
			{
				if (!int.TryParse(targetNumStr, out targetNum))
					return int.MinValue;
				else if (!Debouncer.CanToggle(targetNum))
					return int.MinValue;
			}

			return targetNum;
		}

		string targetNumStr;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ToggleInGameCreature\s+([\w\.\{\}]+)");
			if (match.Success)
			{
				targetNumStr = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
