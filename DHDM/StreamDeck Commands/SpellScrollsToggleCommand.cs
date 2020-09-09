using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class SpellScrollsToggleCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SpellScrollsToggle();
		}

		public bool Matches(string message)
		{
			return message == "SpellScrollsToggle";
		}
	}
}
