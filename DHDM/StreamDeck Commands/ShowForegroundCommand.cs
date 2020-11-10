using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ShowForegroundCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string sourceName;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.ShowForeground(sourceName);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ShowForeground ([\w\._]+)$");
			if (match.Success)
			{
				sourceName = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}
