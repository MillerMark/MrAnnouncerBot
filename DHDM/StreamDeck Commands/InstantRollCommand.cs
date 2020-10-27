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
		bool rollForTargets;
		DiceRollType diceRollType;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			List<int> playerIds = GetPlayerIds(dungeonMasterApp, testAllPlayers);
			if (dieStr.Contains("{count}"))
			{
				const int MaxDiceAllowed = 50;
				decimal value = ApplyCommand.GetValue();
				if (value == decimal.MinValue)
					value = 1;
				if (value > MaxDiceAllowed)
				{
					value = MaxDiceAllowed;
					dungeonMasterApp.TellDungeonMaster($"Unable to roll {value} dice at once. Maximum on-screen dice is limited to {MaxDiceAllowed}.");
				}
				dieStr = dieStr.Replace("{count}", value.ToString());
				ApplyCommand.ResetValue();
			}
			dungeonMasterApp.InstantDice(diceRollType, dieStr.Trim(), playerIds);
		}

		public bool Matches(string message)
		{
			testAllPlayers = false;
			Match match = Regex.Match(message, @"^InstantRoll\s+(\w+)\s+([\w\(\)\{\}\:""\s]+)" + PlayerSpecifier);
			if (!match.Success)
			{
				testAllPlayers = true;
				match = Regex.Match(message, @"^InstantRoll\s+(\w+)\s+([\w\(\)\{\}\:""\s]+)");
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
