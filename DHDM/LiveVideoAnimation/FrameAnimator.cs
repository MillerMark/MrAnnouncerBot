using System;
using System.Timers;

namespace DHDM
{
	public class FrameAnimator
	{
		public event RenderFrameEventHandler RenderFrame;
		public event EventHandler AnimationComplete;
		public int FrameIndex { get; set; }
		Timer timer;
		DateTime startTime;
		TimeSpan totalDrawTimeSoFar;
		bool allDone;
		bool needToStopNow;
		readonly int frameCount;
		public LightingSequence LightingSequence { get; set; }

		public FrameAnimator(int frameCount)
		{
			this.frameCount = frameCount;
			timer = new Timer();
			timer.Elapsed += Timer_Elapsed;
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			timer.Stop();
			AdvanceFrame();
		}

		private void AdvanceFrame()
		{
			if (needToStopNow)
				FrameIndex = frameCount - 1;

			if (FrameIndex < frameCount)
			{
				OnRenderFrame(out double duration, out bool shouldStop);
				if (shouldStop)
					return;

				SetNextTimer(duration);

				FrameIndex++;
			}
			if (FrameIndex >= frameCount || needToStopNow)
				Stop();
		}

		protected void OnRenderFrame(out double duration, out bool shouldStop)
		{
			RenderFrameEventArgs ea = new RenderFrameEventArgs();
			RenderFrame?.Invoke(this, ea);
			duration = ea.Duration;
			shouldStop = ea.ShouldStop;
		}

		void SetNextTimer(double duration)
		{
			if (allDone)
				return;
			DateTime now = DateTime.Now;
			//totalDrawTimeSoFar += TimeSpan.FromSeconds(liveFeedFrame.Duration * TimeStretchFactor); FAILS!
			totalDrawTimeSoFar += TimeSpan.FromTicks((long)(10_000_000 * duration * TimeStretchFactor));  // WORKS!!!
			DateTime nextFrameDrawTime = startTime + totalDrawTimeSoFar;
			if (nextFrameDrawTime <= now)
				timer.Interval = 1;  // Render the next frame ASAP.
			else
				timer.Interval = (nextFrameDrawTime - now).TotalMilliseconds;
			timer.Start();
		}

		public double StartTimeOffset { get; set; } = 0;
		public double TimeStretchFactor { get; set; } = 1;
		public object Data { get; set; }
		public DateTime StartTime { get => startTime; }

		void OnAnimationComplete(object sender, EventArgs e)
		{
			AnimationComplete?.Invoke(sender, e);
		}

		void Finished()
		{
			OnAnimationComplete(this, EventArgs.Empty);
		}

		public void Start(DateTime startTime)
		{
			if (frameCount == 0)  // Nothing to animate.
				return;
			FrameIndex = 0;

			this.startTime = startTime;

			// Sync offset start time if the video settings say so.
			if (StartTimeOffset < 0)
				this.startTime -= TimeSpan.FromSeconds(-StartTimeOffset);
			else if (StartTimeOffset > 0)
				this.startTime += TimeSpan.FromSeconds(StartTimeOffset);

			totalDrawTimeSoFar = TimeSpan.Zero;
			timer.Interval = 1;
			timer.Start();  // Start ASAP.
		}

		public void Stop()
		{
			allDone = true;
			timer.Stop();
			Finished();
		}

		public void Reset()
		{
			FrameIndex = 0;
		}

		public void StopSoon()
		{
			timer.Stop();
			timer.Interval = 1;
			needToStopNow = true;
			timer.Start();
		}
	}
}