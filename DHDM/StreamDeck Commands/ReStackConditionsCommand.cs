using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ReStackConditionsCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string playerName, conditionStr;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.ReStackConditions();
		}

		public bool Matches(string message)
		{
			return message == "Re-stack Conditions";
		}
	}
}
