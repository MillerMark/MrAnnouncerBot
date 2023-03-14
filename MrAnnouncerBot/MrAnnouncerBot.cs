using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Diagnostics;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;
using BotCore;
using CsvHelper;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.PubSub.Models.Responses.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using MrAnnouncerBot.Games.Zork;
using OBSWebsocketDotNet;
using TwitchLib.Client;
using TwitchLib.PubSub;
using OBSWebsocketDotNet.Types;
using SheetsPersist;

namespace MrAnnouncerBot
{
	public partial class MrAnnouncerBot
	{
		MySecureString mrAnnouncerGuyClientId;
		MySecureString mrAnnouncerGuyAccessToken;

		TwitchClient kidzCodeClient;
		List<Entry> log = new List<Entry>();
		public static readonly HttpClient httpClient = new HttpClient();

		Dictionary<string, DateTime> lastScenePlayTime = new Dictionary<string, DateTime>();
		Dictionary<string, DateTime> lastCategoryPlayTime = new Dictionary<string, DateTime>();
		AllViewers allViewers = new AllViewers();
		private const string STR_ChannelName = "CodeRushed";
		//private const string STR_TwitchUserName = "MrAnnouncerGuy";
		const string STR_GetChattersApi = "https://tmi.twitch.tv/group/user/coderushed/chatters";
		const string STR_Ellipsis = "...";
		const string STR_CodeRushedUserId = "237584851";

		private static List<SceneDto> scenes;
		private static List<RestrictedSceneDto> restrictedScenes;
		private static List<ChannelPointAction> channelPointActions;
		private string activeSceneName;
		private Timer checkChatRoomTimer;
		private Timer autoSaveTimer;
		private OBSWebsocket obsWebsocket = new OBSWebsocket();
		private ZorkGame zork;
		private Random random = new Random((int)DateTime.Now.Ticks);

		Timer reconnectObsClientTimer;

		private bool useObs = true;
		HubConnection hubConnection;

		public MrAnnouncerBot()
		{
			RegisterSpreadsheets();
			CheckDocs();
			InitChatRoomTimer();
			LoadPersistentData();
			InitZork();
			new BotCommand("?", HandleQuestionCommand);
			new BotCommand("reload", ReloadCommand);
			new BotCommand("help", HandleQuestionCommand);
			new BotCommand("commands", HandleQuestionCommand);
			new BotCommand("+", HandleLevelUp);
			new BotCommand("github", HandleGitHubCommand);
			new BotCommand("vscode", HandleVsCodeCommand);
			new BotCommand("suppressFanfare", HandleSuppressFanfareCommand);
			new BotCommand("crIssue", MarkCodeRushIssue);
			new BotCommand("crIssueStart", MarkCodeRushIssueStart);
			new BotCommand("discord", HandleDiscordCommand);
			new BotCommand("dh", HandleDragonHCommand);
			new BotCommand("dhn", HandleDragonHNewTimeCommand);
			new BotCommand("book*", HandleBookCommand);
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

			InitializeKidzCodeBot();
			mrAnnouncerGuyClientId = new MySecureString(Twitch.Configuration["Secrets:MrAnnouncerGuyTwitchClientId"]);
			mrAnnouncerGuyAccessToken = new MySecureString(Twitch.Configuration["Secrets:MrAnnouncerGuyTwitchAccessToken"]);
		}

		void ChangeScene(string sceneName)
		{
			try
			{
				obsWebsocket.SetCurrentProgramScene(sceneName);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				//Debugger.Break();
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
			autoSaveTimer = new Timer(AutoSaveViewers, null, fiveMinutes, fiveMinutes);
		}

		private void InitZork()
		{
			zork = new ZorkGame(Twitch.CodeRushedClient, STR_ChannelName);
			new BotCommand("zork", zork.HandleCommand);
		}

		private void LoadPersistentData()
		{
			fanfares = CsvData.Get<FanfareDto>(FileName.FanfareData);
			try
			{
				allViewers.Load();
				if (allViewers.Viewers.Count < 270)  // We have had at least 270 viewers tracked as of the writing of this bug check code.
				{
					Console.Beep();
					Console.BackgroundColor = ConsoleColor.DarkRed;
					Console.WriteLine("");
					Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
					Console.WriteLine("!!                                                             !!");
					Console.WriteLine("!!  Possible corruption detected in the AllViewers.json file!  !!");
					Console.WriteLine("!!                                                             !!");
					Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
					Console.WriteLine("");
					Console.WriteLine($"allViewers.Viewers.Count = {allViewers.Viewers.Count}");
					Console.WriteLine("");
				}
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
			HookupTwitchEvents(Twitch.CodeRushedClient);
			HookupPubSubEvents(Twitch.CodeRushedPubSub);
			HookupTwitchEvents(Twitch.DroneCommandsClient);
		}

		void HookupPubSubEvents(TwitchPubSub codeRushedPubSub)
		{
			codeRushedPubSub.OnPubSubServiceError += CodeRushedPubSub_OnPubSubServiceError;
			codeRushedPubSub.OnPubSubServiceClosed += CodeRushedPubSub_OnPubSubServiceClosed;
			codeRushedPubSub.OnPubSubServiceConnected += CodeRushedPubSub_OnPubSubServiceConnected;
			codeRushedPubSub.OnChannelPointsRewardRedeemed += CodeRushedPubSub_OnChannelPointsRewardRedeemed;
		}

		void UnhookPubSubEvents(TwitchPubSub codeRushedPubSub)
		{
			codeRushedPubSub.OnPubSubServiceError -= CodeRushedPubSub_OnPubSubServiceError;
			codeRushedPubSub.OnPubSubServiceClosed -= CodeRushedPubSub_OnPubSubServiceClosed;
			codeRushedPubSub.OnPubSubServiceConnected -= CodeRushedPubSub_OnPubSubServiceConnected;
			codeRushedPubSub.OnChannelPointsRewardRedeemed -= CodeRushedPubSub_OnChannelPointsRewardRedeemed;
		}

		private void CodeRushedPubSub_OnPubSubServiceConnected(object sender, EventArgs e)
		{

		}

		private void CodeRushedPubSub_OnPubSubServiceClosed(object sender, EventArgs e)
		{

		}

		private void CodeRushedPubSub_OnPubSubServiceError(object sender, TwitchLib.PubSub.Events.OnPubSubServiceErrorArgs e)
		{
			log.Add(new ErrorEntry() { Exception = e.Exception });
			// TODO: Reconnect on error?
			//System.Diagnostics.Debugger.Break();
		}

		void QueueSceneToPlay(string sceneToPlay)
		{
			obsWebsocket.SetCurrentProgramScene(sceneToPlay);
		}

		void SetState(string stateToSet)
		{

		}

		void ExecuteChannelPointAction(ChannelPointAction channelPointAction, User user)
		{
			if (channelPointAction == null)
				return;
			if (!string.IsNullOrWhiteSpace(channelPointAction.SceneToPlay))
				QueueSceneToPlay(channelPointAction.SceneToPlay);
			else if (!string.IsNullOrWhiteSpace(channelPointAction.StateToSet))
				SetState(channelPointAction.StateToSet);
		}

		private void CodeRushedPubSub_OnChannelPointsRewardRedeemed(object sender, TwitchLib.PubSub.Events.OnChannelPointsRewardRedeemedArgs e)
		{
			// e.RewardRedeemed.Redemption.Status??? "UNFULFILLED", "FULFILLED", "CANCELED"
			// We may be able to update the status with a call.
			//user.Id;
			//user.DisplayName
			string id = e.RewardRedeemed.Redemption.Reward.Id;
			string title = e.RewardRedeemed.Redemption.Reward.Title;
			ExecuteChannelPointAction(GetChannelPointAction(id, title), e.RewardRedeemed.Redemption.User);
		}

		ChannelPointAction GetChannelPointAction(string id, string title)
		{
			if (!string.IsNullOrWhiteSpace(id))
			{
				ChannelPointAction channelPointAction = ChannelPointActions.FirstOrDefault(x => x.ID == id);
				if (channelPointAction != null)
					return channelPointAction;
			}
			return ChannelPointActions.FirstOrDefault(x => string.Compare(x.Title, title, true) == 0);
		}

		void HookupTwitchEvents(TwitchClient client)
		{
			client.OnError += Client_OnError;
			client.OnJoinedChannel += TwitchClient_OnJoinedChannel;
			client.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
			client.OnMessageReceived += TwitchClient_OnMessageReceived;
			client.OnUserJoined += TwitchClient_OnUserJoined;
			client.OnUserLeft += TwitchClient_OnUserLeft;
			client.OnChannelStateChanged += Client_OnChannelStateChanged;
			client.OnDisconnected += Client_OnDisconnected;
			client.OnLog += Client_OnLog;
		}

		void UnHookTwitchEvents(TwitchClient client)
		{
			client.OnJoinedChannel -= TwitchClient_OnJoinedChannel;
			client.OnChatCommandReceived -= TwitchClient_OnChatCommandReceived;
			client.OnMessageReceived -= TwitchClient_OnMessageReceived;
			client.OnUserJoined -= TwitchClient_OnUserJoined;
			client.OnUserLeft -= TwitchClient_OnUserLeft;
			client.OnChannelStateChanged -= Client_OnChannelStateChanged;
			client.OnDisconnected -= Client_OnDisconnected;
			client.OnError -= Client_OnError;
			client.OnLog -= Client_OnLog;
		}

		private void Client_OnLog(object sender, OnLogArgs e)
		{
			log.Add(new LogEntry() { BotUsername = e.BotUsername, Data = e.Data, Time = e.DateTime });
		}

		private void Client_OnError(object sender, TwitchLib.Communication.Events.OnErrorEventArgs e)
		{
			log.Add(new ErrorEntry() { Exception = e.Exception, Time = DateTime.Now });
		}

		private void Client_OnDisconnected(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
		{
			log.Add(new MessageEntry() { Message = "ClientDisconnected", Time = DateTime.Now });
		}

		private void Client_OnChannelStateChanged(object sender, OnChannelStateChangedArgs e)
		{
			log.Add(new MessageEntry() { Message = "ChannelStateChanged: " + e.Channel, Time = DateTime.Now });
		}

		void AutoSaveViewers(object obj)
		{
			Console.WriteLine($"Saving allViewers data for {allViewers.Viewers.Count} viewers... {DateTime.Now:T}");
			allViewers.Save();
		}

		async void CheckViewers(object obj)
		{
			try
			{
				HttpResponseMessage response = await httpClient.PostAsync(STR_GetChattersApi, null);

				string responseString = await response.Content.ReadAsStringAsync();
				if (response.IsSuccessStatusCode)
				{
					if (responseString == null)
						return;

					LiveViewers liveViewers = JsonConvert.DeserializeObject<LiveViewers>(responseString);
					if (liveViewers != null)
						allViewers.UpdateLiveViewers(liveViewers.chatters.viewers);
				}
				else
				{
					// TODO: Respond to errors in responseString
					//System.Diagnostics.Debugger.Break();
				}


			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception in CheckViewers: " + ex.Message);
				//Debugger.Break();
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

		Queue<string> fanfareQueue = new Queue<string>();
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

			int userFanfareCount = GetFanfareCount(chatMessage.DisplayName);

			if (userFanfareCount > 0)
			{
				PlayFanfare(chatMessage.DisplayName, chatMessage.Message);
			}
			else
				PlayBackloggedFanfare();
		}

		private int GetFanfareCount(string displayName)
		{
			return fanfares.Where(x => string.Compare(x.DisplayName, displayName, StringComparison.InvariantCultureIgnoreCase) == 0).Count();
		}

		void PlayBackloggedFanfare()
		{

			if (fanfareQueue.Count == 0)
				return;

			string displayName = fanfareQueue.Peek();

			if (PlayFanfare(displayName))
				fanfareQueue.Dequeue();

		}

		private const string emptyString = "";
		const string STR_MarkSaysOrThinks = "!mark";
		const string STR_FredSaysOrThinks = "!fred";
		const string STR_CampbellSaysOrThinks = "!campbell";
		const string STR_RorySaysOrThinks = "!rory";
		const string STR_RichardSaysOrThinks = "!richard";

		private bool PlayFanfare(string displayName, string message = emptyString)
		{

			string fanfareKey = displayName.ToLower();
			if (playedFanfares.ContainsKey(fanfareKey) && playedFanfares[fanfareKey].DayOfYear == DateTime.Now.DayOfYear)
				return true;

			bool stillPlaying = DateTime.Now - lastFanfareActivated < TimeSpan.FromSeconds(lastFanfareDuration);
			bool suppressFanfareToday = message.StartsWith('[');

			if (suppressFanfareToday)
			{
				MarkFanfareAsPlayed(displayName);
				return true;
			}

			if (stillPlaying || RestrictedSceneIsActive())
			{
				if (!fanfareQueue.Contains(displayName))
					fanfareQueue.Enqueue(displayName);
				return false;
			}

			lastFanfareActivated = DateTime.Now;


			// Determine the Fanfare to be played
			FanfareDto fanfare = DetermineFanfareToPlay(displayName);

			if (fanfare != null && (DateTime.Now - fanfare.LastPlayed).TotalHours > 5)
			{
				string sceneName = fanfare.DisplayName;
				if (GetFanfareCount(fanfare.DisplayName) > 1)
					sceneName += fanfare.Index;

				lastFanfareDuration = fanfare.SecondsLong + 3;

				ActivatingSceneByName(sceneName, "Fanfare");
				try
				{
					hubConnection.InvokeAsync("SuppressVolume", fanfare.SecondsLong);
					obsWebsocket.SetCurrentProgramScene(sceneName);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Unable to play fanfare: " + sceneName);
					Debugger.Break();
				}

				MarkFanfareAsPlayed(fanfare);

				Chat(new VIPGreeting(displayName).Greeting);
			}

			return true;
		}

		FanfareDto DetermineFanfareToPlay(string displayName)
		{

			List<FanfareDto> userFanfares = fanfares.Where(x => string.Compare(x.DisplayName, displayName, StringComparison.InvariantCultureIgnoreCase) == 0).ToList();

			// Make sure none of the fanfares have been played today
			// Handles scenario where MrAnnouncerBot has been restarted mid stream
			if (userFanfares.Where(fanfare => (DateTime.Now - fanfare.LastPlayed).TotalHours > 5).Any())
			{

				// Get the list of Full Length fanfares 
				// that have not been played in the last week
				IEnumerable<FanfareDto> fanFaresToPlay = userFanfares.Where(fanfare => fanfare.Duration == FanfareDuration.fullLength)
				.Where(fanfare => (DateTime.Now - fanfare.LastPlayed).TotalHours > 5);

				// No full length fanfares to play.  Get the clipped fanfare
				if (!fanFaresToPlay.Any())
				{
					fanFaresToPlay = userFanfares.Where(_ => _.Duration == FanfareDuration.clipped);
				}


				// Select a random fanfare from the available list

				if (!fanFaresToPlay.Any())
				{
					return null;
				}
				else if (fanFaresToPlay.Count() == 1)
				{
					return fanFaresToPlay.First();
				}
				else
				{
					return fanFaresToPlay.ElementAt(new Random().Next(fanFaresToPlay.Count()));
				}
			}
			else
			{
				return null;
			}
		}

		static void WriteFanfareData(string dataFileName, List<FanfareDto> records)
		{
			using (var writer = new StreamWriter(dataFileName))
			using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
			{
				csv.WriteRecords(records);
			}
		}

		void MarkFanfareAsPlayed(FanfareDto fanfare)
		{

			FanfareDto updatedFanfare = fanfares.Where(_ => _.DisplayName == fanfare.DisplayName && _.Index == fanfare.Index && _.Duration == fanfare.Duration).First();

			updatedFanfare.LastPlayed = DateTime.Now;

			WriteFanfareData(FileName.FanfareData_Source, fanfares);

			MarkFanfareAsPlayed(fanfare.DisplayName);
		}

		void MarkFanfareAsPlayed(string DisplayName)
		{
			string fanfareKey = DisplayName.ToLower();
			if (playedFanfares.ContainsKey(fanfareKey))
				playedFanfares[fanfareKey] = DateTime.Now;
			else
				playedFanfares.Add(fanfareKey, DateTime.Now);
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
				try
				{
					showStartURL = await Twitch.GetActiveShowPointURL(mrAnnouncerGuyClientId, mrAnnouncerGuyAccessToken, STR_CodeRushedUserId, backTrackStr);
				}
				catch
				{
					showStartURL = startTimeURL;
					Debugger.Break();
				}
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
							catch
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

#pragma warning disable CS0168 // Used for diagnostics and debugging.
				catch (Exception ex)
				{
					Debugger.Break();
				}
				// 
			}

			string htmlBody = $"{title}:\n{showStartURL} {durationStr}{errors}\nEmail sent at {DateTime.Now.ToLongTimeString()}, local time.";
			Email($"CodeRush Issue - {title}", htmlBody, attachedFiles);
		}

		public static void Email(string subject, string htmlBody, List<string> attachedFiles)
		{
			try
			{
				MailMessage message = new MailMessage();
				SmtpClient smtp = new SmtpClient();
				//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				//`! !!!                                                                                      !!!
				//`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
				//`! !!!                                                                                      !!!
				//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

				message.From = new MailAddress(Twitch.Configuration["Secrets:EmailFromAddress"]);
				message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailMark"]));
				message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailRory"]));
				//if (sendPrz)
				//	message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailPrz"]));
				//if (sendAlex)
				//message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailAlex"]));
				//if (sendPerf)
				//	message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailPerf"]));
				//if (sendAllDevs)
				//	message.To.Add(new MailAddress(Twitch.Configuration["Secrets:EmailAllDevs"]));
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
					//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
					//`! !!!                                                                                      !!!
					//`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
					//`! !!!                                                                                      !!!
					//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

					Debugger.Break();
				}
			}
		}

		private void ConnectToObs()
		{
			if (obsWebsocket.IsConnected) return;
			try
			{
				//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				//`! !!!                                                                                      !!!
				//`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
				//`! !!!                                                                                      !!!
				//`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				obsWebsocket.ConnectAsync(ObsHelper.WebSocketPort, Twitch.Configuration["Secrets:ObsPassword"]);  // Settings.Default.ObsPassword);

			}
			catch (AuthFailureException)
			{
				Console.WriteLine("Authentication failed.");
				Debugger.Break();
			}
			catch (ErrorResponseException ex)
			{
				Console.WriteLine($"Connect failed. {ex.Message}");
				Debugger.Break();
			}
		}

		private void InitializeObsWebSocket()
		{
			HookObsEvents();

			ConnectToObs();
		}

		private void HookObsEvents()
		{
			obsWebsocket.Connected += ObsWebsocket_Connected;
			obsWebsocket.Disconnected += ObsWebsocket_Disconnected;
			obsWebsocket.CurrentProgramSceneChanged += ObsWebsocket_CurrentProgramSceneChanged;
			obsWebsocket.ProfileListChanged += ObsWebsocket_ProfileListChanged;
			obsWebsocket.StreamStateChanged += ObsWebsocket_StreamStateChanged;
		}

		private void UnhookObsEvents()
		{
			obsWebsocket.Connected -= ObsWebsocket_Connected;
			obsWebsocket.Disconnected -= ObsWebsocket_Disconnected;
			obsWebsocket.CurrentProgramSceneChanged -= ObsWebsocket_CurrentProgramSceneChanged;
			obsWebsocket.ProfileListChanged -= ObsWebsocket_ProfileListChanged;
			obsWebsocket.StreamStateChanged -= ObsWebsocket_StreamStateChanged;
		}

		private void ObsWebsocket_StreamStateChanged(object sender, OBSWebsocketDotNet.Types.Events.StreamStateChangedEventArgs e)
		{
			Console.WriteLine($"ObsWebsocket_ProfileChanged: {e.OutputState}");
		}

		private void ObsWebsocket_ProfileListChanged(object sender, OBSWebsocketDotNet.Types.Events.ProfileListChangedEventArgs e)
		{
			Console.WriteLine("ObsWebsocket_ProfileChanged");
		}

		private void ObsWebsocket_CurrentProgramSceneChanged(object sender, OBSWebsocketDotNet.Types.Events.ProgramSceneChangedEventArgs e)
		{
			activeSceneName = e.SceneName;
			if (activeSceneName == "EventReset")
			{
				Debugger.Break();

				UnhookPubSubEvents(Twitch.CodeRushedPubSub);
				UnHookTwitchEvents(Twitch.CodeRushedClient);
				UnHookTwitchEvents(Twitch.DroneCommandsClient);
				Twitch.InitializeConnections();
				HookupTwitchEvents(Twitch.DroneCommandsClient);
				HookupTwitchEvents(Twitch.CodeRushedClient);
				HookupPubSubEvents(Twitch.CodeRushedPubSub);
			}
			Console.WriteLine($"Active Scene: {activeSceneName}");
		}

		private void ObsWebsocket_Disconnected(object sender, OBSWebsocketDotNet.Communication.ObsDisconnectionInfo e)
		{
			UnhookObsEvents();
			obsWebsocket = null;
			reconnectObsClientTimer = new Timer(ReconnectObsClient, null, 250, Timeout.Infinite);
		}

		void ReconnectObsClient(object obj)
		{
			reconnectObsClientTimer = null;
			obsWebsocket = new OBSWebsocket();
			InitializeObsWebSocket();
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
				Debugger.Break();
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
			Twitch.Chat(Twitch.CodeRushedClient, TruncateForTwitch(msg));
		}

		public void Run()
		{
			Twitch.InitializeConnections();
			InitializeConnections();
		}

		private SceneDto GetScene(string command)
		{
			return useObs ? Scenes.FirstOrDefault(m => m.Matches(command)) : null;
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
			try
			{
				obsWebsocket.SetCurrentProgramScene(sceneName);
			}
			catch (Exception e)
			{
				Chat($"Sorry, I can't find that scene: {sceneName}");
			}
		}

		private void ActivateSceneIfPermitted(SceneDto scene, string displayName, int userLevel)
		{
			if (RestrictedSceneIsActive() && userLevel < AllViewers.ModeratorLevel)
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
			return RestrictedScenes.Any(x => x.SceneName == activeSceneName);
		}

		private const int minUserLevelForSpeechBubbles = 4;
		ProfanityFilter.ProfanityFilter profanityFilter;

		public static List<SceneDto> Scenes
		{
			get
			{
				if (scenes == null)
					scenes = GoogleSheets.Get<SceneDto>();
				return scenes;
			}
		}

		public static List<RestrictedSceneDto> RestrictedScenes
		{
			get
			{
				if (restrictedScenes == null)
				{
					restrictedScenes = GoogleSheets.Get<RestrictedSceneDto>();
				}
				return restrictedScenes;
			}
		}

		public static List<ChannelPointAction> ChannelPointActions
		{
			get
			{
				if (channelPointActions == null)
					channelPointActions = GoogleSheets.Get<ChannelPointAction>();
				return channelPointActions;
			}
		}

		async void SayIt(int playerId, string phrase)
		{
			string colorStr = ExtractColorStr(ref phrase);
			string offsetStr = ExtractOffsetStr(ref phrase);
			string quotedPhrase = phrase.Trim('"').Trim() + colorStr + offsetStr;
			await SafeHubInvokeAsync("SpeechBubble", $"{playerId} says: {quotedPhrase}");
		}

		private async Task SafeHubInvokeAsync(string methodName, string parameters)
		{
			if (hubConnection.State != HubConnectionState.Connected)
				await hubConnection.StartAsync();

			await hubConnection.InvokeAsync(methodName, parameters);
		}

		async void ThinkIt(ChatMessage chatMessage, int playerId, string phrase)
		{
			string colorStr = ExtractColorStr(ref phrase);
			string offsetStr = ExtractOffsetStr(ref phrase);

			string quotedPhrase = phrase.Trim().TrimStart('(').TrimEnd(')') + colorStr + offsetStr;
			await SafeHubInvokeAsync("SpeechBubble", $"{playerId} thinks: {quotedPhrase}");
		}

		private static string ExtractColorStr(ref string phrase)
		{
			string colorStr = string.Empty;
			int colorStrStart = phrase.IndexOf("(#");
			if (colorStrStart >= 0)
			{
				int colorStrStop = phrase.IndexOf(")", colorStrStart);
				if (colorStrStop > 0)
				{
					colorStr = phrase.Substring(colorStrStart, colorStrStop - colorStrStart + 1);
					string firstPart = phrase.Substring(0, colorStrStart);
					string secondPart = string.Empty;
					if (colorStrStop < phrase.Length - 1)
						secondPart = phrase.Substring(colorStrStop + 1);
					phrase = firstPart + secondPart;
				}
			}

			return colorStr;
		}

		private static string ExtractOffsetStr(ref string phrase)
		{
			string offsetStr = string.Empty;
			int offsetStrStart = phrase.IndexOf("(+");
			if (offsetStrStart < 0)
				offsetStrStart = phrase.IndexOf("(-");
			if (offsetStrStart >= 0)
			{
				int offsetStrStop = phrase.IndexOf(")", offsetStrStart);
				if (offsetStrStop > 0)
				{
					offsetStr = phrase.Substring(offsetStrStart, offsetStrStop - offsetStrStart + 1);
					string firstPart = phrase.Substring(0, offsetStrStart);
					string secondPart = string.Empty;
					if (offsetStrStop < phrase.Length - 1)
						secondPart = phrase.Substring(offsetStrStop + 1);
					phrase = firstPart + secondPart;
				}
			}

			return offsetStr;
		}

		void SayOrThinkIt(ChatMessage chatMessage)
		{
			//if (DateTime.Now.Hour > 16)
			//{
			//	Chat($"{chatMessage.Username}, this command is only available in the CodeRush chat room before 16:00 Central time.");
			//	return;
			//}

			if (allViewers.GetUserLevel(chatMessage) < minUserLevelForSpeechBubbles)
			{
				Chat($"{chatMessage.Username}, this command is only available for level {minUserLevelForSpeechBubbles} users and up.");
				return;
			}

			string msg = chatMessage.Message.Trim();
			GetNameAndPhrase(msg, out string name, out string phrase);

			string colorStr = "";
			int playerId;
			if (name == "mark")
			{
				playerId = 2;
				colorStr = "(#3600d1)";
			}
			else if (name == "fred" || name == "richard")
			{
				playerId = 4;
				colorStr = "(#284974)";
			}
			else if (name == "campbell")
				playerId = 5;
			else if (name == "rory")
			{
				playerId = 5;
				colorStr = "(#880000)";
			}
			else
				return;

			var censoredPhrase = CensorText(phrase);

			if (censoredPhrase.Contains("(#"))  // Already specifies a color?
				colorStr = "";

			if (phrase.StartsWith("("))
				ThinkIt(chatMessage, playerId, censoredPhrase + colorStr);
			else
				SayIt(playerId, censoredPhrase + colorStr);
		}

		private static void GetNameAndPhrase(string msg, out string name, out string phrase)
		{
			name = null;
			phrase = null;
			int breakPos;

			int colonPos = msg.IndexOf(':');
			if (colonPos < 0 || colonPos >= msg.Length - 1)
			{
				int spacePos = msg.IndexOf(' ');
				if (spacePos < 0 || spacePos >= msg.Length - 1)
					return;
				else
					breakPos = spacePos;
			}
			else
				breakPos = colonPos;
			name = msg.Substring(1, breakPos - 1).Trim().ToLower();
			phrase = msg.Substring(breakPos + 1).Trim();
		}

		bool Vocalizes(string lowerMessage, string prefix)
		{
			return lowerMessage.StartsWith(prefix) &&
				lowerMessage.Length > prefix.Length &&
				(lowerMessage[prefix.Length] == ':' || lowerMessage[prefix.Length] == ' ');
		}

		private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{
			var command = e.Command.CommandText;
			var lowerMessage = e.Command.ChatMessage.Message.ToLower();

			if (Vocalizes(lowerMessage, STR_MarkSaysOrThinks) || Vocalizes(lowerMessage, STR_FredSaysOrThinks) ||
				Vocalizes(lowerMessage, STR_CampbellSaysOrThinks) || Vocalizes(lowerMessage, STR_RorySaysOrThinks) ||
				Vocalizes(lowerMessage, STR_RichardSaysOrThinks))
			{
				SayOrThinkIt(e.Command.ChatMessage);
				return;
			}

			if (BotCommands.Execute(e.Command.CommandText, e) > 0)
				return;

			if (e.Command.ChatMessage.DisplayName == "CodeRushed")
			{
				if (e.Command.CommandText == "Reset" && e.Command.ArgumentsAsString == "Fanfare")
					ResetFanfares();

				if (e.Command.CommandText == "Fanfare")
				{
					string displayName = e.Command.ChatMessage.DisplayName;
					PlayFanfare(displayName);
				}
			}

			var scene = GetScene(command);
			if (scene != null)
				ActivateSceneIfPermitted(scene, e.Command.ChatMessage.DisplayName, allViewers.GetUserLevel(e.Command.ChatMessage));
			//else
			//	Whisper(e.Command.ChatMessage.Username, GetWhatMessage() + " Command not recognized: " + e.Command.CommandText);
		}

		void ResetFanfares()
		{
			playedFanfares.Clear();
			foreach (FanfareDto fanfareDto in fanfares)
			{
				fanfareDto.LastPlayed = DateTime.MinValue;
			}
			WriteFanfareData(FileName.FanfareData_Source, fanfares);
		}

		string QuotedIfSpace(string chatShortcut)
		{
			if (chatShortcut.IndexOf(' ') >= 0)
				return $"\"{chatShortcut}\"";
			else
				return chatShortcut;
		}

		void ReloadCommand(OnChatCommandReceivedArgs obj)
		{
			scenes = null;
			restrictedScenes = null;
			channelPointActions = null;
		}

		void HandleQuestionCommand(OnChatCommandReceivedArgs obj)
		{
			int userLevel = allViewers.GetUserLevel(obj.Command.ChatMessage);

			List<string> accessibleScenes = Scenes.Where(m => m.Level <= userLevel).Select(x => x.SceneName).ToList();

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

		void HandleDiscordCommand(OnChatCommandReceivedArgs obj)
		{
			Chat($"Join the CodeRush community on Discord: ");
			Chat($"https://discord.gg/B7WSz6Q");
		}

		void HandleDragonHCommand(OnChatCommandReceivedArgs obj)
		{
			Chat($"Live comedy Dungeons and Dragons with over-the-top special effects (built right here), every Wednesday and Sunday: ");
			Chat($"https://twitch.tv/DragonHumpers");
			Chat($"9p ET / 6p PT / 1a GMT / 11a AEST");
		}

		void HandleDragonHNewTimeCommand(OnChatCommandReceivedArgs obj)
		{
			Chat($"Special time for Dungeons and Dragons today/tonight: ");
			Chat($"https://twitch.tv/DragonHumpers");
			Chat($"11p ET / 8p PT / 3a GMT / 13a AEST");
		}

		void HandleVsCodeCommand(OnChatCommandReceivedArgs obj)
		{
			Chat($"Please vote here: ");
			Chat($"https://github.com/microsoft/vscode/issues/63791");
		}

		async void HandleBookCommand(OnChatCommandReceivedArgs obj)
		{
			ChatCommand chatCommand = obj.Command;
			string bookTitle = chatCommand.ArgumentsAsString;

			await SafeHubInvokeAsync("ShowBook", CensorText(bookTitle));
		}

		private string CensorText(string text)
		{
			CreateProfanityFilterIfNecessary();
			return profanityFilter.CensorString(text);
		}

		private void CreateProfanityFilterIfNecessary()
		{
			if (profanityFilter == null)
				profanityFilter = new ProfanityFilter.ProfanityFilter();
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

		async void MarkCodeRushIssueStart(OnChatCommandReceivedArgs obj)
		{
			startTimeURL = await Twitch.GetActiveShowPointURL(mrAnnouncerGuyClientId, mrAnnouncerGuyAccessToken, STR_CodeRushedUserId);
			issueStartTime = DateTime.Now;
		}

		void HandleSuppressFanfareCommand(OnChatCommandReceivedArgs obj)
		{
			suppressingFanfare = true;
			Chat("Fanfare is suppressed.");
		}

		void InitializeKidzCodeBot()
		{
			kidzCodeClient = Twitch.CreateNewClient("cheese_minor", "cheese_minor", "DragonHumpersDmOAuthToken");
			HookTwitchClientEvents();
		}

		private void HookTwitchClientEvents()
		{
			if (kidzCodeClient == null)
				return;
			kidzCodeClient.OnDisconnected += KidzCodeClient_OnDisconnected;
			kidzCodeClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
		}

		private void KidzCodeClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{
			throw new NotImplementedException();
		}

		private void KidzCodeClient_OnDisconnected(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
		{
			UnhookTwitchClientEvents();
			kidzCodeClient = null;
		}

		private void UnhookTwitchClientEvents()
		{
			if (kidzCodeClient == null)
				return;
			kidzCodeClient.OnDisconnected -= KidzCodeClient_OnDisconnected;
			kidzCodeClient.OnChatCommandReceived -= TwitchClient_OnChatCommandReceived;
		}

		void RegisterSpreadsheets()
		{
			GoogleSheets.RegisterDocumentID("Mr. Announcer Guy", "1s-j-4EF3KbI8ZH0nSj4G4a1ApNFPz_W5DK9A9JTyb3g");
		}
	}
}
