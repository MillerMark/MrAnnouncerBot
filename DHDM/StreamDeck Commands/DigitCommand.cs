using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DndCore;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class DigitCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			ApplyCommand.AddDigit(digit);
		}

		string digit;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^Digit\s+([\d\.])");
			if (match.Success)
			{
				digit = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
