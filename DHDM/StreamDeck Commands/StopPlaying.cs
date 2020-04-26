using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class StopPlaying : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string mainFolder;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.StopPlayer(mainFolder);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^StopPlaying\s+(\w+)");
			if (match.Success)
			{
				mainFolder = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}
