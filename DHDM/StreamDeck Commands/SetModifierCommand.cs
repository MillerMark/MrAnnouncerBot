using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class SetModifierCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		int modifier;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SetModifier(modifier);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, $"^SetModifier\\(\\d\\)$");
			if (match.Success)
			{
				if (int.TryParse(match.Groups[1].Value, out modifier))
					return true;
			}
			return false;
		}
	}
}
