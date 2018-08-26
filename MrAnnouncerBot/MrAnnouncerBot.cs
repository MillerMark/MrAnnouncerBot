using System.Net.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using MrAnnouncerBot.Games.Zork;
using OBSWebsocketDotNet;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using System.Threading;
using Newtonsoft.Json;
using TwitchLib.Api;
using System.Reflection;
using System.Text;
using BotCore;

namespace MrAnnouncerBot
{
	public partial class MrAnnouncerBot
	{
		public static readonly HttpClient httpClient = new HttpClient();

		Dictionary<string, DateTime> lastScenePlayTime = new Dictionary<string, DateTime>();
		Dictionary<string, DateTime> lastCategoryPlayTime = new Dictionary<string, DateTime>();
		AllViewers allViewers = new AllViewers();
		private const string STR_ChannelName = "CodeRushed";
		private const string STR_WebSocketPort = "ws://127.0.0.1:4444";
		private const string STR_TwitchUserName = "MrAnnouncerGuy";
		const string STR_GetChattersApi = "https://tmi.twitch.tv/group/user/coderushed/chatters";

		private static List<SceneDto> scenes = new List<SceneDto>();
		private static List<RestrictedSceneDto> restrictedScenes = new List<RestrictedSceneDto>();
		private string activeSceneName;
		private Timer checkChatRoomTimer;
		private Timer autoSaveTimer;
		private readonly OBSWebsocket obsWebsocket = new OBSWebsocket();
		private ZorkGame zork;
		private Random random = new Random((int)DateTime.Now.Ticks);

		private bool useObs = true;

		public MrAnnouncerBot()
		{
			CheckDocs();
			InitChatRoomTimer();
			LoadPersistentData();
			InitZork();
			new BotCommand("?", HandleQuestionCommand);
			new BotCommand("+", HandleLevelUp);

		}

		public void Disconnect()
		{
			Chat(GetExitMessage());
			Twitch.Disconnect();
			if (checkChatRoomTimer != null)
				checkChatRoomTimer.Dispose();
			if (autoSaveTimer != null)
				autoSaveTimer.Dispose();
			allViewers.Save();
			obsWebsocket.Disconnect();
		}

		void InitChatRoomTimer()
		{
			int oneMinute = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
			int fiveMinutes = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;

			checkChatRoomTimer = new Timer(CheckViewers, null, oneMinute, oneMinute);
			autoSaveTimer = new Timer(AutoSaveViewers, null, fiveMinutes, fiveMinutes);
		}

		private void InitZork()
		{
			zork = new ZorkGame(Twitch.Client, STR_ChannelName);
			new BotCommand("zork", zork.HandleCommand);
		}

		private void LoadPersistentData()
		{
			scenes = CsvData.Get<SceneDto>(FileName.SceneData);
			restrictedScenes = CsvData.Get<RestrictedSceneDto>(FileName.SceneRestrictions);
			try
			{
				allViewers.Load();
			}
			catch (Exception ex)
			{
				//Chat("Exception loading allViewers data: " + ex.Message);
			}
		}

		private void InitializeConnections()
		{
			if (useObs)
				InitializeObsWebSocket();
			HookupTwitchEvents();
		}

		void HookupTwitchEvents()
		{
			Twitch.Client.OnJoinedChannel += TwitchClient_OnJoinedChannel;
			Twitch.Client.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
			Twitch.Client.OnMessageReceived += TwitchClient_OnMessageReceived;
			Twitch.Client.OnUserJoined += TwitchClient_OnUserJoined;
			Twitch.Client.OnUserLeft += TwitchClient_OnUserLeft;
		}

		void AutoSaveViewers(object obj)
		{
			Console.WriteLine("Saving viewer data....");
			allViewers.Save();
		}

		async void CheckViewers(object obj)
		{
			try
			{
				var response = await httpClient.PostAsync(STR_GetChattersApi, null);
				var responseString = await response.Content.ReadAsStringAsync();
				if (responseString == null)
					return;

				LiveViewers liveViewers = JsonConvert.DeserializeObject<LiveViewers>(responseString);
				if (liveViewers != null)
					allViewers.UpdateLiveViewers(liveViewers.chatters.viewers);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception in CheckViewers: " + ex.Message);
			}
		}

		private void TwitchClient_OnUserLeft(object sender, OnUserLeftArgs e)
		{
			allViewers.UserLeft(e.Username);
		}

		private void TwitchClient_OnUserJoined(object sender, OnUserJoinedArgs e)
		{
			allViewers.UserJoined(e.Username);
		}

		private void TwitchClient_OnMessageReceived(object sender, OnMessageReceivedArgs e)
		{
			allViewers.OnMessageReceived(e.ChatMessage);
		}

		private void ConnectToObs()
		{
			if (obsWebsocket.IsConnected) return;
			try
			{
				obsWebsocket.Connect(STR_WebSocketPort, Twitch.Configuration["Secrets:ObsPassword"]);  // Settings.Default.ObsPassword);
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
			//Console.WriteLine("ObsWebsocket_StreamStatus");
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

		private void Whisper(string userName, string msg)
		{
			Twitch.Client.SendWhisper(userName, msg);
		}

		private void TwitchClient_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
		{
			try
			{
				Chat(GetEntranceMessage());
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception thrown in TwitchClient_OnJoinedChannel: " + ex);
			}
		}

		private void Chat(string msg)
		{
			if (msg.Length > 509)
				msg = msg.Substring(0, 509) + "...";
			Twitch.Chat(msg);
		}

		public void Run()
		{
			Twitch.InitializeConnections();
			InitializeConnections();
		}

		private SceneDto GetScene(string command)
		{
			return useObs ? scenes.FirstOrDefault(m => m.Matches(command)) : null;
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

		string GetWhatMessage()
		{
			switch (RandomInt(6))
			{
				case 0:
					return "Sorry?";
				case 1:
					return "Didn't get that.";
				case 2:
					return "Unknown command.";
				case 3:
					return "You talking to me?";
				case 4:
					return "That's not gonna work.";
				case 5:
					return "Nobody understands what you're saying.";
				default:
					return "I don't think so.";
			}
		}

		string GetEntranceMessage()
		{
			switch (RandomInt(6))
			{
				case 0:
					return "Mr. Announcer Bot is in da House!";
				case 1:
					return "Mr. Announcer Bot has arrived!";
				case 2:
					return "You called? Mr. Announcer Bot at your service!";
				case 3:
					return "Mr. Announcer Bot is here to take care of all your chatting needs!";
				case 4:
					return "LET'S DO THIS!!! (in the house).";
				case 5:
					return "Mr. Announcer Bot greets you: Good day!";
				default:
					return "Mr. Announcer Bot is ready to ROCK!!!";
			}
		}
		string GetExitMessage()
		{
			switch (RandomInt(6))
			{
				case 0:
					return "MrAnnouncerBot has left the building!";
				case 1:
					return "Mr. Announcer Bot has departed! (the chat room)";
				case 2:
					return "Mr. Announcer Bot is off to another PARTY!";
				case 3:
					return "Mr. Announcer Bot is gone! You're on your own!";
				case 4:
					return "I'm outta here!";
				case 5:
					return "Good day! Goodbye! And good luck!";
				default:
					return "Like Schrödinger's cat, am I in the box? Or am I out? Don't look!";
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
			if (adjustedMinutesToSame > minutesSinceLastSceneActivation && userLevel < 99)
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

		private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{
			var command = e.Command.CommandText;

			if (BotCommands.Execute(e.Command.CommandText, e) > 0)
				return;

			var scene = GetScene(command);
			if (scene != null)
				ActivateSceneIfPermitted(scene, e.Command.ChatMessage.DisplayName, allViewers.GetUserLevel(e.Command.ChatMessage));
			else
				Whisper(e.Command.ChatMessage.Username, GetWhatMessage() + " Command not recognized: " + e.Command.CommandText);
		}


		void HandleQuestionCommand(OnChatCommandReceivedArgs obj)
		{
			int userLevel = allViewers.GetUserLevel(obj.Command.ChatMessage);

			List<string> accessibleScenes = scenes.Where(m => m.Level <= userLevel)
																						.Select(m => { 
																							if (m.ChatShortcut.IndexOf(' ') >= 0)
																								return $"\"{m.ChatShortcut}\"";
																							else
																								return m.ChatShortcut;
																						}
																						).ToList();

			string sceneList = string.Join(", ", accessibleScenes);
			
			Chat($"@{obj.Command.ChatMessage.DisplayName}, your user level is: {userLevel}. You can say any of these: {sceneList}." );
			Chat($"See https://github.com/MillerMark/MrAnnouncerBot/blob/master/README.md for more info.");
		}

		void HandleLevelUp(OnChatCommandReceivedArgs obj)
		{
			int userLevel = allViewers.GetUserLevel(obj.Command.ChatMessage);
			if (userLevel < 99)
				return;

			if (obj.Command.ArgumentsAsString != null)
			{
				string userName = obj.Command.ArgumentsAsString.TrimStart('@');
				if (allViewers.LevelChange(userName, 1) is Viewer viewer)
				{
					int newUserLevel = viewer.GetLevel();
					Chat($"{userName} is now at level {newUserLevel}.");
				}
				else
					Chat($"{userName} not found.");
			}
		}

		void CheckDocs()
		{
			if (DocChecker.NeedToGenerateNewReadme())
			{
				Console.WriteLine("Generating updated readme...");
				DocChecker.GenerateNewReadme();
			}
		}
	}
}