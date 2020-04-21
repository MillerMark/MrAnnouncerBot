using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DndCore;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ChangeThemeVolumeCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		int newVolume;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SetThemeVolume(newVolume);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ChangeThemeVolume\s+(\d+)");
			if (match.Success && int.TryParse(match.Groups[1].Value, out newVolume))
				return true;
			return false;
		}
	}
}
