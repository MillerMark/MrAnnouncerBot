using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api.Models.v5.Users;
using TwitchLib.Client.Models;

namespace MrAnnouncerBot
{
	public class AllViewers
	{
		TimeSpan fourHours = TimeSpan.FromHours(4);

		List<Viewer> viewers = new List<Viewer>();
		public AllViewers()
		{

		}

		public List<Viewer> Viewers { get => viewers; }


		async void CheckData()
		{
			foreach (Viewer viewer in viewers)
			{
				if (string.IsNullOrEmpty(viewer.UserId) || string.IsNullOrEmpty(viewer.DisplayName))
				{
					User user = await Twitch.GetUser(viewer.UserName);
					if (user != null)
					{
						viewer.UserId = user.Id;
						viewer.DisplayName = user.DisplayName;
					}
				}
			}
		}
		public void Load()
		{
			viewers = AppData.Load<List<Viewer>>("AllViewers.json");
			CheckData();
		}

		public void Save()
		{
			AppData.Save("AllViewers.json", viewers);
		}

		Viewer CreateNewViewer(ChatMessage chatMessage)
		{
			Viewer viewer = new Viewer();
			viewer.UserId = chatMessage.UserId;
			viewer.UserName = chatMessage.Username;
			viewer.DisplayName = chatMessage.DisplayName;
			return viewer;
		}

		public void OnMessageReceived(ChatMessage chatMessage)
		{
			Viewer existingUser = GetViewer(chatMessage);
			if (existingUser == null)
			{
				existingUser = CreateNewViewer(chatMessage);
				viewers.Add(existingUser);
			}

			if (!chatMessage.Message.StartsWith("!"))
				existingUser.NumberOfChatMessagesSent++;
		}

		private Viewer GetViewer(ChatMessage chatMessage)
		{
			return viewers.FirstOrDefault(x => x.UserId == chatMessage.UserId);
		}

		private Viewer GetViewerByUserName(string userName)
		{
			return viewers.FirstOrDefault(x => string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) == 0);
		}

		private Viewer GetViewerById(string userId)
		{
			return viewers.FirstOrDefault(x => x.UserId == userId);
		}

		public int GetUserLevel(ChatMessage chatMessage)
		{
			var subscriberBonus = 0;
			if (chatMessage.IsSubscriber)
				subscriberBonus = chatMessage.SubscribedMonthCount;

			Viewer existingViewer = GetViewer(chatMessage);
			if (existingViewer == null)
				return subscriberBonus;
			if (existingViewer.UserName == "coderushed")
				return 99;

			return existingViewer.GetLevel() + subscriberBonus;
		}

		public void UserLeft(string userName)
		{
		}

		async public void UserJoined(string userName)
		{
			User user = await Twitch.GetUser(userName);
			CheckViewer(user);

		}

		async Task<Viewer> CreateNewViewerFromUserName(string userName)
		{
			Viewer viewer = new Viewer();
			viewer.UserName = userName;
			viewer.UserId = await Twitch.GetUserId(userName);
			return viewer;
		}

		Viewer CreateNewViewer(string id, string name, string displayName)
		{
			Viewer viewer = new Viewer();
			viewer.UserName = name;
			viewer.UserId = id;
			viewer.DisplayName = displayName;
			return viewer;
		}

		async public void UpdateLiveViewers(string[] viewers)
		{
			var results = await Twitch.Api.Users.v5.GetUsersByNameAsync(viewers.ToList());
			var userList = results.Matches;

			foreach (User user in userList)
				CheckViewer(user);
		}

		private void CheckViewer(User user)
		{
			if (user == null)
				return;
			Viewer viewer = GetViewerById(user.Id);
			if (viewer == null)
				viewer = CreateNewViewer(user.Id, user.Name, user.DisplayName);

			if (WatchingNewShow(viewer))
			{
				viewer.LastShowWatched = DateTime.Now;
				viewer.NumberOfShowsWatched++;
				Console.WriteLine($"{viewer.DisplayName} shows watched: {viewer.NumberOfShowsWatched}");
			}
			else
				viewer.LastShowWatched = DateTime.Now;
		}

		private bool WatchingNewShow(Viewer viewer)
		{
			return DateTime.Now - viewer.LastShowWatched > fourHours;
		}

		public Viewer LevelChange(string userName, int value)
		{
			Viewer viewer = GetViewerByUserName(userName);
			if (viewer != null)
				viewer.ModeratorOffset += value;
			return viewer;
		}
	}
}