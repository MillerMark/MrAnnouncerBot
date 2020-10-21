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
		double backScale;
		double frontScale;
		double backPlaneZ;
		double frontPlaneZ;
		
		DndCore.Vector backUpperLeft3d;
		DndCore.Vector backLowerRight3d;
		DndCore.Vector frontUpperLeft3d;
		DndCore.Vector frontLowerRight3d;

		Point backUpperLeft2d;
		Point backLowerRight2d;
		Point frontUpperLeft2d;
		Point frontLowerRight2d;

		internal LeapMotionCalibrationStep leapMotionCalibrationStep;
		LeapCalibrationData leapCalibrationData = new LeapCalibrationData();
		LeapMotion leapCalibrator;

		public LeapInterpreter()
		{

		}

		DndCore.Vector fingertipPosition;
		public bool Testing { get; set; }
		private void LeapMotion_HandsMoved(object sender, LeapFrameEventArgs ea)
		{
			if (ea.Frame.Hands.Count == 0)
				return;
			if (ea.Frame.Hands[0].Fingers.Count == 0)
				return;

			if (leapMotionCalibrationStep != LeapMotionCalibrationStep.NotCalibrating)
			{
				Leap.Vector tipPosition = ea.Frame.Hands[0].Fingers[1].TipPosition;
				fingertipPosition = new DndCore.Vector(tipPosition.x, tipPosition.y, tipPosition.z);
				leapCalibrationData.SetFingertipPosition(fingertipPosition);
				HubtasticBaseStation.CalibrateLeapMotion(JsonConvert.SerializeObject(leapCalibrationData));
			}
		}

		public void StartCalibration()
		{
			leapCalibrator = new LeapMotion();
			leapCalibrator.HandsMoved += LeapMotion_HandsMoved;
			leapMotionCalibrationStep = LeapMotionCalibrationStep.BackUpperLeft;
			leapCalibrationData.SetDiscoverabilityIndex(leapMotionCalibrationStep);
			HubtasticBaseStation.CalibrateLeapMotion(JsonConvert.SerializeObject(leapCalibrationData));
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

		void StorePoints()
		{
			Point mousePosition = new Point(leapCalibrationData.X, leapCalibrationData.Y);
			switch (leapMotionCalibrationStep)
			{
				case LeapMotionCalibrationStep.BackUpperLeft:
					backUpperLeft3d = fingertipPosition;
					backUpperLeft2d = mousePosition;
					backPlaneZ = fingertipPosition.z;
					backScale = leapCalibrationData.Scale;
					break;
				case LeapMotionCalibrationStep.BackLowerRight:
					backLowerRight3d = fingertipPosition;
					backLowerRight2d = mousePosition;
					if (fingertipPosition.z > backPlaneZ)
					{
						backPlaneZ = fingertipPosition.z;
						backScale = leapCalibrationData.Scale;
					}
					break;
				case LeapMotionCalibrationStep.FrontUpperLeft:
					frontUpperLeft3d = fingertipPosition;
					frontUpperLeft2d = mousePosition;
					frontPlaneZ = fingertipPosition.z;
					frontScale = leapCalibrationData.Scale;
					break;
				case LeapMotionCalibrationStep.FrontLowerRight:
					frontLowerRight3d = fingertipPosition;
					frontLowerRight2d = mousePosition;
					if (fingertipPosition.z < frontPlaneZ)
					{
						frontPlaneZ = fingertipPosition.z;
						frontScale = leapCalibrationData.Scale;
					}
					break;
			}
		}

		public Point MouseDown(MouseButtonEventArgs e, MainWindow mainWindow)
		{
			if (leapMotionCalibrationStep == LeapMotionCalibrationStep.NotCalibrating)
				return default;
			Point position = e.GetPosition(mainWindow);
			// TODO: Record the 2D and 3D points somewhere
			StorePoints();
			if (leapMotionCalibrationStep == LeapMotionCalibrationStep.FrontLowerRight)
			{
				leapMotionCalibrationStep = LeapMotionCalibrationStep.NotCalibrating;
				leapCalibrator.HandsMoved -= LeapMotion_HandsMoved;
				leapCalibrator.DisposeController();
				leapCalibrator = null;
			}
			else
			{
				leapMotionCalibrationStep++;
			}

			leapCalibrationData.SetDiscoverabilityIndex(leapMotionCalibrationStep);
			HubtasticBaseStation.CalibrateLeapMotion(JsonConvert.SerializeObject(leapCalibrationData));
			return position;
		}

		public void ToggleTesting()
		{
			Testing = !Testing;
		}
	}
}