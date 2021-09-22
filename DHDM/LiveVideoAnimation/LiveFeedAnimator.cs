using System;
using System.Collections.Generic;
using Imaging;
using System.Timers;
using OBSWebsocketDotNet.Types;

namespace DHDM
{
  public class LiveFeedAnimator : BaseLiveFeedAnimator
  {
    public event EventHandler AnimationComplete;
    int frameIndex;
    Timer timer;
    DateTime startTime;
    TimeSpan totalDrawTimeSoFar;
		bool allDone;
		bool needToStopNow;

		public LiveFeedAnimator(double videoAnchorHorizontal,
      double videoAnchorVertical,
      double videoWidth,
      double videoHeight,
      string sceneName,
      string itemName,
      List<LiveFeedFrame> liveFeedFrames) : base(videoAnchorHorizontal, 
                            videoAnchorVertical, 
                            videoWidth, 
                            videoHeight, sceneName, itemName)
    {
      LiveFeedFrames = liveFeedFrames;
      timer = new Timer();
      timer.Elapsed += Timer_Elapsed;
      // TODO: Figure out ScreenAnchorLeft and ScreenAnchorTop
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      timer.Stop();
      AdvanceFrame();
    }

    private void AdvanceFrame()
    {
      if (needToStopNow)
        frameIndex = LiveFeedFrames.Count - 1;

      LiveFeedFrame liveFeedFrame = LiveFeedFrames[frameIndex];
      if (liveFeedFrame == null)
      {
        Stop();
        return;
      }

			RenderActiveFrame(liveFeedFrame);
			SetNextTimer(liveFeedFrame);
			
      frameIndex++;
      if (frameIndex >= LiveFeedFrames.Count || needToStopNow)
        Stop();
    }

    void RenderActiveFrame(LiveFeedFrame liveFeedFrame)
    {
      ScreenAnchorLeft = liveFeedFrame.Origin.X;
      ScreenAnchorTop = liveFeedFrame.Origin.Y;
      VideoFeedAnimationManager.ObsManager.SizeAndPositionItem(this, (float)liveFeedFrame.Scale, liveFeedFrame.Opacity, liveFeedFrame.Rotation);
    }

    void SetNextTimer(LiveFeedFrame liveFeedFrame)
		{
			if (allDone)
				return;
			DateTime now = DateTime.Now;
			totalDrawTimeSoFar += TimeSpan.FromSeconds(liveFeedFrame.Duration);
			DateTime nextFrameDrawTime = startTime + totalDrawTimeSoFar;
			if (nextFrameDrawTime <= now)
				timer.Interval = 1;  // Render the next frame ASAP.
			else
				timer.Interval = (nextFrameDrawTime - now).TotalMilliseconds;
			timer.Start();
		}

		public List<LiveFeedFrame> LiveFeedFrames { get; set; }

    void OnAnimationComplete(object sender, EventArgs e)
    {
      AnimationComplete?.Invoke(sender, e);
    }

    void Finished()
    {
      OnAnimationComplete(this, EventArgs.Empty);
    }

    public void Start()
    {
      if (LiveFeedFrames.Count == 0)  // Nothing to animate.
        return;
      frameIndex = 0;
      startTime = DateTime.Now - TimeSpan.FromSeconds(0.25);  // Sync adjust.
      totalDrawTimeSoFar = TimeSpan.Zero;
      timer.Interval = 1;
      timer.Start();  // Start ASAP.
    }

		public void Stop()
		{
      allDone = true;
			timer.Stop();
      Finished();
      // TODO: Restore the feed and stop any timers. Abort! Abort! Abort!
    }

		public void Reset()
		{
      frameIndex = 0;
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
