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

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.RollSavingThrow(abilityToTest);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^sv\s+(\w+)$");
			if (match.Success)
			{
				abilityToTest = DndUtils.ToAbility(match.Groups[1].Value);
				return abilityToTest != Ability.None;
			}
			return false;
		}
	}
}
