using System;
using System.Linq;
using System.Text.RegularExpressions;
using DndCore;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class HealthDamageCommand : IDungeonMasterCommand
	{
		int healthDamageValue;

		public void Execute(IDungeonMasterApp dungeonMasterApp, TwitchClient twitchClient, ChatMessage chatMessage)
		{
			DamageHealthChange damageHealthChange = new DamageHealthChange();
			damageHealthChange.DamageHealth = healthDamageValue;
			dungeonMasterApp.ApplyDamageHealthChange(damageHealthChange);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"([\-\+]\d+)");
			if (match.Success)
			{
				if (int.TryParse(match.Groups[0].Value, out healthDamageValue))
					return true;
			}
			return false;
		}
	}
}
