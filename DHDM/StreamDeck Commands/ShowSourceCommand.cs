using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ShowSourceCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string sourceName;
		string sceneName;
		bool sourceVisibility;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SetObsSourceVisibility(sceneName, sourceName, sourceVisibility);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ShowSource ([\w\._]+), ([\w\._]+), ([0|1|T])$");
			if (match.Success)
			{
				sourceName = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}
