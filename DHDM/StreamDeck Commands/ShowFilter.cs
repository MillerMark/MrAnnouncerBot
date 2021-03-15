using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ShowFilter : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		bool visible;
		string filterName;
		string sourceName;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			//TestManager.AddTest(keyword, test);
			dungeonMasterApp.ShowFilter(sourceName, filterName, visible);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ShowFilter\(""(.+)"", ""(.+)"", (true|false)\);");
			if (match.Success)
			{
				sourceName = match.Groups[1].Value;
				filterName = match.Groups[2].Value;
				string newVisibility = match.Groups[3].Value;
				visible = newVisibility == "true";
				return true;
			}

			return false;
		}
	}
}
