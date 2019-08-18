using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class StaticCommands : IDungeonMasterCommand
	{
		DungeonMasterCommand command;

		public void Execute(IDungeonMasterApp dungeonMasterApp, TwitchClient twitchClient, ChatMessage chatMessage)
		{
			dungeonMasterApp.ExecuteCommand(command);
		}

		DungeonMasterCommand GetStaticCommand(string message)
		{
			if (message == "Enter Combat")
				return DungeonMasterCommand.EnterCombat;
			else if (message == "Exit Combat")
				return DungeonMasterCommand.ExitCombat;
			else if (message == "Clear Scroll Emphasis")
				return DungeonMasterCommand.ClearScrollEmphasis;
			else if (message == "Non-combat Initiative")
				return DungeonMasterCommand.NonCombatInitiative;
			else if (message == "Roll Wild Magic Check")
				return DungeonMasterCommand.RollWildMagicCheck;
			else if (message == "Roll Wild Magic")
				return DungeonMasterCommand.RollWildMagic;

			return DungeonMasterCommand.None;
		}
		public bool Matches(string message)
		{
			DungeonMasterCommand command = GetStaticCommand(message);
			if (command != DungeonMasterCommand.None)
			{
				this.command = command;
				return true;
			}
			return false;
		}
	}
}
