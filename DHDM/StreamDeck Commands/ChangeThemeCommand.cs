using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ChangeThemeCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string newTheme;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SetTheme(newTheme);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ChangeTheme\s+(\w+)");
			if (match.Success)
			{
				newTheme = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}
