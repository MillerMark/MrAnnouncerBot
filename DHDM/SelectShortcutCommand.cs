using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class SelectShortcutCommand : IDungeonMasterCommand
	{
		string shortcutName;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SelectPlayerShortcut(shortcutName);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ss\s+(\w+)$");
			if (match.Success)
			{
				shortcutName = match.Groups[1].Value;
				return shortcutName != "";
			}
			return false;
		}
	}
}
