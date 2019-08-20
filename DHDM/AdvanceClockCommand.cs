using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class AdvanceClockCommand : IDungeonMasterCommand
	{
		int hours;
		int minutes;
		int seconds;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.AdvanceClock(hours, minutes, seconds);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ac\s+(\w+)$");
			if (match.Success)
			{
				//abilityToTest = DndUtils.ToAbility(match.Groups[1].Value);
				//return abilityToTest != Ability.None;
			}
			return false;
		}
	}
}
