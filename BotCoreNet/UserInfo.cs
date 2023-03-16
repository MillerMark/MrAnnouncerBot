using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using TwitchLib.Client.Models;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace BotCore
{
	public class UserInfo
	{
		public string userId;
		public string userName;
		public string displayName;
		public string color;
		public int showsWatched;
		public string profileImageUrl;

		public UserInfo()
		{

		}

		static Dictionary<string, string> profileImageCache = new Dictionary<string, string>();

		public static async Task<UserInfo> FromChatMessage(TwitchLibMessage chatMessage, int showsWatched)
		{
			UserInfo userInfo = new UserInfo();
			string profileImageUrl = await GetProfileImageUrl(chatMessage);
			userInfo.color = chatMessage.ColorHex;
			userInfo.displayName = chatMessage.DisplayName;
			userInfo.showsWatched = showsWatched;
			userInfo.userName = chatMessage.Username;
			userInfo.userId = chatMessage.UserId;
			userInfo.profileImageUrl = profileImageUrl;

			return userInfo;
		}

		private static async Task<string> GetProfileImageUrl(TwitchLibMessage chatMessage)
		{
			if (profileImageCache.ContainsKey(chatMessage.UserId))
				return profileImageCache[chatMessage.UserId];

			List<string> userIds = new List<string>() { chatMessage.UserId };
			
			GetUsersResponse usersAsync = await Twitch.Api.Helix.Users.GetUsersAsync(userIds);
			User firstOrDefault = usersAsync.Users.FirstOrDefault();
			if (firstOrDefault == null)
				return null;
			
			string profileImageUrl = firstOrDefault.ProfileImageUrl;

			profileImageCache[chatMessage.UserId] = profileImageUrl;

			return profileImageUrl;
		}
	}
}