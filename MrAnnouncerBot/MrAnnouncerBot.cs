using CsvHelper;
using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace MrAnnouncerBot
{
	public class MrAnnouncerBot
	{
		// client
		// api
		// channel name 
		// oAuth token
		// twitch username

		const string STR_CodeRushed = "CodeRushed";
		OBSWebsocket obsWebsocket = new OBSWebsocket();
		static List<SceneDto> scenes = new List<SceneDto>();
		string activeSceneName;
		public TwitchClient TwitchClient { get; set; }
		public MrAnnouncerBot()
		{
			GetSceneData();
		}

		private static void GetSceneData()
		{
			var textReader = File.OpenText("Scene Data\\Mr. Announcer Guy - Scenes.csv");
			CsvReader csvReader = new CsvReader(textReader);
			scenes = csvReader.GetRecords<SceneDto>().ToList();
		}

		void TwitchClientLog(object sender, OnLogArgs e)
		{
			Console.WriteLine(e.Data);
		}

		void GetCredentials()
		{
			InitializeObsWebSocket();

			string oAuthToken = Properties.Settings.Default.TwitchBotOAuthToken;
			ConnectionCredentials connectionCredentials = new ConnectionCredentials("MrAnnouncerGuy", oAuthToken);
			TwitchClient = new TwitchClient();
			TwitchClient.Initialize(connectionCredentials, STR_CodeRushed);

			TwitchClient.OnLog += TwitchClientLog;
			TwitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
			TwitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;
			TwitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
		}

		void ConnectToObs()
		{
			if (!obsWebsocket.IsConnected)
			{
				try
				{
					obsWebsocket.Connect("ws://127.0.0.1:4444", Properties.Settings.Default.ObsPassword);
				}
				catch (AuthFailureException)
				{
					Console.WriteLine("Authentication failed.");
					return;
				}
				catch (ErrorResponseException ex)
				{
					Console.WriteLine("Connect failed. " + ex.Message);
					return;
				}
			}
		}

		void InitializeObsWebSocket()
		{
			obsWebsocket.Connected += ObsWebsocket_Connected;
			obsWebsocket.Disconnected += ObsWebsocket_Disconnected;

			obsWebsocket.SceneChanged += ObsWebsocket_SceneChanged;
			obsWebsocket.SceneCollectionChanged += ObsWebsocket_SceneCollectionChanged;
			obsWebsocket.ProfileChanged += ObsWebsocket_ProfileChanged;
			obsWebsocket.TransitionChanged += ObsWebsocket_TransitionChanged;
			obsWebsocket.TransitionDurationChanged += ObsWebsocket_TransitionDurationChanged;

			obsWebsocket.StreamingStateChanged += ObsWebsocket_StreamingStateChanged;
			obsWebsocket.RecordingStateChanged += ObsWebsocket_RecordingStateChanged;

			obsWebsocket.StreamStatus += ObsWebsocket_StreamStatus;

			ConnectToObs();
		}

		private void ObsWebsocket_StreamStatus(OBSWebsocket sender, StreamStatus status)
		{
			Console.WriteLine("ObsWebsocket_StreamStatus");
		}

		private void ObsWebsocket_RecordingStateChanged(OBSWebsocket sender, OutputState type)
		{
			Console.WriteLine("ObsWebsocket_RecordingStateChanged");
		}

		private void ObsWebsocket_StreamingStateChanged(OBSWebsocket sender, OutputState type)
		{
			Console.WriteLine("ObsWebsocket_StreamingStateChanged");
		}

		private void ObsWebsocket_TransitionDurationChanged(OBSWebsocket sender, int newDuration)
		{
			Console.WriteLine("ObsWebsocket_TransitionDurationChanged");
		}

		private void ObsWebsocket_TransitionChanged(OBSWebsocket sender, string newTransitionName)
		{
			Console.WriteLine("ObsWebsocket_TransitionChanged");
		}

		private void ObsWebsocket_ProfileChanged(object sender, EventArgs e)
		{
			Console.WriteLine("ObsWebsocket_ProfileChanged");
		}

		private void ObsWebsocket_SceneCollectionChanged(object sender, EventArgs e)
		{
			Console.WriteLine("ObsWebsocket_SceneCollectionChanged");
		}

		private void ObsWebsocket_SceneChanged(OBSWebsocket sender, string newSceneName)
		{
			activeSceneName = newSceneName;
			Console.WriteLine("Active Scene: " + activeSceneName);
		}

		private void ObsWebsocket_Disconnected(object sender, EventArgs e)
		{
			Console.WriteLine("ObsWebsocket_Disconnected");
		}

		private void ObsWebsocket_Connected(object sender, EventArgs e)
		{
			Console.WriteLine("ObsWebsocket_Connected");
		}

		SceneDto GetScene(string command)
		{
			SceneDto foundScene = scenes.FirstOrDefault(m => string.Compare(m.ChatShortcut, "!" + command, StringComparison.OrdinalIgnoreCase) == 0);
			if (foundScene != null)
				return foundScene;
			return scenes.FirstOrDefault(m => string.Compare(m.AlternateShortcut, "!" + command, StringComparison.OrdinalIgnoreCase) == 0);
		}

		void InvokeScene(SceneDto scene)
		{
			obsWebsocket.SetCurrentScene(scene.SceneName);
		}

		private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{
			string command = e.Command.CommandText;
			if (command == "zork")
			{
				Chat(e.Command.ChatMessage.DisplayName + " is standing in an open field west of a white house, with a boarded front door.");
				return;
			}
			SceneDto scene = GetScene(command);
			if (scene != null)
				InvokeScene(scene);
		}

		private void TwitchClient_OnMessageReceived(object sender, OnMessageReceivedArgs e)
		{
			// TODO: Consider killing this.
			
		}

		private void TwitchClient_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
		{
			Chat("Mr. Announcer Bot is in da House!");
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

		public void Disconnect()
		{
			obsWebsocket.Disconnect();
		}

	}
}
