using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class TensCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			ApplyCommand.SetTens(tensValue);
		}

		string tensValue;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^Tens\s+(\d+)");
			if (match.Success)
			{
				tensValue = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
