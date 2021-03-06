﻿using System;
using DndCore;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using System.Collections.Generic;

namespace DHDM
{
	public class SkillCheckNowCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		Skills skillToTest;
		bool testAllPlayers;
		bool testSelectedPlayers;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			List<int> playerIds = GetPlayerIds(dungeonMasterApp, testAllPlayers);
			if (testSelectedPlayers)
				playerIds = null;

			dungeonMasterApp.RollSkillCheck(skillToTest, playerIds);
		}

		public bool Matches(string message)
		{
			testAllPlayers = false;
			TargetPlayer = null;
			Match match = Regex.Match(message, @"^sk\s+(\w+)" + PlayerSpecifier);
			if (match.Success)
			{
				SetTargetPlayer(match.Groups);
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

			match = Regex.Match(message, @"^sks\s+(\w+)$");
			if (match.Success)
			{
				testSelectedPlayers = true;
				skillToTest = DndUtils.ToSkill(match.Groups[1].Value);
				return skillToTest != Skills.none;
			}
			return false;
		}
	}
}
