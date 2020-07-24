using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ToggleNpcCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		System.Collections.Generic.Dictionary<int, DateTime> secondsSinceLastToggle;
		bool CanToggle(int targetNum)
		{
			if (secondsSinceLastToggle == null)
				secondsSinceLastToggle = new System.Collections.Generic.Dictionary<int, DateTime>();
			if (!secondsSinceLastToggle.ContainsKey(targetNum))
			{
				secondsSinceLastToggle.Add(targetNum, DateTime.Now);
				return true;
			}
			if (DateTime.Now - secondsSinceLastToggle[targetNum] < TimeSpan.FromMilliseconds(200))
			{
				return false;
				// De-bounce it. Could be a bug in the stream deck sending two commands at once.
			}
			secondsSinceLastToggle[targetNum] = DateTime.Now;
			return true;
		}
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			if (int.TryParse(targetNumStr, out int targetNum))
				if (CanToggle(targetNum))
					dungeonMasterApp.ToggleInGameCreature(targetNum);
		}

		string targetNumStr;
		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ToggleInGameCreature\s+(\d+)");
			if (match.Success)
			{
				targetNumStr = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
