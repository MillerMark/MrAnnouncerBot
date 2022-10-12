using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Events;

namespace BotCore
{
	public static class BotCommands
	{
		static List<BotCommand> allChatCommands = new List<BotCommand>();

		public static void Register(BotCommand chatCommand)
		{
			allChatCommands.Add(chatCommand);
		}

		static List<BotCommand> GetAll(string commandName)
		{
			return allChatCommands.Where(x => (x.Command == commandName || x.Command.TrimEnd('*') == commandName)).ToList();
		}

		public static int Execute(string commandName, OnChatCommandReceivedArgs e)
		{
			List<BotCommand> allCommands = GetAll(commandName);
			int count = 0;
			foreach (BotCommand chatCommand in allCommands)
			{
				chatCommand.Execute(e);
				count++;
			}
			return count;
		}
	}
}
