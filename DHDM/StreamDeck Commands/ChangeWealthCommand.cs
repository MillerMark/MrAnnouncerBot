using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;
using DndCore;

namespace DHDM
{
	public class ChangeWealthCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		decimal deltaAmount;
		bool testAllPlayers;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			List<int> playerIds = GetPlayerIds(dungeonMasterApp, testAllPlayers);
			dungeonMasterApp.ChangeWealth(playerIds, deltaAmount);
		}

		public bool Matches(string message)
		{
			testAllPlayers = false;
			Match match = Regex.Match(message, @"^\$(-?\d*\.?\d+)" + PlayerSpecifier);
			
			if (!match.Success)
			{
				testAllPlayers = true;
				match = Regex.Match(message, @"^\$(\d+)");
			}
			if (match.Success)
			{
				if (!testAllPlayers)
				{
					SetTargetPlayer(match.Groups);
					if (TargetPlayer == null)
						testAllPlayers = true;
				}

				if (decimal.TryParse(match.Groups[1].Value, out deltaAmount))
					return true;
			}

			return false;
		}
	}
}
