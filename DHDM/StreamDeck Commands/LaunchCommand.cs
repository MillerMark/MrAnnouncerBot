using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class LaunchCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string launchCommand;
		string dataValue;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			// TODO: Fill out the HandFxDto properties before sending.
			dungeonMasterApp.LaunchHandTrackingEffect(launchCommand, dataValue);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^Launch\s+(\w+)\s+(\d+)?");

			if (match.Success)
			{
				launchCommand = match.Groups[1].Value;
				if (match.Groups.Count > 2)
					dataValue = match.Groups[2].Value;
				else
					dataValue = null;
				return true;
			}
			return false;
		}
	}
}

