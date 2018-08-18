using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using MrAnnouncerBot.Games.Zork;
using MrAnnouncerBot.Properties;
using OBSWebsocketDotNet;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace MrAnnouncerBot
{
	public partial class MrAnnouncerBot
	{
		private const string STR_ChannelName = "CodeRushed";
		private const string STR_WebSocketPort = "ws://127.0.0.1:4444";
		private const string STR_SceneDataFile = "Scene Data\\Mr. Announcer Guy - Scenes.csv";
		private const string STR_SceneRestrictions = "Scene Data\\Mr. Announcer Guy - Restrictions.csv";
		private const string STR_TwitchUserName = "MrAnnouncerGuy";

		private static List<SceneDto> scenes = new List<SceneDto>();
		private static List<string> restrictedScenes = new List<string>();
		private string activeSceneName;
		private readonly OBSWebsocket obsWebsocket = new OBSWebsocket();
		private ZorkGame zork;

		private bool logging = false;
		private bool useObs = true;
		private bool zorkEnabled = true;

		public MrAnnouncerBot()
		{
			InitTwitchClient();
			InitSceneData();
			InitZork();
		}

		private void InitTwitchClient()
		{
			TwitchClient = new TwitchClient();
		}

		private void InitZork()
		{
			zork = new ZorkGame(TwitchClient, STR_ChannelName);
		}

		public TwitchClient TwitchClient { get; set; }

		private void InitSceneData()
		{
			scenes = GetCsvData<SceneDto>(STR_SceneDataFile);
			var localRestrictedScenes = GetCsvData<RestrictedSceneDto>(STR_SceneRestrictions);
			restrictedScenes.Clear();
			foreach (RestrictedSceneDto restrictedSceneDto in localRestrictedScenes)
				restrictedScenes.Add(restrictedSceneDto.SceneName);
		}

		private List<T> GetCsvData<T>(string dataFileName)
		{
			List<T> result = new List<T>();
			try
			{
				var textReader = File.OpenText(dataFileName);

				using (var csvReader = new CsvReader(textReader))
					result = csvReader.GetRecords<T>().ToList();
			}
			catch (Exception ex)
			{
				Chat("Error reading scene data file (" + dataFileName + "): " + ex.Message);
			}
			return result;
		}

		private void TwitchClientLog(object sender, OnLogArgs e)
		{
			if (logging)
				Console.WriteLine(e.Data);
		}

		private void GetCredentials()
		{
			if (useObs)
				InitializeObsWebSocket();

			var oAuthToken = Settings.Default.TwitchBotOAuthToken;
			var connectionCredentials = new ConnectionCredentials(STR_TwitchUserName, oAuthToken);
			TwitchClient.Initialize(connectionCredentials, STR_ChannelName);

			TwitchClient.OnLog += TwitchClientLog;
			TwitchClient.OnConnectionError += TwitchClient_OnConnectionError;
			TwitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
			TwitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
		}

		private void TwitchClient_OnConnectionError(object sender, OnConnectionErrorArgs e)
		{
			Console.WriteLine(e.Error.Message);
		}

		private void ConnectToObs()
		{
			if (obsWebsocket.IsConnected) return;
			try
			{
				obsWebsocket.Connect(STR_WebSocketPort, Settings.Default.ObsPassword);
			}
			catch (AuthFailureException)
			{
				Console.WriteLine("Authentication failed.");
			}
			catch (ErrorResponseException ex)
			{
				Console.WriteLine($"Connect failed. {ex.Message}");
			}
		}

		private void InitializeObsWebSocket()
		{
			obsWebsocket.Connected += ObsWebsocket_Connected;
			obsWebsocket.Disconnected += ObsWebsocket_Disconnected;

			obsWebsocket.SceneChanged += ObsWebsocket_SceneChanged;
			obsWebsocket.ProfileChanged += ObsWebsocket_ProfileChanged;

			//obsWebsocket.SceneCollectionChanged += ObsWebsocket_SceneCollectionChanged;
			//obsWebsocket.TransitionChanged += ObsWebsocket_TransitionChanged;
			//obsWebsocket.TransitionDurationChanged += ObsWebsocket_TransitionDurationChanged;
			//obsWebsocket.StreamingStateChanged += ObsWebsocket_StreamingStateChanged;
			//obsWebsocket.RecordingStateChanged += ObsWebsocket_RecordingStateChanged;

			obsWebsocket.StreamStatus += ObsWebsocket_StreamStatus;

			ConnectToObs();
		}

		private void ObsWebsocket_StreamStatus(OBSWebsocket sender, StreamStatus status)
		{
			Console.WriteLine("ObsWebsocket_StreamStatus");
		}

		private void ObsWebsocket_ProfileChanged(object sender, EventArgs e)
		{
			Console.WriteLine("ObsWebsocket_ProfileChanged");
		}

		private void ObsWebsocket_SceneChanged(OBSWebsocket sender, string newSceneName)
		{
			activeSceneName = newSceneName;
			Console.WriteLine($"Active Scene: {activeSceneName}");
		}

		private void ObsWebsocket_Disconnected(object sender, EventArgs e)
		{
			Console.WriteLine("ObsWebsocket_Disconnected");
		}

		private void ObsWebsocket_Connected(object sender, EventArgs e)
		{
			Console.WriteLine("ObsWebsocket_Connected");
		}

		private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{
			var command = e.Command.CommandText;

			if (command == "zork" && zorkEnabled)
			{
				zork.HandleCommand(e);
				return;
			}
			else
			{
				Whisper(e.Command.ChatMessage.Username, "I beg your pardon?");
			}

			if (useObs)
			{
				var scene = GetScene(command);
				if (scene != null)
					InvokeScene(scene);
			}
		}

		private void Whisper(string userName, string msg)
		{
			TwitchClient.SendWhisper(userName, msg);
		}

		private void TwitchClient_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
		{
			Chat("Mr. Announcer Bot is in da House!");
		}

		private void Chat(string msg)
		{
			TwitchClient.SendMessage(STR_ChannelName, msg);
		}

		public void Run()
		{
			GetCredentials();
			TwitchClient.Connect();
		}

		public void Disconnect()
		{
			TwitchClient.Disconnect();
			obsWebsocket.Disconnect();
		}

		private SceneDto GetScene(string command)
		{
			return scenes.FirstOrDefault(m =>
					string.Compare(m.ChatShortcut, "!" + command, StringComparison.OrdinalIgnoreCase) == 0) 
					?? 
					scenes.FirstOrDefault(m =>
					string.Compare(m.AlternateShortcut, "!" + command, StringComparison.OrdinalIgnoreCase) == 0);
		}

		string SelectRandomScene(string sceneName)
		{
			var filter = sceneName;
			if (filter.EndsWith("*"))
				filter = filter.TrimEnd('*');
			var currentSceneCollection = obsWebsocket.ListScenes();
			List<string> foundNames = new List<string>();

			foundNames = currentSceneCollection.Where(x => x.Name.StartsWith(filter)).Select(x => x.Name).ToList();

			//foreach (var scene in currentSceneCollection)
			//{
			//	if (scene.Name.StartsWith(filter))
			//		foundNames.Add(scene.Name);
			//}

			if (foundNames.Count == 0)
				return null;

			int index = (int)(new Random().Next(foundNames.Count));
			return foundNames[index];
		}

		private void InvokeScene(SceneDto scene)
		{
			if (restrictedScenes.IndexOf(activeSceneName) >= 0)
			{
				// TODO: Make my bot a moderator.
				// Change up these messages to see if they appear more responsive.
				Chat("I'm on a break right now.");
				return;
			}

			string sceneName = scene.SceneName;
			if (sceneName.EndsWith("*"))
				sceneName = SelectRandomScene(sceneName);

			if (sceneName == null)
				return;
			obsWebsocket.SetCurrentScene(sceneName);
		}
	}
}