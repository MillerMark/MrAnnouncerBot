using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ChangeCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string subFolder;
		string mainFolder;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SetPlayerFolder(mainFolder, subFolder);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^Change\s+(\w+)\s+(\w+)");
			if (match.Success)
			{
				mainFolder = match.Groups[1].Value;
				subFolder = match.Groups[2].Value;
				return true;
			}
			return false;
		}
	}
}
