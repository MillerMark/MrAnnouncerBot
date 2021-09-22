//#define profiling
using DndCore;
using ObsControl;
using System;
using System.Linq;
using System.Timers;

namespace DHDM
{
	public class StampedeCardEvent : CardEvent
	{
		const string MediaSource_SprinklesTrots = "Sprinkles Trots";
		const string Scene_Stampedes = "DH.Stampedes";
		const string MediaSource_HumpySoars = "Humpy Soars";
		const string MediaSource_JeffreyScoots = "Jeffrey Scoots";
		Timer stampedePlaybackTimer;
		Timer remindDungeonMasterTimer;

		public StampedeCardEvent(object[] args) : base(args)
		{
			// TODO: Play and activate the card from here. We're playing way too soon.
			CardName = Expressions.GetStr((string)args[0]);
			MediaSources = Expressions.GetStr((string)args[1]);
			DamageStr = Expressions.GetStr((string)args[2]);
			CreateTimers();
		}

		void CreateTimers()
		{
			stampedePlaybackTimer = new Timer();
			const int stampedeVideoPlaybackTimeSec = 20;
			stampedePlaybackTimer.Interval = stampedeVideoPlaybackTimeSec * 1000;
			stampedePlaybackTimer.Elapsed += StampedePlaybackTimer_Elapsed;

			const int dmReminderSpanSec = 90;
			remindDungeonMasterTimer = new Timer();
			remindDungeonMasterTimer.Interval = dmReminderSpanSec * 1000; 
			remindDungeonMasterTimer.Elapsed += RemindDungeonMasterTimer_Elapsed;
		}

		private void RemindDungeonMasterTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			TimeSpan timeSpanSinceCardPlay = DateTime.Now - timeActivated;
			DungeonMasterApp.TellDungeonMaster($"Reminder caulfielder: The \"{CardName}\" stampede card was activated {(int)timeSpanSinceCardPlay.TotalSeconds} seconds ago. Target the players/NPCs in the path and roll their dexterity saving throws!");
		}

		bool videoPlayIsComplete;
		string stampedeDieRollGuid;

		void CheckIfDone()
		{
			if (videoPlayIsComplete && dieStoppedRolling)
				IsDone = true;
		}
		private void StampedePlaybackTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			stampedePlaybackTimer.Stop();
			videoPlayIsComplete = true;
			HideAllStampedes();
			CheckIfDone();
		}

		private void HideAllStampedes()
		{
			ShowStampedeMediaSource(MediaSource_SprinklesTrots, false);
			ShowStampedeMediaSource(MediaSource_HumpySoars, false);
			ShowStampedeMediaSource(MediaSource_JeffreyScoots, false);
		}

		public override void Activate()
		{
			const bool suppressMediaForDebugging = false;
			timeActivated = DateTime.Now;
			videoPlayIsComplete = false;
			dieStoppedRolling = false;

			if (!suppressMediaForDebugging)
			{
				string[] mediaSources = MediaSources.Split(';');
				foreach (string mediaSource in mediaSources)
					ShowStampedeMediaSource(mediaSource.Trim(), true);
			}

			stampedePlaybackTimer.Start();
			stampedeDieRollGuid = Guid.NewGuid().ToString();
			DungeonMasterApp.SetNextStampedeRoll(CardName, UserName, DamageStr, stampedeDieRollGuid);
			remindDungeonMasterTimer.Start();
			HubtasticBaseStation.DiceStoppedRolling += HubtasticBaseStation_DiceStoppedRolling;
		}

		public override void ConditionRoll(DiceRoll diceRoll)
		{
			if (diceRoll.RollID == stampedeDieRollGuid)
				remindDungeonMasterTimer.Stop();
		}

		bool dieStoppedRolling;
		DateTime timeActivated;
		private void HubtasticBaseStation_DiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			if (ea.StopRollingData.rollId == stampedeDieRollGuid)
			{
				HubtasticBaseStation.DiceStoppedRolling -= HubtasticBaseStation_DiceStoppedRolling;
				dieStoppedRolling = true;
				CheckIfDone();
			}
		}

		private void ShowStampedeMediaSource(string mediaSource, bool visible)
		{
			ObsManager.SetSourceVisibility(Scene_Stampedes, mediaSource, visible);
		}

		public string CardName { get; set; }
		public string MediaSources { get; set; }
		public string DamageStr { get; set; }
	}
}
