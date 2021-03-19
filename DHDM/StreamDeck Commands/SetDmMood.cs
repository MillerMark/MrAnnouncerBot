using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class SetDmMood : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string moodName;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SetDmMood(moodName);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^SetDmMood (.+)$");
			if (match.Success)
			{
				moodName = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
