//#define profiling
using System;
using DndCore;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using LeapTools;
using Newtonsoft.Json;
using System.Timers;
using BotCore;
using Imaging;

namespace DHDM
{
	public class LeapDevice
	{
		private const double fps30 = 1000 / 30;
		Timer timer = new Timer(fps30) { Enabled = true };

		LeapMotion leapMotion;
		bool active;
		public bool Active
		{
			get
			{
				return active;
			}
			set
			{
				if (active == value)
					return;
				active = value;
				if (active)
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

		SkeletalData2d skeletalData2d = new SkeletalData2d();
		DateTime LastUpdateTime;
		LifxService lifxService;
		bool ShowingDiagnostics()
		{
			return skeletalData2d.ShowingDiagnostics();
		}

		public VectorCompassDirection GetPalmDirection()
		{
			if (skeletalData2d.Hands.Count > 0)
				return skeletalData2d.Hands[0].PalmDirection;

			return VectorCompassDirection.None;
		}

		void ClearImpulseData()
		{
			skeletalData2d.HandEffect = null;
		}

		private void LeapMotion_HandsMoved(object sender, LeapFrameEventArgs ea)
		{
			if (!LeapCalibrator.Calibrated && !ShowingDiagnostics())
				return;

			if (LeapCalibrator.Calibrated)
				skeletalData2d.SetFromFrame(ea.Frame);
			else
				skeletalData2d.SetFromFrame(null);

			UpdateData();
		}

		private const int MinTimeBetweenUpdatesMs = 1000 / 10;

		private const double LeftBulbMaxBrightness = 0.8;
		private const double RightBulbMaxBrightness = 0.6;

		async void ChangeBulbSettings(string bulbLabel, int hueShift, double brightness)
		{
			HueSatLight hsl = new HueSatLight(hueShift / 360.0, 1, 0.5);
			System.Diagnostics.Debug.WriteLine($"{hsl.AsHtml}");
			await lifxService.ChangeBulbSettings(bulbLabel, hsl.AsHtml, brightness);
		}

		void ChangeLeftBulbSettings(int hueShift, double brightnessMultiplier)
		{
			ChangeBulbSettings("Left", hueShift, brightnessMultiplier * LeftBulbMaxBrightness);
		}

		void ChangeRightBulbSettings(int hueShift, double brightnessMultiplier)
		{
			ChangeBulbSettings("Right", hueShift, brightnessMultiplier * RightBulbMaxBrightness);
		}

		int GetHueShiftFromHand(Hand2d hand2D)
		{
			if (hand2D.Side == HandSide.Left)
				return skeletalData2d.TrackableEffectHueShiftLeft;
			else
				return skeletalData2d.TrackableEffectHueShiftRight;
		}

		async void UpdateLights()
		{
			if (DateTime.Now - LastLightUpdateTime < TimeSpan.FromMilliseconds(MinTimeBetweenUpdatesMs))
				return;

			Leap.Vector markPosition = new Leap.Vector(-20.2903f, 445.1746f, 423.4169f);
			lock (skeletalData2d.handsLock)
				foreach (Hand2d hand2D in skeletalData2d.Hands)
				{
					Leap.Vector lightPosition = skeletalData2d.GetLightPosition(hand2D);

					if (lightPosition != null)
					{
						double deltaX = lightPosition.x - markPosition.x;
						double deltaY = lightPosition.y - markPosition.y;
						double deltaZ = lightPosition.z - markPosition.z;
						double distance = Math.Round(Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ));

						// Pythagorean theorem in 3D:

						// <formula 2; distance = \sqrt{deltaX^2 + deltaY^2 + deltaZ^2}>

						const int minDistance = 100;
						const int maxDistance = 250;
						distance = Math.Max(minDistance, distance);
						distance = Math.Min(maxDistance, distance);

						double brightnessMultiplier = (distance - minDistance) / (maxDistance - minDistance);
						// 250 to about 100

						int hueShift = GetHueShiftFromHand(hand2D);
						if (lightPosition.x < markPosition.x)
							ChangeLeftBulbSettings(hueShift, brightnessMultiplier);
						else
							ChangeRightBulbSettings(hueShift, brightnessMultiplier);

//						System.Diagnostics.Debug.WriteLine($"({lightPosition.x}, {lightPosition.y}, {lightPosition.z})");
					}
					
				}

			LastLightUpdateTime = DateTime.Now;
		}

		DateTime LastLightUpdateTime = DateTime.Now;

		private void UpdateData()
		{
			UpdateLights();
			// Don't update the LIFX bulbs faster than 20Hz!

			skeletalData2d.UpdateVirtualObjects();
			lock (skeletalData2d.world.Instances)
				HubtasticBaseStation.UpdateSkeletalData(JsonConvert.SerializeObject(skeletalData2d));
			LastUpdateTime = DateTime.Now;
			ClearImpulseData();
		}

		public LeapDevice()
		{
			timer.Elapsed += Timer_Elapsed;
			lifxService = new LifxService(new MySecureString(Twitch.Configuration["Secrets:LifxBearerToken"]));
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			// TODO: Turn timer on when we have any virtual objects and off when there are none.
			if (!skeletalData2d.HasVirtualObjects())
				return;

			if ((DateTime.Now - LastUpdateTime).TotalMilliseconds > fps30)
			{
				UpdateData();
			}
		}

		public bool ShowingLiveHandPosition
		{
			get
			{
				return skeletalData2d.ShowLiveHandPosition;
			}
			set
			{
				skeletalData2d.ShowLiveHandPosition = value;
			}
		}


		public void SetDiagnosticsOptions(bool showBackPlane, bool showFrontPlane, bool showActivePlane)
		{
			skeletalData2d.ShowActivePlane = showActivePlane;
			skeletalData2d.ShowFrontPlane = showFrontPlane;
			skeletalData2d.ShowBackPlane = showBackPlane;
		}

		public void CalibrationPointUpdated(LeapMotionCalibrationStep leapMotionCalibrationStep, Point position, DndCore.Vector point3D, double scale)
		{
			switch (leapMotionCalibrationStep)
			{
				case LeapMotionCalibrationStep.BackUpperLeft:
					skeletalData2d.BackPlane.UpperLeft2D = Point2D.From(position);
					skeletalData2d.BackPlane.UpperLeft = ScaledPoint.From(point3D, scale);
					break;
				case LeapMotionCalibrationStep.BackLowerRight:
					skeletalData2d.BackPlane.LowerRight2D = Point2D.From(position);
					skeletalData2d.BackPlane.LowerRight = ScaledPoint.From(point3D, scale);
					break;
				case LeapMotionCalibrationStep.FrontUpperLeft:
					skeletalData2d.FrontPlane.UpperLeft2D = Point2D.From(position);
					skeletalData2d.FrontPlane.UpperLeft = ScaledPoint.From(point3D, scale);
					break;
				case LeapMotionCalibrationStep.FrontLowerRight:
					skeletalData2d.FrontPlane.LowerRight2D = Point2D.From(position);
					skeletalData2d.FrontPlane.LowerRight = ScaledPoint.From(point3D, scale);
					break;
			}
		}
		public void TriggerHandFx(HandFxDto handFxDto)
		{
			// TODO: Add support for hand-specific (left or right) placement.
			// TODO: Add support for tossable objects in the stream deck commands.
			skeletalData2d.HandEffect = handFxDto;
		}

		public void LaunchHandTrackingEffect(string launchCommand, string dataValue)
		{
			switch (launchCommand)
			{
				case "TowardFingers":
					skeletalData2d.LaunchTowardFingers(dataValue.ToInt(1));
					break;
				case "ToCamera":
					skeletalData2d.LaunchToCamera(dataValue.ToInt(1));
					break;

				default:
					return;
			}
			UpdateData();
		}
	}
}