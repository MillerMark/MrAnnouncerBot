using System;
using System.Timers;
using System.Collections.Generic;
using OBSWebsocketDotNet.Types;

namespace DHDM
{
  public class LiveFeedScaler: BaseLiveFeedAnimator
	{
		Timer timer;
		static Dictionary<string, LiveFeedScaler> allAnimations = new Dictionary<string, LiveFeedScaler>();
		DateTime startTime;
		public event EventHandler<LiveFeedScaler> Render;

		public LiveFeedScaler(string itemName, 
    string sceneName, 
    double playerX, 
    double videoAnchorHorizontal, 
    double videoAnchorVertical, 
    double videoWidth, 
    double videoHeight, 
    double startScale, 
    double targetScale, 
    double timeMs): base(videoAnchorHorizontal, videoAnchorVertical, videoWidth, videoHeight, sceneName, itemName)
		{
			ScreenAnchorLeft = playerX;
			ScreenAnchorTop = 1080;
			StartScale = startScale;
			timer = new Timer();
			timer.Interval = 40;
			timer.Elapsed += Timer_Elapsed;
			ItemName = itemName;
			
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

		void OnRender(object sender, LiveFeedScaler e)
		{
			Render?.Invoke(sender, e);
		}
		void Disposing()
		{
			Stop();
			timer.Dispose();
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Stop();
			OnRender(this, this);
			double msElapsed = GetElapsedMs();
			if (msElapsed < TimeMs)
				timer.Start();
			else
			{
				if (msElapsed >= TimeMs)
				{
					string key = GetKey();
					Remove(key);
				}
			}
		}

		private static void Remove(string key)
		{
			if (allAnimations.ContainsKey(key))
			{
				allAnimations[key].Disposing();
				allAnimations.Remove(key);
			}
		}
		public double TargetScale { get; }
		public double TimeMs { get; }
		public double StartScale { get; set; }

		void Stop()
		{
			timer.Stop();
		}

		void Start()
		{
			startTime = DateTime.Now;
			timer.Start();
		}

		double EaseIn(double time, double startValue, double change, double duration)
		{
			time /= duration / 2;
			if (time < 1)
				return change / 2 * time * time + startValue;

			time--;
			return -change / 2 * (time * (time - 2) - 1) + startValue;
		}

		public double GetTargetScale()
		{
			double msElapsed = GetElapsedMs();
			if (msElapsed <= 0)
				return StartScale;

			if (msElapsed >= TimeMs)
			{
				Remove(GetKey());
				Stop();
				return TargetScale;
			}

			return EaseIn(msElapsed, StartScale, TargetScale - StartScale, TimeMs);
		}
		
		private double GetElapsedMs()
		{
			return (DateTime.Now - startTime).TotalMilliseconds;
		}
	}
}
