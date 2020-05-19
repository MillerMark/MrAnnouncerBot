using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	// TODO: We can delete this.
	public class ApplyLastDamageCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.Apply("LastDamage", 0, GetPlayerIds(dungeonMasterApp, applyToAllPlayers));
		}

		bool applyToAllPlayers;
		public bool Matches(string message)
		{
			applyToAllPlayers = false;
			Match match = Regex.Match(message, @"^ApplyLastDamage" + PlayerSpecifier);

			if (!match.Success)
			{
				applyToAllPlayers = true;
				match = Regex.Match(message, @"^ApplyLastDamage");
			}
			if (match.Success)
			{
				if (!applyToAllPlayers)
				{
					SetTargetPlayer(match.Groups);
					if (TargetPlayer == null)
						applyToAllPlayers = true;
				}

				return true;
			}

			return false;
		}
	}
}
