using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.IO.Compression;

namespace MrAnnouncerBot
{
	public partial class MrAnnouncerBot
	{
		public static readonly HttpClient httpClient = new HttpClient();

		Dictionary<string, DateTime> lastScenePlayTime = new Dictionary<string, DateTime>();
		Dictionary<string, DateTime> lastCategoryPlayTime = new Dictionary<string, DateTime>();
		AllViewers allViewers = new AllViewers();
		private const string STR_ChannelName = "CodeRushed";
		//private const string STR_TwitchUserName = "MrAnnouncerGuy";
		const string STR_GetChattersApi = "https://tmi.twitch.tv/group/user/coderushed/chatters";
		const string STR_Ellipsis = "...";
		const string STR_CodeRushedUserId = "237584851";

		private static List<SceneDto> scenes = new List<SceneDto>();
		private static List<RestrictedSceneDto> restrictedScenes = new List<RestrictedSceneDto>();
		private string activeSceneName;
		private Timer checkChatRoomTimer;
		private Timer autoSaveTimer;
		private readonly OBSWebsocket obsWebsocket = new OBSWebsocket();
		private ZorkGame zork;
		private Random random = new Random((int)DateTime.Now.Ticks);

		private bool useObs = true;
		HubConnection hubConnection;

		public MrAnnouncerBot()
		{
			CheckDocs();
			InitChatRoomTimer();
			LoadPersistentData();
			InitZork();
			new BotCommand("?", HandleQuestionCommand);
			new BotCommand("+", HandleLevelUp);
			new BotCommand("github", HandleGitHubCommand);
			new BotCommand("vscode", HandleVsCodeCommand);
			new BotCommand("suppressFanfare", HandleSuppressFanfareCommand);
			new BotCommand("crIssue", MarkCodeRushIssue);
			new BotCommand("crIssueStart", MarkCodeRushIssueStart);
			hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:44303/MrAnnouncerBotHub").Build();
			if (hubConnection != null)
			{
				//hubConnection.Closed += HubConnection_Closed;
				hubConnection.On<string, int>("AddCoins", AddCoins);
				hubConnection.On<string>("NeedToGetCoins", NeedToGetCoins);
				hubConnection.On<string>("ChangeScene", ChangeScene);
				// TODO: Check out benefits of stopping gracefully with a cancellation token.
				hubConnection.StartAsync();
			}
			lastFanfareDuration = 15;
		}

		void ChangeScene(string sceneName)
		{
			try
			{
				obsWebsocket.SetCurrentScene(sceneName);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		void NeedToGetCoins(string userId)
		{
			Viewer viewerById = allViewers.GetViewerById(userId);
			if (viewerById != null)
				hubConnection.InvokeAsync("UserHasCoins", userId, viewerById.CoinsCollected);
		}

		void AddCoins(string userID, int amount)
		{
			Viewer viewerById = allViewers.GetViewerById(userID);
			if (viewerById != null)
				viewerById.CoinsCollected += amount;
		}

		//private System.Threading.Tasks.Task HubConnection_Closed(Exception arg)
		//{
		//	
		//}

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
			int thirtySeconds = (int)TimeSpan.FromMinutes(0.5).TotalMilliseconds;

			checkChatRoomTimer = new Timer(CheckViewers, null, oneMinute, oneMinute);
			//autoSaveTimer = new Timer(AutoSaveViewers, null, fiveMinutes, fiveMinutes);
			autoSaveTimer = new Timer(AutoSaveViewers, null, thirtySeconds, thirtySeconds);
		}

		private void InitZork()
		{
			zork = new ZorkGame(Twitch.Client, STR_ChannelName);
			new BotCommand("zork", zork.HandleCommand);
		}

		private void LoadPersistentData()
		{
			scenes = CsvData.Get<SceneDto>(FileName.SceneData);
			fanfares = CsvData.Get<FanfareDto>(FileName.FanfareData);
			restrictedScenes = CsvData.Get<RestrictedSceneDto>(FileName.SceneRestrictions);
			try
			{
				allViewers.Load();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception loading allViewers data: " + ex.Message);
				Debugger.Break();
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
			Console.WriteLine($"Saving viewer data... {DateTime.Now:T}");
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

		Dictionary<string, DateTime> playedFanfares = new Dictionary<string, DateTime>();

		Queue<FanfareDto> fanfareQueue = new Queue<FanfareDto>();
		List<FanfareDto> fanfares = new List<FanfareDto>();
		DateTime lastFanfareActivated = DateTime.Now;
		double lastFanfareDuration;
		bool suppressingFanfare;
		string startTimeURL;
		DateTime issueStartTime;

		void HandleUserFanfare(ChatMessage chatMessage)
		{
			if (suppressingFanfare)
				return;

			FanfareDto fanfare = fanfares.FirstOrDefault(x => string.Compare(x.DisplayName, chatMessage.DisplayName, StringComparison.InvariantCultureIgnoreCase) == 0);

			if (fanfare != null)
			{
				PlayFanfare(fanfare, chatMessage);
			}
			else
				PlayBackloggedFanfare();
		}

		void PlayBackloggedFanfare()
		{
			if (fanfareQueue.Count == 0)
				return;
			FanfareDto fanfare = fanfareQueue.Peek();
			if (PlayFanfare(fanfare, null))
				fanfareQueue.Dequeue();
		}

		private bool PlayFanfare(FanfareDto fanfare, ChatMessage chatMessage)
		{
			if (fanfare == null)
				return false;

			if (playedFanfares.ContainsKey(fanfare.DisplayName) && playedFanfares[fanfare.DisplayName].DayOfYear == DateTime.Now.DayOfYear)
				return true;

			bool stillPlaying = DateTime.Now - lastFanfareActivated < TimeSpan.FromSeconds(lastFanfareDuration);
			bool suppressFanfareToday = chatMessage != null && chatMessage.Message != null && chatMessage.Message.StartsWith('[');

			if (suppressFanfareToday)
			{
				MarkFanfareAsPlayed(fanfare);
				return true;
			}

			if (chatMessage != null && chatMessage.Message.IndexOf('[') >= 0)
			{

			}

			if (stillPlaying || RestrictedSceneIsActive())
			{
				if (!fanfareQueue.Contains(fanfare))
					fanfareQueue.Enqueue(fanfare);
				return false;
			}

			lastFanfareActivated = DateTime.Now;
			lastFanfareDuration = fanfare.SecondsLong + 3;

			string sceneName = fanfare.DisplayName;

			if (fanfare.Count > 1)
			{
				string indexStr = (new Random().Next(fanfare.Count) + 1).ToString();
				sceneName += indexStr;
			}


			ActivatingSceneByName(sceneName, "Fanfare");
			try
			{
				hubConnection.InvokeAsync("SuppressVolume", fanfare.SecondsLong);
				obsWebsocket.SetCurrentScene(sceneName);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unable to play fanfare: " + sceneName);
			}
			if (fanfare.DisplayName == "SurlyDev")
				GreetSurlyDev();
			else
				GreetVip(fanfare.DisplayName);
			MarkFanfareAsPlayed(fanfare);
			return true;
		}

		private void GreetVip(string displayName)
		{
			switch (new Random().Next(41))
			{
				case 0:
					Chat($"@{displayName} is in the house!");
					break;
				case 1:
					Chat($"@{displayName} has arrived!");
					break;
				case 2:
					Chat($"Welcome @{displayName}!");
					break;
				case 3:
					Chat($"Everybody say Hi to @{displayName}!");
					break;
				case 4:
					Chat($"Greetings @{displayName}!");
					break;
				case 5:
					Chat($"Hello @{displayName}! So glad you are here!");
					break;
				case 6:
					Chat($"OMG! @{displayName} is here!");
					break;
				case 7:
					Chat($"A warm welcome to @{displayName}, who is NOW in the house!");
					break;
				case 8:
					Chat($"Hey @{displayName}! So happy you could make it!");
					break;
				case 9:
					Chat($"Look out kids! Here comes @{displayName}!");
					break;
				case 10:
					Chat($"Everybody stand back! @{displayName} is here!");
					break;
				case 11:
					Chat($"Yay! My favorite (@{displayName}) just got here!");
					break;
				case 12:
					Chat($"Fantastic! @{displayName} has entered the building.");
					break;
				case 13:
					Chat($"Hey! Say hello to our friend @{displayName}!");
					break;
				case 14:
					Chat($"Welcome! Welcome! Welcome! (talking to you, @{displayName})");
					break;
				case 15:
					Chat($"Good news, kids: @{displayName} is in the house!");
					break;
				case 16:
					Chat($"Thank God you're here, @{displayName}. Now we can finally write some decent code.");
					break;
				case 17:
					Chat($"Gentle folk of the chat room, I give you, the incredible, the amazing, the mind-blowingly fantastic, it's.... @{displayName}!!!!!");
					break;
				case 18:
					Chat($"Hey @{displayName}! How are you doing today?");
					break;
				case 19:
					Chat($"Everybody relax! @{displayName} is here. Everything's gonna be okay.");
					break;
				case 20:
					Chat($"We can finally calm down, people! @{displayName} is here!");
					break;
				case 21:
					Chat($"Wassup @{displayName}???");
					break;
				case 22:
					Chat($"I'm so psyched that @{displayName} is here. That means this show is gonna be awesome!");
					break;
				case 23:
					Chat($"Yo @{displayName}!");
					break;
				case 24:
					Chat($"Hey @{displayName}!");
					break;
				case 25:
					Chat($"Hi @{displayName}!");
					break;
				case 26:
					Chat($"Hello @{displayName}!");
					break;
				case 27:
					Chat($"Yo yo yo! It's @{displayName}!");
					break;
				case 28:
					Chat($"Yes! It's @{displayName}!");
					break;
				case 29:
					Chat($"It's @{displayName}!");
					break;
				case 30:
					Chat($"@{displayName}! Excellent.");
					break;
				case 31:
					Chat($"@{displayName}! Fantastic to have you here.");
					break;
				case 32:
					Chat($"@{displayName}! Yes! Exactly what we need right now.");
					break;
				case 33:
					Chat($"@{displayName}! Now the stream's gonna be awesome.");
					break;
				case 34:
					Chat($"@{displayName}! Buckle up, kids. This show is gonna rock now.");
					break;
				case 35:
					Chat($"Whoa kids! @{displayName} is in the house!");
					break;
				case 36:
					Chat($"Yo @{displayName}! How's it going?");
					break;
				case 37:
					Chat($"Hey @{displayName}! How are you doing?");
					break;
				case 38:
					Chat($"Hi @{displayName}! Glad you are here.");
					break;
				case 39:
					Chat($"Greetings @{displayName}! Great to see you.");
					break;
				case 40:
					Chat($"Greetings @{displayName}!");
					break;
			}
		}
		private void GreetSurlyDev()
		{
			switch (new Random().Next(11))
			{
				case 0:
					Chat($"Look out Mark! SurlyDev just showed up!");
					break;
				case 1:
					Chat($"Oh no! SurlyDev is here!");
					break;
				case 2:
					Chat($"Watch out, Mark! (SurlyDev is here)");
					break;
				case 3:
					Chat($"Look out, kids! SurlyDev is here!");
					break;
				case 4:
					Chat($"Everyone to the lifeboats (me first)! SurlyDev is here!");
					break;
				case 5:
					Chat($"Mark, you'd better clean up this code! SurlyDev is in the house!");
					break;
				case 6:
					Chat($"Break out the drone-swatter! SurlyDev just showed up!");
					break;
				case 7:
					Chat($"Drat! We almost made a show without him! But alas, no, SurlyDev just sauntered in!");
					break;
				case 8:
					Chat($"He does one PR on GitHub and he thinks he's an open-source contributor. SurlyDev fork off ;) Let's just hope he never clones himself.");
					break;
				case 9:
					Chat($"Drone, drone, drone. That's all SurlyDev ever does. Not talking about the game; I'm sayin' that guy never shuts his trap.");
					break;
				case 10:
					GreetVip("SurlyDev");
					break;
			}
		}

		private void MarkFanfareAsPlayed(FanfareDto fanfare)
		{
			if (playedFanfares.ContainsKey(fanfare.DisplayName))
				playedFanfares[fanfare.DisplayName] = DateTime.Now;
			else
				playedFanfares.Add(fanfare.DisplayName, DateTime.Now);
		}

		void MarkCodeRushIssue(OnChatCommandReceivedArgs obj)
		{
			if (obj.Command.ChatMessage.UserId != STR_CodeRushedUserId)
				return;

			bool attachLogFiles = false;
			bool attachSettingsFiles = false;
			bool sendPrz = false;
			bool sendAlex = false;
			bool sendPerf = false;
			bool sendAllDevs = false;
			string message = "";
			string backTrackStr = "";
			foreach (string arg in obj.Command.ArgumentsAsList)
			{
				if (arg == "-log")
					attachLogFiles = true;
				else if (arg == "-settings")
					attachSettingsFiles = true;
				else if (arg == "-prz")
					sendPrz = true;
				else if (arg == "-alex")
					sendAlex = true;
				else if (arg == "-perf")
					sendPerf = true;
				else if (arg == "-allDevs")
					sendAllDevs = true;
				else if (arg.StartsWith("-$"))
					backTrackStr = arg.Substring(1);
				else
					message = arg;
			}

			MarkCodeRushIssue(message, attachLogFiles, attachSettingsFiles, sendPrz, sendAlex, sendPerf, sendAllDevs, backTrackStr);
		}

		void CompressFiles(string[] files)
		{
			
		}

		private void TwitchClient_OnMessageReceived(object sender, OnMessageReceivedArgs e)
		{
			HandleUserFanfare(e.ChatMessage);
			allViewers.OnMessageReceived(e.ChatMessage);
		}

		async void MarkCodeRushIssue(string title, bool attachLogFiles, bool attachSettingsFiles, bool sendPrz, bool sendAlex, bool sendPerf, bool sendAllDevs, string backTrackStr)
		{
			string showStartURL;
			
			string durationStr = "";
			string errors = "";

			if (startTimeURL == null)
			{
				showStartURL = await GetActiveShowPointURL(backTrackStr);
			}
			else
			{  // We already marked a start time for this issue.
				TimeSpan timeSpan = DateTime.Now - issueStartTime;
				durationStr = $" (duration: {timeSpan.TotalMinutes:F} minutes)";
				showStartURL = startTimeURL;
				startTimeURL = null;
			}

			List<string> attachedFiles = new List<string>();

			if (attachLogFiles)
			{
				try
				{
					const string path = @"C:\Users\Mark Miller\AppData\Local\CodeRush\Logs\";
					string baseZipFileName = Path.GetFileNameWithoutExtension(showStartURL);
					baseZipFileName = "CodeRushLogFiles_" + baseZipFileName.Replace("?t=", "_");
					string fullPathToZipFile = Path.Combine(path, baseZipFileName + ".zip");
					using (var zip = ZipFile.Open(fullPathToZipFile, ZipArchiveMode.Create))
					{
						IEnumerable<string> logFiles = Directory.EnumerateFiles(@"C:\Users\Mark Miller\AppData\Local\CodeRush\Logs", "*.log");
						foreach (string file in logFiles)
						{
							try
							{
								// new FileStream("c:\test.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
								// What is going on here?
								zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
							}
							catch (Exception ex1)
							{
								try
								{
									string destFileName = Path.Combine(Path.GetDirectoryName(file), "MostRecent_" + Path.GetFileName(file));
									File.Copy(file, destFileName);
									zip.CreateEntryFromFile(destFileName, Path.GetFileName(destFileName), CompressionLevel.Optimal);
									File.Delete(destFileName);
								}
								catch (Exception ex2)
								{
									errors += $"\n\n Exception attached log file {file}: " + ex2.Message;
								}
								
							}
						}
					}
					attachedFiles.Add(fullPathToZipFile);
				}
				catch (Exception ex)
				{
					
				}
				// 
			}

			Email($"CodeRush Issue - {title}", $"{title}:\n{showStartURL} {durationStr}{errors}", attachedFiles, sendPrz, sendAlex, sendPerf, sendAllDevs);
		}

		public static void Email(string subject, string htmlBody, List<string> attachedFiles, bool sendPrz, bool sendAlex, bool sendPerf, bool sendAllDevs)
		{
			try
			{
				MailMessage message = new MailMessage();
				SmtpClient smtp = new SmtpClient();
				message.From = new MailAddress(Twitch.Configuration["Secrets:EmailFromAddress"]);
				message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailMark"]));
				message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailRory"]));
				if (sendPrz)
					message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailPrz"]));
				if (sendAlex)
					message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailAlex"]));
				if (sendPerf)
					message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailPerf"]));
				if (sendAllDevs)
					message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailAllDevs"]));
				message.Subject = subject;
				message.IsBodyHtml = true; //to make message body as html  
				message.Body = htmlBody;

				foreach (string attachedFile in attachedFiles)
				{
					message.Attachments.Add(new Attachment(attachedFile));
				}
				
				smtp.Port = 587;
				smtp.Host = Twitch.Configuration["Secrets:EmailHost"];
				smtp.EnableSsl = true;
				smtp.UseDefaultCredentials = false;
				smtp.Credentials = new NetworkCredential(Twitch.Configuration["Secrets:EmailUserName"], Twitch.Configuration["Secrets:EmailPassword"]);
				smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
				smtp.Send(message);
			}
			catch (Exception ex)
			{
				if (ex != null)
				{

				}
			}
		}

		private void ConnectToObs()
		{
			if (obsWebsocket.IsConnected) return;
			try
			{
				obsWebsocket.Connect(ObsHelper.WebSocketPort, Twitch.Configuration["Secrets:ObsPassword"]);  // Settings.Default.ObsPassword);
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

		private static string TruncateForTwitch(string msg)
		{
			const int maxLength = 410;//  500;
			if (msg.Length > maxLength)
				msg = msg.Substring(0, maxLength - STR_Ellipsis.Length) + STR_Ellipsis;
			return msg;
		}

		private void Chat(string msg)
		{
			Twitch.Chat(TruncateForTwitch(msg));
		}

		private void Whisper(string userName, string msg)
		{
			//Twitch.Whisper(userName, "yo");
			Twitch.Whisper(userName, TruncateForTwitch(msg));
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

		object GetLevelName(int userLevel)
		{
			if (userLevel == 0)
				return "padawan";
			if (userLevel == 1)
				return "wizardling";
			if (userLevel == 2)
				return "apprentice";
			if (userLevel == 3)
				return "student";
			if (userLevel == 4)
				return "magician";
			return "wizard";
		}

		string GetNeedToLevelUpMessage(SceneDto scene, string displayName, int userLevel)
		{
			string learnMore = "You can learn about botcasting levels here: https://github.com/MillerMark/MrAnnouncerBot";
			switch (RandomInt(4))
			{
				case 0:
					return $"{displayName}, that's a level {scene.Level} spell, but alas, you are a level {userLevel} {GetLevelName(userLevel)}. " + learnMore;
				case 1:
					return $"Unfortunately {displayName}, there's no way a level {userLevel} {GetLevelName(userLevel)} can botcast level {scene.Level} spell! " + learnMore;
				case 2:
					return $"{displayName}, you'll need to level-up to {scene.Level} before you botcast that spell! " + learnMore;
				default:
					return $"{displayName} that's a level {scene.Level} spell! You need to level-up first! " + learnMore;
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

		void ActivatingSceneByName(string name, string category)
		{
			DateTime now = DateTime.Now;

			if (!lastScenePlayTime.ContainsKey(name))
				lastScenePlayTime.Add(name, now);
			else
				lastScenePlayTime[name] = now;

			if (!lastCategoryPlayTime.ContainsKey(category))
				lastCategoryPlayTime.Add(category, now);
			else
				lastCategoryPlayTime[category] = now;
		}

		void ActivatingScene(SceneDto scene)
		{
			ActivatingSceneByName(scene.SceneName, scene.Category);
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
			if (scene.Level > userLevel)
			{
				Chat(GetNeedToLevelUpMessage(scene, displayName, userLevel));
				return;
			}
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
			if (RestrictedSceneIsActive())
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

		private bool RestrictedSceneIsActive()
		{
			return restrictedScenes.Any(x => x.SceneName == activeSceneName);
		}

		private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{
			var command = e.Command.CommandText;

			if (BotCommands.Execute(e.Command.CommandText, e) > 0)
				return;

			if (e.Command.ChatMessage.DisplayName == "CodeRushed")
			{
				if (e.Command.CommandText == "Reset" && e.Command.ArgumentsAsString == "Fanfare")
					playedFanfares.Clear();

				if (e.Command.CommandText == "Fanfare")
					PlayFanfare(fanfares.FirstOrDefault(x => x.DisplayName == e.Command.ArgumentsAsString), null);
			}

			var scene = GetScene(command);
			if (scene != null)
				ActivateSceneIfPermitted(scene, e.Command.ChatMessage.DisplayName, allViewers.GetUserLevel(e.Command.ChatMessage));
			//else
			//	Whisper(e.Command.ChatMessage.Username, GetWhatMessage() + " Command not recognized: " + e.Command.CommandText);
		}


		string QuotedIfSpace(string chatShortcut)
		{
			if (chatShortcut.IndexOf(' ') >= 0)
				return $"\"{chatShortcut}\"";
			else
				return chatShortcut;
		}

		void HandleQuestionCommand(OnChatCommandReceivedArgs obj)
		{
			int userLevel = allViewers.GetUserLevel(obj.Command.ChatMessage);

			List<string> accessibleScenes = scenes.Where(m => m.Level <= userLevel)
																						.Select(m => QuotedIfSpace(m.ChatShortcut))
																						.ToList();

			string sceneList = string.Join(", ", accessibleScenes);

			//Whisper(obj.Command.ChatMessage.Username, $"{obj.Command.ChatMessage.DisplayName}, your user level is: {userLevel}. You can say any of these: {sceneList}." );
			Chat($"{obj.Command.ChatMessage.DisplayName}, your user level is: {userLevel}. You can say any of these: {sceneList}.");
			Chat($"See https://github.com/MillerMark/MrAnnouncerBot/blob/master/README.md for more info.");
		}

		void HandleGitHubCommand(OnChatCommandReceivedArgs obj)
		{
			Chat($"Active Projects: ");
			Chat($"https://github.com/MillerMark/MrAnnouncerBot");
			Chat($"https://github.com/MillerMark/TimeLine");
		}

		void HandleVsCodeCommand(OnChatCommandReceivedArgs obj)
		{
			Chat($"Please vote here: ");
			Chat($"https://github.com/microsoft/vscode/issues/63791");
		}


		void HandleLevelUp(OnChatCommandReceivedArgs obj)
		{
			int userLevel = allViewers.GetUserLevel(obj.Command.ChatMessage);
			if (userLevel < AllViewers.ModeratorLevel)
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
				var scene = GetScene("levelup");
				if (scene != null)
					ActivateSceneIfPermitted(scene, "CodeRushed", AllViewers.ModeratorLevel);
			}
		}

		void CheckDocs()
		{
			if (ReadmeManager.NeedToGenerateNewReadme())
			{
				Console.WriteLine("Generating updated readme...");
				ReadmeManager.GenerateNewReadme();
			}
		}

		int GetNumberBefore(string workStr, string marker)
		{
			int markerIndex = workStr.IndexOf(marker);
			string numberStr = "";
			if (markerIndex == -1)
				return 0;

			int numberIndex = markerIndex - 1;

			while (numberIndex >= 0)
			{
				if (!char.IsDigit(workStr[numberIndex]))
					break;
				numberStr = workStr[numberIndex] + numberStr;
				numberIndex--;
			}
			if (string.IsNullOrWhiteSpace(numberStr))
				return 0;
			return int.Parse(numberStr);
		}
		async Task<string> GetActiveShowPointURL(string backTrackStr = "")
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("Client-ID", Twitch.CodeRushedBotApiClientId);
			string requestUri = $"https://api.twitch.tv/helix/videos?user_id={STR_CodeRushedUserId}";
			HttpResponseMessage response = await client.GetAsync(requestUri);
			string responseBody = await response.Content.ReadAsStringAsync();
			LiveStreamData<LiveShowData> liveShowData = JsonConvert.DeserializeObject<LiveStreamData<LiveShowData>>(responseBody);
			if (liveShowData != null && liveShowData.data.Count > 0)
			{
				LiveShowData showData = liveShowData.data[0];
				string timeNow = showData.duration;
				if (!string.IsNullOrWhiteSpace(backTrackStr))
				{
					int dollarIndex = backTrackStr.IndexOf("$");
					if (dollarIndex >= 0)
						backTrackStr = backTrackStr.Substring(dollarIndex + 1);
					int minutes = GetNumberBefore(backTrackStr, "m");
					int seconds = GetNumberBefore(backTrackStr, "s");

					int totalSecondsBack = minutes * 60 + seconds;

					int existingHours = GetNumberBefore(timeNow, "h");
					int existingMinutes = GetNumberBefore(timeNow, "m");
					int existingSeconds = GetNumberBefore(timeNow, "s");

					const int secondsPerHour = 3600;
					const int secondsPerMinute = 60;
					int totalExistingSeconds = existingSeconds + 
						existingMinutes * secondsPerMinute + 
						existingHours * secondsPerHour;

					int newTimeInSeconds = totalExistingSeconds - totalSecondsBack;

					int newHours = (int)Math.Floor((double)newTimeInSeconds / secondsPerHour);
					newTimeInSeconds -= newHours * secondsPerHour;
					int newMinutes = (int)Math.Floor((double)newTimeInSeconds / secondsPerMinute);
					newTimeInSeconds -= newMinutes * secondsPerMinute;
					int newSeconds = newTimeInSeconds;

					timeNow = $"{newHours}h{newMinutes}m{newSeconds}s";
				}
				return showData.url + "?t=" + timeNow;
			}
			return null;
		}

		async void MarkCodeRushIssueStart(OnChatCommandReceivedArgs obj)
		{
			startTimeURL = await GetActiveShowPointURL();
			issueStartTime = DateTime.Now;
		}

		void HandleSuppressFanfareCommand(OnChatCommandReceivedArgs obj)
		{
			suppressingFanfare = true;
			Chat("Fanfare is suppressed.");
		}
	}
}
