using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ClearDiceCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.ClearDice();
		}

		public bool Matches(string message)
		{
			return message == "ClearDice";
		}
	}
}
