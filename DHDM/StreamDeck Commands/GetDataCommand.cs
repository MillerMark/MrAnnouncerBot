using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class GetDataCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string reportName;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.GetData(reportName);
		}

		public bool Matches(string message)
		{
			Match match = Regex.Match(message, $"^GetData\\s+([\\(\\)\\s\\w']+)$");
			if (match.Success)
			{
				reportName = match.Groups[1].Value;
				return reportName != "";
			}
			return false;
		}
	}
}
