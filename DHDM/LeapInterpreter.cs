//#define profiling
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LeapTools;
using Newtonsoft.Json;

namespace DHDM
{
	public class LeapInterpreter
	{
		internal LeapMotionCalibrationStep leapMotionCalibrationStep;
		LeapCalibrationData leapCalibrationData = new LeapCalibrationData();
		LeapMotion leapMotion;

		public LeapInterpreter()
		{

		}

		DndCore.Vector fingertipPosition;
		private void LeapMotion_HandsMoved(object sender, LeapFrameEventArgs ea)
		{
			if (ea.Frame.Hands.Count == 0)
				return;
			if (ea.Frame.Hands[0].Fingers.Count == 0)
				return;
			Leap.Vector tipPosition = ea.Frame.Hands[0].Fingers[1].TipPosition;
			fingertipPosition = new DndCore.Vector(tipPosition.x, tipPosition.y, tipPosition.z);
		}

		public void StartCalibration()
		{
			leapMotion = new LeapMotion();
			leapMotion.HandsMoved += LeapMotion_HandsMoved;
			leapMotionCalibrationStep = LeapMotionCalibrationStep.BackUpperLeft;
			leapCalibrationData.SetDiscoverabilityIndex(leapMotionCalibrationStep);
		}

		public void CalibrationScaleChanged(int delta)
		{
			if (leapMotionCalibrationStep == LeapMotionCalibrationStep.NotCalibrating)
				return;
			leapCalibrationData.ChangeScale(delta);
			HubtasticBaseStation.CalibrateLeapMotion(JsonConvert.SerializeObject(leapCalibrationData));

		}
		public void MouseMoved(MouseEventArgs e, MainWindow window)
		{
			if (leapMotionCalibrationStep == LeapMotionCalibrationStep.NotCalibrating)
				return;
			leapCalibrationData.SetXY(e.GetPosition(window));
			HubtasticBaseStation.CalibrateLeapMotion(JsonConvert.SerializeObject(leapCalibrationData));
		}
		public Point MouseDown(MouseButtonEventArgs e, MainWindow mainWindow)
		{
			if (leapMotionCalibrationStep == LeapMotionCalibrationStep.NotCalibrating)
				return default;
			Point position = e.GetPosition(mainWindow);
			// TODO: Record the 2D and 3D points somewhere
			try
			{
				if (leapMotionCalibrationStep == LeapMotionCalibrationStep.FrontLowerRight)
				{
					leapMotionCalibrationStep = LeapMotionCalibrationStep.NotCalibrating;
					leapMotion.HandsMoved -= LeapMotion_HandsMoved;
					leapMotion = null;
					leapCalibrationData.SetDiscoverabilityIndex(leapMotionCalibrationStep);
					HubtasticBaseStation.CalibrateLeapMotion(JsonConvert.SerializeObject(leapCalibrationData));
					return default;
				}
				leapMotionCalibrationStep++;
				leapCalibrationData.SetDiscoverabilityIndex(leapMotionCalibrationStep);
				HubtasticBaseStation.CalibrateLeapMotion(JsonConvert.SerializeObject(leapCalibrationData));
				return position;
			}
			finally
			{
				
			}
		}
	}
}