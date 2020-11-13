using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class HandFxCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		HandFxDto handFxDto;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			// TODO: Fill out the HandFxDto properties before sending.
			dungeonMasterApp.TriggerHandFx(handFxDto);
			handFxDto = null;
		}

		public bool Matches(string message)
		{
			string[] lines = message.Split(';');
			if (lines.Length > 0 && lines[0].StartsWith("HandFx"))
			{
				handFxDto = new HandFxDto();
				bool success = false;
				foreach (string line in lines)
				{
					Match match = Regex.Match(line.Trim(), @"^HandFx\s+(\w+)\s+(\d+)(?:\s+(\d+)%)?");

					if (match.Success)
					{
						success = true;
						int scalePercent = 100;
						if (match.Groups.Count > 3 && !string.IsNullOrWhiteSpace(match.Groups[3].Value))
							scalePercent = int.Parse(match.Groups[3].Value);
						handFxDto.AddHandEffect(match.Groups[1].Value, int.Parse(match.Groups[2].Value), scalePercent);
					}
				}
				return success;
			}
			return false;
		}
	}
}

