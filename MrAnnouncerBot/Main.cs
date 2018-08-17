using CsvHelper;
using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace MrAnnouncerBot
{
	class Program
	{
		static void Main(string[] args)
		{
			var mrAnnouncerBot = new MrAnnouncerBot();
			mrAnnouncerBot.Run();
			Console.ReadLine();
			// TODO: async, task, etc.
		}
	}

	public class Scene
	{
		public string ChatShortcut { get; set; }
		public string AlternateShortcut { get; set; }
		public string Category { get; set; }
		public string SceneName { get; set; }
		public string LevelStr { get; set; }
		public string MinSpanToSameStr { get; set; }
		public string MinSpanToAnyPreviousStr { get; set; }
		public string LimitToUser { get; set; }
		public Scene()
		{
			
		}
	}

	public class MrAnnouncerBot
	{
		// client
		// api
		// channel name 
		// oAuth token
		// twitch username

		const string STR_CodeRushed = "CodeRushed";
		OBSWebsocket obsWebsocket = new OBSWebsocket();
		static List<Scene> scenes = new List<Scene>();
		public TwitchClient TwitchClient { get; set; }
		public MrAnnouncerBot()
		{
			GetSceneData();
		}

		private static void GetSceneData()
		{
			var textReader = File.OpenText("Scene Data\\Mr. Announcer Guy - Scenes.csv");
			CsvReader csvReader = new CsvReader(textReader);
			scenes = csvReader.GetRecords<Scene>().ToList();
			foreach (Scene scene in scenes)
			{
				
			}
		}

		void TwitchClientLog(object sender, OnLogArgs e)
		{
			Console.WriteLine(e.Data);
		}

		void GetCredentials()
		{
			string oAuthToken = Properties.Settings.Default.TwitchBotOAuthToken;
			ConnectionCredentials connectionCredentials = new ConnectionCredentials("MrAnnouncerGuy", oAuthToken);
			TwitchClient = new TwitchClient();
			TwitchClient.Initialize(connectionCredentials, STR_CodeRushed);

			TwitchClient.OnLog += TwitchClientLog;
			TwitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
			TwitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;
			TwitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
		}

		Scene GetScene(string command)
		{
			Scene foundScene = scenes.FirstOrDefault(m => m.ChatShortcut == "!" + command);
			if (foundScene != null)
				return foundScene;
			return scenes.FirstOrDefault(m => m.AlternateShortcut == "!" + command);
		}

		void InvokeScene(Scene scene)
		{
			obsWebsocket.Connect("", "no password, suckahs!");
			// BLOCKED!
		}

		private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{
			string command = e.Command.CommandText;
			if (command == "zork")
			{
				Chat(e.Command.ChatMessage.DisplayName + " is standing in an open field west of a white house, with a boarded front door.");
				return;
			}
			Scene scene = GetScene(command);
			if (scene != null)
				InvokeScene(scene);
		}

		private void TwitchClient_OnMessageReceived(object sender, OnMessageReceivedArgs e)
		{
			// TODO: Consider killing this.
		}

		private void TwitchClient_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
		{
			Chat("Mr. Announcer Bot is in the House!");
		}

		private void Chat(string msg)
		{
			TwitchClient.SendMessage(STR_CodeRushed, msg);
		}

		public void Run()
		{
			// TODO: get credentials.
			GetCredentials();
			TwitchClient.Connect();
		}

	}
}
