using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class NextTurnCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.NextTurn();
		}

		public bool Matches(string message)
		{
			return message == "NextTurn";
		}
	}
}
