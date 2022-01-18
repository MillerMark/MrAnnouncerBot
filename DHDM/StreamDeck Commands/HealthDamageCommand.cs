using System;
using System.Linq;
using System.Text.RegularExpressions;
using DndCore;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class HealthDamageCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		int healthDamageValue;
		
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			DamageHealthChange damageHealthChange = new DamageHealthChange();
			damageHealthChange.DamageHealth = healthDamageValue;
			if (TargetPlayer == null)
			{
				damageHealthChange.PlayerIds.Add(int.MaxValue);
			}
			else
			{
				int playerId = dungeonMasterApp.GetPlayerIdFromName(TargetPlayer);
				if (playerId != -1)
					damageHealthChange.PlayerIds.Add(playerId);
			}
			dungeonMasterApp.ApplyDamageHealthChange(damageHealthChange);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"(^[\-\+]\d+)" + PlayerSpecifier);
			if (!match.Success)
			{
				match = Regex.Match(message, @"(^[\-\+]\d+)");
			}
			if (match.Success)
			{
				SetTargetPlayer(match.Groups);
				if (int.TryParse(match.Groups[match.Groups.Count - 2].Value, out healthDamageValue))
					return true;
			}
			return false;
		}
	}
}
