using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Events;

namespace BotCore
{
	public class BotCommand
	{
		public bool Enabled { get; set; }
		public string Command { get; set; }
		public Action<OnChatCommandReceivedArgs> Action { get; set; }
		public BotCommand(string command, Action<OnChatCommandReceivedArgs> action, bool enabled)
		{
			Enabled = enabled;
			Init(command, action);
		}

		public BotCommand(string command, Action<OnChatCommandReceivedArgs> action)
		{
			Enabled = true;
			Init(command, action);
		}

		private void Init(string command, Action<OnChatCommandReceivedArgs> action)
		{
			Command = command;
			Action = action;
			BotCommands.Register(this);
		}

		public void Execute(OnChatCommandReceivedArgs e)
		{
			try
			{
				Action(e);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception executing command \"{e.Command.CommandText}\": " + ex.Message);
			}
		}
	}
}
