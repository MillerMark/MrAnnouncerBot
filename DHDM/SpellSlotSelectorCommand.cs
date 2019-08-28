using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class SpellSlotSelectorCommand : IDungeonMasterCommand
	{
		int spellSlot;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SetSpellSlotLevel(spellSlot);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^sss ([0-7])$");
			if (match.Success)
				return int.TryParse(match.Groups[0].Value, out spellSlot);
			else
				return false;
		}
	}
}
