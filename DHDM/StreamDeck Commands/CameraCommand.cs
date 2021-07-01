using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class CameraCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string cameraCommand;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.TaleSpireCamera(cameraCommand);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^Camera\s+(\w+)");

			if (match.Success)
			{
				cameraCommand = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}

