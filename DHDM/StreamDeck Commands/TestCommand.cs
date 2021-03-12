using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class TestCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			//TestManager.AddTest(keyword, test);
		}

		string testCommand;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^Test\s+(\w+)");
			if (match.Success)
			{
				testCommand = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
