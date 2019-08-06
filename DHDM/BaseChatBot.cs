using System;
using System.Linq;
using System.Collections.Generic;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class BaseChatBot
	{
		protected List<string> userIds = new List<string>();

		public BaseChatBot()
		{
		}

		public BaseChatBot(string userId): this()
		{
			ListenTo(userId);
		}

		public void ListenTo(string userId)
		{
			userIds.Add(userId);
		}

		public bool ListensTo(string userId)
		{
			if (userId == "163482168")
				return true;
			return userIds.Contains(userId);
		}

		public virtual void HandleMessage(ChatMessage chatMessage, TwitchClient twitchClient)
		{

		}
	}
}
