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
using System.Threading;

namespace MrAnnouncerBot
{
	public partial class MrAnnouncerBot
	{
		Dictionary<string, DateTime> lastScenePlayTime = new Dictionary<string, DateTime>();
		Dictionary<string, DateTime> lastCategoryPlayTime = new Dictionary<string, DateTime>();
		AllViewers allViewers = new AllViewers();
		private const string STR_ChannelName = "CodeRushed";
		private const string STR_WebSocketPort = "ws://127.0.0.1:4444";
		private const string STR_SceneDataFile = "Data\\Mr. Announcer Guy - Scenes.csv";
		private const string STR_SceneRestrictions = "Data\\Mr. Announcer Guy - Restrictions.csv";
		private const string STR_TwitchUserName = "MrAnnouncerGuy";

		private static List<SceneDto> scenes = new List<SceneDto>();
		private static List<RestrictedSceneDto> restrictedScenes = new List<RestrictedSceneDto>();
		private string activeSceneName;
		private Timer checkChatRoomTimer;
		private readonly OBSWebsocket obsWebsocket = new OBSWebsocket();
		private ZorkGame zork;
		private Random random = new Random((int)DateTime.Now.Ticks);

		private bool logging = false;
		private bool useObs = true;
		private bool zorkEnabled = true;
		private bool live;

		public MrAnnouncerBot()
		{
			InitChatRoomTimer();
			InitTwitchClient();
			LoadPersistentData();
			InitZork();
			live = true;
			
		}

		public void Disconnect()
		{
			try
			{
				allViewers.Save();
			}
			catch (Exception ex)
			{
				// TODO: Alert and offer to try again.
			}
			Chat("MrAnnouncerBot has left the building!");
			live = false;
			checkChatRoomTimer.Dispose();
			TwitchClient.Disconnect();
			obsWebsocket.Disconnect();
		}

		void InitChatRoomTimer()
		{
			checkChatRoomTimer = new Timer(CheckChatRoom, null, 0, 5000);
		}

		void CheckChatRoom(Object obj)
		{
			if (!live)
				return;

			//TwitchClient.SendMessage();
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

		private void LoadPersistentData()
		{
			scenes = GetCsvData<SceneDto>(STR_SceneDataFile);
			restrictedScenes = GetCsvData<RestrictedSceneDto>(STR_SceneRestrictions);
			try
			{
				allViewers.Load();
			}
			catch (Exception ex)
			{
				//Chat("Exception loading allViewers data: " + ex.Message);
			}
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
			TwitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;
		}

		private void TwitchClient_OnMessageReceived(object sender, OnMessageReceivedArgs e)
		{
			allViewers.OnMessageReceived(e.ChatMessage);
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
					ActivateSceneIfPermitted(scene, e.Command.ChatMessage.DisplayName, allViewers.GetUserLevel(e.Command.ChatMessage));
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

		private SceneDto GetScene(string command)
		{
			return scenes.FirstOrDefault(m => m.Matches(command));
		}

		string SelectRandomScene(string sceneName)
		{
			var filter = sceneName;
			if (filter.EndsWith("*"))
				filter = filter.TrimEnd('*');
			var currentSceneCollection = obsWebsocket.ListScenes();
			List<string> foundNames = new List<string>();

			foundNames = currentSceneCollection.Where(x => x.Name.StartsWith(filter)).Select(x => x.Name).ToList();

			if (foundNames.Count == 0)
				return null;

			int index = RandomInt(foundNames.Count);
			return foundNames[index];
		}

		private int RandomInt(int maxValue)
		{
			return random.Next(maxValue);
		}

		string GetBreakMessage()
		{
			switch (RandomInt(6))
			{
				case 0:
					return "I'm on a break right now.";
				case 1:
					return "On a break. Ask me later.";
				case 2:
					return "Consuming coffee. Back in a bit.";
				case 3:
					return "I'm sorry. What?";
				case 4:
					return "I say we let Mark talk.";
				case 5:
					return "Maybe later.";
				default:
					return "Gimme a sec...";
			}

		}
		TimeSpan GetTimeSinceLastSceneActivation(SceneDto scene)
		{
			if (lastScenePlayTime.ContainsKey(scene.SceneName))
				return DateTime.Now - lastScenePlayTime[scene.SceneName];
			return TimeSpan.MaxValue;
		}
		TimeSpan GetTimeSinceLastCategoryActivation(SceneDto scene)
		{
			if (lastCategoryPlayTime.ContainsKey(scene.Category))
				return DateTime.Now - lastCategoryPlayTime[scene.Category];
			return TimeSpan.MaxValue;
		}
		void ActivatingScene(SceneDto scene)
		{
			DateTime now = DateTime.Now;

			if (!lastScenePlayTime.ContainsKey(scene.SceneName))
				lastScenePlayTime.Add(scene.SceneName, now);
			else
				lastScenePlayTime[scene.SceneName] = now;

			if (!lastCategoryPlayTime.ContainsKey(scene.Category))
				lastCategoryPlayTime.Add(scene.Category, now);
			else
				lastCategoryPlayTime[scene.Category] = now;
		}

		double GetSpanWaitAdjust(int userLevel)
		{
			if (userLevel < 0)
				return 2;

			if (userLevel < 5)
				return 1;

			if (userLevel < 10)
				return 0.75;

			if (userLevel < 15)
				return 0.5;

			return 0.25;
		}

		void ActivateScene(SceneDto scene, string displayName, int userLevel)
		{
			string sceneName = GetSceneName(scene);
			if (sceneName == null)
				return;

			double minutesSinceLastSceneActivation = GetTimeSinceLastSceneActivation(scene).TotalMinutes;
			double minutesSinceLastCategoryActivation = GetTimeSinceLastCategoryActivation(scene).TotalMinutes;

			var adjustedMinutesToSame = GetSpanWaitAdjust(userLevel) * scene.MinMinutesToSame;
			if (adjustedMinutesToSame > minutesSinceLastSceneActivation)
			{
				double minutesToWait = scene.MinMinutesToSame - minutesSinceLastSceneActivation;
				Chat($"I already said that @{displayName}. You'll have to wait another {minutesToWait:0.#} minutes until I can say that again.");
				return;
			}
			ActivatingScene(scene);
			obsWebsocket.SetCurrentScene(sceneName);
		}

		private void ActivateSceneIfPermitted(SceneDto scene, string displayName, int userLevel)
		{
			if (restrictedSceneIsActive())
				Chat(GetBreakMessage());
			else
				ActivateScene(scene, displayName, userLevel);
		}

		private string GetSceneName(SceneDto scene)
		{
			string sceneName = scene.SceneName;
			if (sceneName.EndsWith("*"))
				sceneName = SelectRandomScene(sceneName);
			return sceneName;
		}

		private bool restrictedSceneIsActive()
		{
			return restrictedScenes.Any(x => x.SceneName == activeSceneName);
		}
	}
}