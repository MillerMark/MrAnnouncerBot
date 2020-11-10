using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ShowWeatherCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string weatherKeyword;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.ShowWeather(weatherKeyword);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, @"^ShowWeather ([\w\._]+)$");
			if (match.Success)
			{
				weatherKeyword = match.Groups[1].Value;
				return true;
			}
			return false;
		}
	}
}
