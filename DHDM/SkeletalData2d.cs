//#define profiling
using Leap;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DHDM
{
	public class SkeletalData2d
	{
		public List<Hand2d> Hands { get; set; } = new List<Hand2d>();
		public ScaledPlane BackPlane { get; set; } = new ScaledPlane();
		public ScaledPlane FrontPlane { get; set; } = new ScaledPlane();
		public ScaledPlane ActivePlane { get; set; } = new ScaledPlane();
		public bool ShowBackPlane { get; set; }
		public bool ShowFrontPlane { get; set; }
		public bool ShowActivePlane { get; set; }
		public bool ShowLiveHandPosition { get; set; }
		public HandFxDto HandEffect { get; set; }

		public SkeletalData2d()
		{
			
		}

		public void SetFromFrame(Frame frame)
		{
			Hands.Clear();
			if (frame == null)
				return;

			foreach (Hand hand in frame.Hands)
				Hands.Add(new Hand2d(hand));

			CheckForThrow();
		}

		void CheckForThrow()
		{
			foreach (Hand2d hand in Hands)
				CheckForThrow(hand);
		}

		int Throw(Vector throwVector)
		{
			return 0;
			// TODO: Left off here. 
			// Create a 3D virtual object, add it to the list of known thrown objects.
			// Apply Physics on those objects - we need a timer in case hand tracking data disappears at least 30fps
			// Map those 3D virtual objects to 2d space
			// Send those 2d space virtual object list (including the virtual object index returned by this function) down to the overlay.
		}

		void CheckForThrow(Hand2d hand)
		{
			Vector throwVector = HandVelocityHistory.GetThrowVector(hand);
			bool validThrow = throwVector.Magnitude > 0;
			if (!validThrow)
				return;
			HandVelocityHistory.ClearHistory(hand);
			hand.Throwing = true;
			hand.ThrownObjectIndex = Throw(throwVector);
		}

		public bool ShowingDiagnostics()
		{
			return ShowBackPlane || ShowFrontPlane || ShowActivePlane;
		}
	}
}