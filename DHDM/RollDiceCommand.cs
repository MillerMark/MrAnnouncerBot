using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class RollDiceCommand : IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.RollDice();
		}

		public bool Matches(string message)
		{
			return message == "RollDice";
		}
	}
}
