using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace MrAnnouncerBot
{
	public class AllViewers
	{
		List<Viewer> viewers = new List<Viewer>();
		public AllViewers()
		{

		}

		public List<Viewer> Viewers { get => viewers; }

		string GetDataFolder()
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MrAnnouncerBot");
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}

		public void Load()
		{
			viewers = JsonConvert.DeserializeObject<List<Viewer>>(File.ReadAllText(GetDataFileName()));
		}

		public void Save()
		{
			File.WriteAllText(GetDataFileName(), JsonConvert.SerializeObject(viewers));
		}

		private string GetDataFileName()
		{
			return Path.Combine(GetDataFolder(), "AllViewers.json");
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

			existingUser.NumberOfChatMessagesSent++;
		}

		private Viewer GetViewer(ChatMessage chatMessage)
		{
			return viewers.FirstOrDefault(x => x.UserId == chatMessage.UserId);
		}

		private Viewer GetViewer(string userName)
		{
			return viewers.FirstOrDefault(x => string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) == 0);
		}

		public int GetUserLevel(ChatMessage chatMessage)
		{
			var subscriberBonus = 0;
			if (chatMessage.IsSubscriber)
				subscriberBonus = chatMessage.SubscribedMonthCount;

			Viewer existingViewer = GetViewer(chatMessage);
			if (existingViewer == null)
				return subscriberBonus;


			return existingViewer.GetLevel() + subscriberBonus;
		}
		public void UserLeft(string username)
		{

		}
		public void UserJoined(string username)
		{
			//Viewer existingViewer = viewers.FirstOrDefault(x => x.UserName == username);
			//if (existingViewer == null)
			//{
			//	existingUser = CreateNewViewer(username);
			//}

			//if (DateTime.Now)
			//	existingViewer.LastShowWatched = ;
		}

		async Task<Viewer> CreateNewViewerFromUserName(string userName)
		{
			var response = await MrAnnouncerBot.httpClient.GetAsync("https://api.twitch.tv/helix/users");
			var responseString = await response.Content.ReadAsStringAsync();
			if (responseString == null)
				return null;
			Viewer viewer = new Viewer();
			viewer.UserName = userName;
			return viewer;
		}

		async public void UpdateLiveViewers(string[] viewers)
		{
			foreach (string userName in viewers)
			{
				Viewer viewer = GetViewer(userName);
				if (viewer == null)
					viewer = await CreateNewViewerFromUserName(userName);

				if (viewer != null)
				{
					if (DateTime.Now - viewer.LastShowWatched > TimeSpan.FromHours(4))
					{
						viewer.LastShowWatched = DateTime.Now;
						viewer.NumberOfShowsWatched++;
						Console.WriteLine($"{viewer.DisplayName} shows watched: {viewer.NumberOfShowsWatched}");
					}
					else
						viewer.LastShowWatched = DateTime.Now;
				}
			}
		}
	}
}