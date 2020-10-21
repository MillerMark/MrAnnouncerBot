//#define profiling
using System;
using System.Linq;
using LeapTools;
using Newtonsoft.Json;

namespace DHDM
{
	public class LeapDevice
	{
		LeapMotion leapMotion;
		bool testing;
		public bool Testing
		{
			get
			{
				return testing;
			}
			set
			{
				if (testing == value)
					return;
				testing = value;
				if (testing)
				{
					leapMotion = new LeapMotion();
					leapMotion.HandsMoved += LeapMotion_HandsMoved;
				}
				else
				{
					leapMotion.HandsMoved -= LeapMotion_HandsMoved;
					leapMotion.DisposeController();
					leapMotion = null;
				}
			}
		}

		private void LeapMotion_HandsMoved(object sender, LeapFrameEventArgs ea)
		{
			if (!LeapCalibrator.Calibrated)
				return;
			ScaledPoint scaledPoint = LeapCalibrator.ToScaledPoint(ea.Frame.Hands[0].Fingers[1].TipPosition);
			HubtasticBaseStation.UpdateSkeletalData(JsonConvert.SerializeObject(scaledPoint));
		}

		public LeapDevice()
		{

		}

		public void ToggleTesting()
		{
			Testing = !Testing;
		}
	}
}