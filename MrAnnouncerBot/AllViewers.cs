using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			viewer.DisplayName = chatMessage.DisplayName;
			return viewer;
		}

		public void OnMessageReceived(ChatMessage chatMessage)
		{
			Viewer existingUser = viewers.FirstOrDefault(x => x.UserId == chatMessage.UserId);
			if (existingUser == null)
			{
				existingUser = CreateNewViewer(chatMessage);
				viewers.Add(existingUser);
			}

			existingUser.NumberOfChatMessagesSent++;
		}
	}
}