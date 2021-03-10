using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ScrollPageCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.ChangeScrollPage(scrollPage);
		}

		string scrollPage;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ShowScrollPage\s+(\w+)");
			if (match.Success)
			{
				scrollPage = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
