using System;
using System.Linq;
using TwitchLib.Client.Models;

namespace BotCore
{
	public class UserInfo
	{
		public string userId;
		public string userName;
		public string displayName;
		public string color;
		public int showsWatched;
		public UserInfo()
		{

		}

		public static UserInfo FromChatMessage(TwitchLibMessage chatMessage, int showsWatched)
		{
			UserInfo userInfo = new UserInfo();
			userInfo.color = chatMessage.ColorHex;
			userInfo.displayName = chatMessage.DisplayName;
			userInfo.showsWatched = showsWatched;
			userInfo.userName = chatMessage.Username;
			userInfo.userId = chatMessage.UserId;
			return userInfo;
		}
	}
}