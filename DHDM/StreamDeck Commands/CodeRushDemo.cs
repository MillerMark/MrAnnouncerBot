using ObsControl;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class CodeRushDemo : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string sourceVideo;

		public CodeRushDemo()
		{
			//obsManager.Connect();
		}

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			ObsManager.SetSourceVisibility(sourceVideo, "CR.Templates", true);
			DndObsManager.SetSourceVisibility(sourceVideo, "CR.Templates", false, 7.7);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^CodeRushDemo (.+)$");
			if (match.Success)
			{
				sourceVideo = match.Groups[1].Value;
				return true;
			}

			return false;
		}
	}
}
