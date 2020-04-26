using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class PlaySoundCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string soundFileName;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.PlaySound(soundFileName);
		}

		public bool Matches(string message)
		{
			const string playSoundCommand = "sfx ";
			if (!message.StartsWith(playSoundCommand))
				return false;

			soundFileName = message.Substring(playSoundCommand.Length).Trim();

			int bracketStart = soundFileName.IndexOf("[");
			if (soundFileName.EndsWith("]") && bracketStart > 0)
			{
				string numSoundFileStr = soundFileName.Substring(bracketStart + 1);
				soundFileName = soundFileName.Substring(0, bracketStart);
				numSoundFileStr = numSoundFileStr.Substring(0, numSoundFileStr.Length - 1);
				if (int.TryParse(numSoundFileStr, out int numSoundFiles))
				{
					int soundFileNumber = new Random().Next(numSoundFiles) + 1;
					soundFileName += soundFileNumber;
				}
			}

			return true;
		}
	}
}
