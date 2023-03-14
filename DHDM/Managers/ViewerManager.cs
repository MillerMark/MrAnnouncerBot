//#define profiling
using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;
using OBSWebsocketDotNet.Types;
using ObsControl;

namespace DHDM
{
	public class ViewerManager
	{
		const int INT_MinSecondsBetweenNormalGreetings = 5;
		const double DBL_HoursUntilWeStopPromotingCards = 1.75;
		DateTime streamStartTime = DateTime.MinValue;
		bool startedStreaming;
		readonly IDungeonMasterApp dungeonMasterApp;
		Timer messageTimer;

		object usersSeenSinceLastStreamlootsMessageLockObject = new object();
		List<string> usersSeenSinceLastStreamlootsMessage = new List<string>();

		public ViewerManager(IDungeonMasterApp dungeonMasterApp)
    {
      this.dungeonMasterApp = dungeonMasterApp;
			ObsManager.SceneChanged += ObsManager_SceneChanged;
			ObsManager.StateChanged += ObsManager_StateChanged;
			messageTimer = new Timer();
			messageTimer.Interval = TimeSpan.FromSeconds(90).TotalMilliseconds;
			messageTimer.Elapsed += MessageTimer_Elapsed;
		}

		private void MessageTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			TimeSpan timeSinceStart = DateTime.Now - streamStartTime;
			if (timeSinceStart < TimeSpan.FromHours(DBL_HoursUntilWeStopPromotingCards))
				ShowFollowUpMessage();
			else  // After 1.75 hours, we stop the streamloots messages.
				messageTimer.Stop();
		}

		private void ShowFollowUpMessage()
		{
			lock (usersSeenSinceLastStreamlootsMessageLockObject)
			{
				if (usersSeenSinceLastStreamlootsMessage.Count == 0)
					return;

				string userList = string.Empty;
				for (int i = 0; i < usersSeenSinceLastStreamlootsMessage.Count; i++)
				{
					string userName = usersSeenSinceLastStreamlootsMessage[i];
					if (userList == string.Empty)
						userList = userName;
					else if (i == usersSeenSinceLastStreamlootsMessage.Count - 1)
						userList += ", and " + userName;
					else
						userList += ", " + userName;
				}

				usersSeenSinceLastStreamlootsMessage.Clear();
				if (string.IsNullOrWhiteSpace(userList))
					return;

				dungeonMasterApp.TellViewers(GetCardsMessage(userList));
			}
		}

		private void ObsManager_StateChanged(object sender, OutputState e)
		{
			if (e == OutputState.OBS_WEBSOCKET_OUTPUT_STARTED)
			{
				StartStreaming();
			}
			else if (e == OutputState.OBS_WEBSOCKET_OUTPUT_STOPPED)
			{
				StopStreaming();
			}
		}

		private void StopStreaming()
		{
			AllViewers.Save();
			startedStreaming = false;
		}

		private void StartStreaming()
		{
			startedStreaming = true;
			streamStartTime = DateTime.Now;
			messageTimer.Start();
		}

		public string StreamTimeCode()
		{
			if (!startedStreaming || streamStartTime == DateTime.MinValue)
				return "Stream has not started yet.";
			TimeSpan timeSpan = DateTime.Now - streamStartTime;
			return $"{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
		}

		private void ObsManager_SceneChanged(object sender, string sceneName)
		{

		}

		Random random = new Random();

		string GetVipGreeting(string username)
		{
			switch (random.Next(9))
			{
				case 0:
					return $"Hey look who's back! It's {username}! So glad you could make it!";
				case 1:
					return $"Hey everyone! {username} is back!";
				case 2:
					return $"Hey Humpers! {username} is in the house!";
				case 3:
					return $"And here they are, please welcome, the amazing and powerful {username}!";
				case 4:
					return $"Make way kids! Beautiful people arriving! It's {username}!";
				case 5:
					return $"Here comes the great and powerful {username}! Welcome!!!";
				case 6:
					return $"Hey look! It's {username}!";
				case 7:
					return $"Look out! {username} just overflowed the awesome in this chat!";
				case 8:
					return $"Key kids, it's {username}! Great to see you! Welcome, welcome, welcome!";
				default:
					return $"OMG - {username} is here!"; ;
			}
		}

		string GetNormalGreeting(string username)
		{
			switch (random.Next(9))
			{
				case 0:
					return $"Hey {username}.";
				case 1:
					return $"Hi {username}.";
				case 2:
					return $"Welcome {username}.";
				case 3:
					return $"Yo yo yo - it's {username}.";
				case 4:
					return $"Greets {username}.";
				case 5:
					return $"Yo {username}.";
				case 6:
					return $"{username}, welcome.";
				case 7:
					return $"{username}, greetings.";
				case 8:
					return $"Greetings {username}.";
				default:
					return $"Tip of the hat to you, {username}.";
			}
		}

		string GetCardsMessage(string userList)
		{
			switch (random.Next(15))
			{
				case 0:
					return $"{userList}: Play cards in this game, available at streamloots.com/DragonHumpers";
				case 1:
					return $"{userList}: Play cards and change the game: streamloots.com/DragonHumpers";
				case 2:
					return $"{userList}: Playing cards available here: streamloots.com/DragonHumpers";
				case 3:
					return $"{userList}: Get your playing cards here: streamloots.com/DragonHumpers";
				case 4:
					return $"{userList}: Want to play with us? -> -> -> streamloots.com/DragonHumpers";
				case 5:
					return $"{userList}: Keep the dream alive! -> -> -> streamloots.com/DragonHumpers";
				case 6:
					return $"{userList}: Support the players and play with us live! streamloots.com/DragonHumpers";
				case 7:
					return $"{userList}: Play cards to change this game, available at streamloots.com/DragonHumpers";
				case 8:
					return $"Hey {userList}, did you know you can play cards and change the game? -> -> -> streamloots.com/DragonHumpers";
				case 9:
					return $"Hey {userList}, get your playing cards here: streamloots.com/DragonHumpers";
				case 10:
					return $"Hey {userList}, want to hump dragons with us? Get your playing cards here: streamloots.com/DragonHumpers";
				case 11:
					return $"Yo {userList}! You can play with us live (by buying and playing cards) -> -> -> streamloots.com/DragonHumpers";
				case 12:
					return $"Yo {userList}! Help us keep the dream alive! -> -> -> streamloots.com/DragonHumpers";
				case 13:
					return $"{userList} -> Support the players and join us live! streamloots.com/DragonHumpers";
				default:
					return $"{userList}: The players need your help! -> -> -> streamloots.com/DragonHumpers";
			}
		}

		void VipGreeting(string username)
		{
			dungeonMasterApp.TellViewers(GetVipGreeting(username));
		}

		DateTime lastNormalGreeting = DateTime.MinValue;

		void NormalGreeting(string username)
		{
			lock (usersSeenSinceLastStreamlootsMessageLockObject)
			{
				if (!usersSeenSinceLastStreamlootsMessage.Contains(username))
					usersSeenSinceLastStreamlootsMessage.Add(username);
			}

			TimeSpan timeSpanSinceLastNormalGreeting = DateTime.Now - lastNormalGreeting;
			if (timeSpanSinceLastNormalGreeting < TimeSpan.FromSeconds(INT_MinSecondsBetweenNormalGreetings))
				return;

			dungeonMasterApp.TellViewers(GetNormalGreeting(username));
			lastNormalGreeting = DateTime.Now;
		}

		string GetRemainingChargeMessage(string username, string chargeList, bool plural)
		{
			switch (random.Next(9))
			{
				case 0:
					if (plural)
						return $"Hey {username}, you have remaining charges from a card play in a previous game: {chargeList}";
					else
						return $"Hey {username}, you have one remaining charge from a card play in a previous game: {chargeList}";
				case 1:
					if (plural)
						return $"{username}, did you know you have remaining charges from a previous game? {chargeList}";
					else
						return $"{username}, did you know you have a remaining charge from a previous game? {chargeList}";
				case 2:
					if (plural)
						return $"{username}, you still have these from a previous game: {chargeList}";
					else
						return $"{username}, you still have this from a previous game: {chargeList}";
				default:
					if (plural)
						return $"{username}, these charges (from a previous game) are still available: {chargeList}";
					else
						return $"{username}, this charge (from a previous game) is still available: {chargeList}";
			}
		}

		void TellViewerAboutRemainingCharges(DndViewer viewer)
		{
			string result = string.Empty;
			int chargeCount = 0;
			foreach (string key in viewer.charges.Keys)
			{
				if (viewer.charges[key] > 0)
				{
					chargeCount += viewer.charges[key];
					result += $"{key} ({viewer.charges[key]}), ";
				}
			}
			result = result.TrimEnd(' ', ',');

			if (chargeCount > 0)
				dungeonMasterApp.TellViewers(GetRemainingChargeMessage(viewer.UserName, result, chargeCount > 1));
		}

		public void UserChats(string username)
		{
			if (username == "dragonhumpersdm" || username == "dragonhumpers")
				return;
			
			DndViewer viewer = AllViewers.Get(username);
			viewer.LastChatMessage = DateTime.Now;
			
			if (viewer.AlreadySeen)
				return;

			viewer.AlreadySeen = true;

			if (viewer.CardsPlayed > 0)
			{
				VipGreeting(viewer.UserName);
				if (viewer.HasAnyCharges())
					TellViewerAboutRemainingCharges(viewer);
			}
			else
				NormalGreeting(viewer.UserName);
		}
  }
}
