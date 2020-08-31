using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class SkillCheckCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string skillCheck;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.PrepareSkillCheck(skillCheck);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, $"^SkillCheck\\s+(\\w+)");
			if (match.Success)
			{
				skillCheck = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}
