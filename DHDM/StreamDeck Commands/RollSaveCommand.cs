using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class RollSaveCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		int diceId;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.RollSave(diceId);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, $"^RollSave\\(\\d\\)$");
			if (match.Success)
			{
				if (int.TryParse(match.Groups[1].Value, out diceId))
					return true;
			}
			return false;
		}
	}
}
