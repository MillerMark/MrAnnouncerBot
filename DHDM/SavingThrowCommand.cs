using System;
using System.Linq;
using System.Text.RegularExpressions;
using DndCore;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class SavingThrowCommand : IDungeonMasterCommand
	{
		Ability abilityToTest;
		bool testAllPlayers;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.RollSavingThrow(abilityToTest, testAllPlayers);
		}

		public bool Matches(string message)
		{
			testAllPlayers = false;
			Match match = Regex.Match(message, @"^sv\s+(\w+)$");
			if (match.Success)
			{
				abilityToTest = DndUtils.ToAbility(match.Groups[1].Value);
				return abilityToTest != Ability.none;
			}
			match = Regex.Match(message, @"^sva\s+(\w+)$");
			if (match.Success)
			{
				testAllPlayers = true;
				abilityToTest = DndUtils.ToAbility(match.Groups[1].Value);
				return abilityToTest != Ability.none;
			}
			return false;
		}
	}
}
