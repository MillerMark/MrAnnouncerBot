using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class ClearNumbersCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			ApplyCommand.ResetValue();
		}

		public bool Matches(string message)
		{
			return message == "ClearNumbers";
		}
	}
}
