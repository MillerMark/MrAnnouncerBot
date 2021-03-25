using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ContestCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string contest;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.Contest(contest);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, $"^Contest\\s+(\\w+)");
			if (match.Success)
			{
				contest = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}
