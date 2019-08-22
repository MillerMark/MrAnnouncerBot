using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class MoveFredCommand : IDungeonMasterCommand
	{
		string animationName;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.MoveFred(animationName);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^MoveFred\s+(\w+)$");
			if (match.Success)
			{
				animationName = match.Groups[1].Value;
				return animationName != "";
			}
			return false;
		}
	}
}
