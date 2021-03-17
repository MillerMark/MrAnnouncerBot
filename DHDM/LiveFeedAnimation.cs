using System;
using System.Timers;
using System.Collections.Generic;
using OBSWebsocketDotNet.Types;

namespace DHDM
{
	public class LiveFeedAnimation
	{
		Timer timer;
		static Dictionary<string, LiveFeedAnimation> allAnimations = new Dictionary<string, LiveFeedAnimation>();
		DateTime startTime;
		public event EventHandler<LiveFeedAnimation> Render;
		public LiveFeedAnimation()
		{
		}

		public LiveFeedAnimation(string itemName, string sceneName, double playerX, double videoAnchorHorizontal, double videoAnchorVertical, double videoWidth, double videoHeight, double startScale, double targetScale, double timeMs)
		{
			VideoHeight = videoHeight;
			VideoWidth = videoWidth;
			PlayerX = playerX;
			StartScale = startScale;
			timer = new Timer();
			timer.Interval = 75;
			timer.Elapsed += Timer_Elapsed;
			ItemName = itemName;
			SceneName = sceneName;
			VideoAnchorHorizontal = videoAnchorHorizontal;
			VideoAnchorVertical = videoAnchorVertical;
			TargetScale = targetScale;
			TimeMs = timeMs;
			string key = GetKey();
			if (allAnimations.ContainsKey(key))
			{
				allAnimations[key].Stop();
				allAnimations.Remove(key);
			}

			allAnimations.Add(key, this);
			Start();
		}

		private string GetKey()
		{
			return $"{SceneName}.{ItemName}";
		}

		void OnRender(object sender, LiveFeedAnimation e)
		{
			Render?.Invoke(sender, e);
		}
		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			timer.Stop();
			OnRender(this, this);

			if (GetElapsedMs() < TimeMs)
				timer.Start();
		}

		public string ItemName { get; }
		public string SceneName { get; }
		public double VideoAnchorHorizontal { get; }
		public double VideoAnchorVertical { get; }
		public double TargetScale { get; }
		public double TimeMs { get; }
		public double StartScale { get; set; }
		public double PlayerX { get; set; }
		public double VideoWidth { get; set; }
		public double VideoHeight { get; set; }

		void Stop()
		{
			timer.Stop();
		}

		void Start()
		{
			startTime = DateTime.Now;
			timer.Start();
		}

		public double GetTargetScale()
		{
			double msElapsed = GetElapsedMs();
			if (msElapsed <= 0)
				return StartScale;

			if (msElapsed >= TimeMs)
			{
				allAnimations.Remove(GetKey());
				Stop();
				return TargetScale;
			}

			double percentComplete = msElapsed / TimeMs;
			if (ItemName == "Fred Rage")
				Console.WriteLine(StartScale + (TargetScale - StartScale) * percentComplete);
			return StartScale + (TargetScale - StartScale) * percentComplete;
		}
		
		private double GetElapsedMs()
		{
			return (DateTime.Now - startTime).TotalMilliseconds;
		}
	}
}
