using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class NpcScrollsToggleCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.NpcScrollsToggle();
		}

		public bool Matches(string message)
		{
			return message == "NpcScrollsToggle";
		}
	}
}
