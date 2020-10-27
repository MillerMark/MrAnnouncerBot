using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class TargetSkillCheckCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string skillCheck;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.PrepareTargetSkillCheck(skillCheck);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, $"^TargetSkillCheck\\s+(\\w+)");
			if (match.Success)
			{
				skillCheck = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}
