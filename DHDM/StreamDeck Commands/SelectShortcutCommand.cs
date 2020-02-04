using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class SelectShortcutCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string shortcutName;
		string playerInitial;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SelectPlayerShortcut(shortcutName, dungeonMasterApp.GetPlayerIdFromNameStart(playerInitial));
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, $"^ss\\s+({RegexConstants.PlayerFirstInitials})\\s+([\\[\\]\\-\\,\\+\\:\\(\\)\\s\\w']+)$");
			if (match.Success)
			{
				playerInitial = match.Groups[1].Value;
				shortcutName = match.Groups[2].Value;
				return shortcutName != "";
			}
			return false;
		}
	}
}
