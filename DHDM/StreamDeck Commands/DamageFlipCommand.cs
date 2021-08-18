using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class DamageFlipCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string direction;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SetDamageSide(direction);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^DamageFlip\s+(\w+)");

			if (match.Success)
			{
				direction = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}

