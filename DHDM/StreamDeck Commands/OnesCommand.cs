using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DndCore;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class OnesCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			ApplyCommand.SetOnes(oneValue);
		}

		string oneValue;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^Ones\s+(\d+)");
			if (match.Success)
			{
				oneValue = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
