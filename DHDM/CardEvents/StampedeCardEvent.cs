//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public class StampedeCardEvent : CardEvent
	{
		System.Timers.Timer timer;
		public StampedeCardEvent(object[] args) : base(args)
		{
			CardName = DndCore.Expressions.GetStr((string)args[0]);
			MediaSources = DndCore.Expressions.GetStr((string)args[1]);
			DamageStr = DndCore.Expressions.GetStr((string)args[2]);
		}

		void CreateTimer()
		{
			timer = new System.Timers.Timer();
			timer.Interval = 20 * 1000;
			timer.Elapsed += Timer_Elapsed;
		}

		bool videoPlayIsComplete;
		string dieRollGuid;

		void CheckIfDone()
		{
			if (videoPlayIsComplete && dieStoppedRolling)
				IsDone = true;
		}
		private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			timer.Stop();
			videoPlayIsComplete = true;
			ShowStampedeMediaSource("Sprinkles Trots", false);
			ShowStampedeMediaSource("Humpy Soars", false);
			ShowStampedeMediaSource("Jeffrey Scoots", false);
			CheckIfDone();
		}

		public override void Activate()
		{
			videoPlayIsComplete = false;
			dieStoppedRolling = false;

			if (timer == null)
				CreateTimer();

			string[] mediaSources = MediaSources.Split(';');
			foreach (string mediaSource in mediaSources)
				ShowStampedeMediaSource(mediaSource.Trim(), true);

			timer.Start();
			dieRollGuid = Guid.NewGuid().ToString();
			DungeonMasterApp.SetNextStampedeRoll(DamageStr, dieRollGuid);
			HubtasticBaseStation.DiceStoppedRolling += HubtasticBaseStation_DiceStoppedRolling;
		}

		bool dieStoppedRolling;
		private void HubtasticBaseStation_DiceStoppedRolling(object sender, DiceEventArgs ea)
		{
			if (ea.StopRollingData.rollId == dieRollGuid)
			{
				HubtasticBaseStation.DiceStoppedRolling -= HubtasticBaseStation_DiceStoppedRolling;
				dieStoppedRolling = true;
				CheckIfDone();
			}
		}

		private void ShowStampedeMediaSource(string mediaSource, bool visible)
		{
			ObsManager.SetSourceVisibility(new DndCore.SetObsSourceVisibilityEventArgs()
			{
				DelaySeconds = 0,
				SceneName = "DH.Stampedes",
				SourceName = mediaSource,
				Visible = visible
			});
		}

		public string CardName { get; set; }
		public string MediaSources { get; set; }
		public string DamageStr { get; set; }
	}
}
