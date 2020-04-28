using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DndCore;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class InstantRollCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string dieStr;
		bool testAllPlayers;
		DiceRollType diceRollType;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			List<int> playerIds = GetPlayerIds(dungeonMasterApp, testAllPlayers);
			dungeonMasterApp.InstantDice(diceRollType, dieStr, playerIds);
		}

		public bool Matches(string message)
		{
			testAllPlayers = false;
			Match match = Regex.Match(message, @"^InstantRoll\s+(\w+)\s+(\w+)" + PlayerSpecifier);
			if (!match.Success)
			{
				testAllPlayers = true;
				match = Regex.Match(message, @"^InstantRoll\s+(\w+)\s+(\w+)");
			}
			if (match.Success)
			{
				if (!testAllPlayers)
					SetTargetPlayer(match.Groups);
				dieStr = match.Groups[2].Value;
				diceRollType = DndUtils.ToDiceRollType(match.Groups[1].Value);
				return true;
			}

			testAllPlayers = false;
			return false;
		}
	}
}
