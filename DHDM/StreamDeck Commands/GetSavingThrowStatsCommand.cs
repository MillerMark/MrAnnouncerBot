using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class GetSavingThrowStatsCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.GetSavingThrowStats();
		}

		public bool Matches(string message)
		{
			return message == "GetSavingThrowStats";
		}
	}
}
