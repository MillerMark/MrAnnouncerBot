using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api.V5.Models.Users;
using TwitchLib.Client.Models;

namespace BotCore
{
	public class AllViewers
	{
		public const int ModeratorLevel = 999999;

		readonly TimeSpan fourHours = TimeSpan.FromHours(4);

		List<Viewer> viewers = new List<Viewer>();
		public AllViewers()
		{

		}

		public List<Viewer> Viewers { get => viewers; }


		async void CheckData()
		{
			if (viewers == null)
				return;

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
				Console.WriteLine($"{viewer.DisplayName} has {viewer.CoinsCollected} coins.");
			}
		}
		public void Load()
		{
			viewers = AppData.Load<List<Viewer>>("AllViewers.json");
			if (viewers == null)
				viewers = new List<Viewer>();
			CheckData();
		}

		public void Save()
		{
			if (viewers != null)
				AppData.Save("AllViewers.json", viewers);
		}

		Viewer CreateNewViewer(ChatMessage chatMessage)
		{
			Viewer viewer = new Viewer() { UserId = chatMessage.UserId, UserName = chatMessage.Username, DisplayName = chatMessage.DisplayName };
			return viewer;
		}

		public void OnMessageReceived(ChatMessage chatMessage)
		{
			Viewer existingUser = GetViewer(chatMessage);
			if (existingUser == null)
			{
				existingUser = CreateNewViewer(chatMessage);
				if (viewers != null)
					viewers.Add(existingUser);
			}

			if (!chatMessage.Message.StartsWith("!"))
				existingUser.NumberOfChatMessagesSent++;
		}

		public Viewer GetViewer(ChatMessage chatMessage)
		{
			if (viewers == null)
				return null;
			return viewers.FirstOrDefault(x => x.UserId == chatMessage.UserId);
		}

		public Viewer GetViewerByUserName(string userName)
		{
			if (viewers == null)
				return null;
			return viewers.FirstOrDefault(x => string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) == 0);
		}

		public Viewer GetViewerById(string userId)
		{
			if (viewers == null)
				return null;
			return viewers.FirstOrDefault(x => x.UserId == userId);
		}

		public int GetUserLevel(ChatMessage chatMessage)
		{
			// TODO: add a bonus for following...
			var bonus = 0;
			if (chatMessage.IsSubscriber)
				bonus += chatMessage.SubscribedMonthCount;

			if (chatMessage.IsModerator)
				bonus += 5;

			Viewer existingViewer = GetViewer(chatMessage);
			if (existingViewer == null)
				return bonus;
			if (existingViewer.UserName == "coderushed" || existingViewer.UserName == "cheese_minor" || existingViewer.UserName == "rorybeckercoderush")
				return ModeratorLevel;

			return existingViewer.GetLevel() + bonus;
		}

		public void UserLeft(string userName)
		{
		}

		async public void UserJoined(string userName)
		{
			try
			{
				User user = await Twitch.GetUser(userName);
				CheckViewer(user);
			}
			catch (Exception ex)
			{
				//Debugger.Break();
			}
		}

		async Task<Viewer> CreateNewViewerFromUserName(string userName)
		{
			Viewer viewer = new Viewer() { UserName = userName, UserId = await Twitch.GetUserId(userName) };
			return viewer;
		}

		Viewer CreateNewViewer(string id, string name, string displayName)
		{
			Viewer viewer = new Viewer() { UserName = name, UserId = id, DisplayName = displayName };
			return viewer;
		}

		async public void UpdateLiveViewers(string[] viewers)
		{
			if (viewers.Length == 0)
				return;
			try
			{
				var results = await Twitch.Api.V5.Users.GetUsersByNameAsync(viewers.ToList());
				var userList = results.Matches;

				foreach (User user in userList)
					CheckViewer(user);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error calling Twitch.Api.V5.Users.GetUsersByNameAsync: " + ex.Message);
			}
			
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