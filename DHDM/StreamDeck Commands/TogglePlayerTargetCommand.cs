using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class TogglePlayerTargetCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		System.Collections.Generic.Dictionary<string, DateTime> secondsSinceLastToggle;

		bool CanToggle(string playerName)
		{
			if (secondsSinceLastToggle == null)
				secondsSinceLastToggle = new System.Collections.Generic.Dictionary<string, DateTime>();
			if (!secondsSinceLastToggle.ContainsKey(playerName))
			{
				secondsSinceLastToggle.Add(playerName, DateTime.Now);
				return true;
			}
			if (DateTime.Now - secondsSinceLastToggle[playerName] < TimeSpan.FromMilliseconds(200))
			{
				return false;
				// De-bounce it. Could be a bug in the stream deck sending two commands at once.
			}
			secondsSinceLastToggle[playerName] = DateTime.Now;
			return true;
		}

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			if (CanToggle(playerNameStr))
				dungeonMasterApp.TogglePlayerTarget(playerNameStr);
		}

		string playerNameStr;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^TogglePlayerTarget\s+(\w+)");
			if (match.Success)
			{
				playerNameStr = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
