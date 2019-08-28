using System;
using DndCore;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class SkillCheckCommand : IDungeonMasterCommand
	{
		Skills skillToTest;
		bool testAllPlayers;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.RollSkillCheck(skillToTest, testAllPlayers);
		}

		public bool Matches(string message)
		{
			testAllPlayers = false;
			Match match = Regex.Match(message, @"^sk\s+(\w+)$");
			if (match.Success)
			{
				skillToTest = DndUtils.ToSkill(match.Groups[1].Value);
				return skillToTest != Skills.none;
			}
			match = Regex.Match(message, @"^ska\s+(\w+)$");
			if (match.Success)
			{
				testAllPlayers = true;
				skillToTest = DndUtils.ToSkill(match.Groups[1].Value);
				return skillToTest != Skills.none;
			}
			return false;
		}
	}
}
