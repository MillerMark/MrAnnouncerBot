using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class TargetMoveCommands : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string targetingCommand;
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.MoveTarget(targetingCommand);
		}

		public bool Matches(string message)
		{
			if (message == "TargetPreviousCreature" || message == "TargetNextCreature" || message == "TargetNextPlayer" || message == "TargetPreviousPlayer")
			{
				targetingCommand = message;
				return true;
			}
			return false;
		}
	}
}
