using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class FlashlightCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string flashlightCommand;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.TaleSpireFlashlight(flashlightCommand);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^Flashlight\s+(\w+)");

			if (match.Success)
			{
				flashlightCommand = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}

