using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DndCore;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ChangeVolumeCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		int newVolume;
		string mainFolder;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SetPlayerVolume(mainFolder, newVolume);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ChangeVolume\s+(\w+)\s+(\d+)");
			if (match.Success)
			{
				mainFolder = match.Groups[1].Value;
				if (int.TryParse(match.Groups[2].Value, out newVolume))
					return true;
			}
			return false;
		}
	}
}
