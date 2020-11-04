//#define profiling
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DndCore;
using LeapTools;
using Newtonsoft.Json;
using CoreVector = DndCore.Vector;

namespace DHDM
{
	public class LeapCalibrator
	{
		public static bool Calibrated { get; set; } = true;
		public CoreVector FingertipPosition { get => fingertipPosition; set => fingertipPosition = value; }

		public static double backScale = 0.6;
		public static double frontScale = 1.5;
		public static double backPlaneZ = 248.5;
		public static double frontPlaneZ = -9.842;

		public static CoreVector backUpperLeft3d = new CoreVector(-323.4, 527.5, 248.5);
		public static CoreVector backLowerRight3d = new CoreVector(221.7, 422.6, 240.1);
		public static CoreVector frontUpperLeft3d = new CoreVector(-179.389, 413.59, -9.842);
		public static CoreVector frontLowerRight3d = new CoreVector(29.67, 382.88, 68.613);  // 36.86, 373.5, -3.678

		public static Point backUpperLeft2d = new Point(887, 711);
		public static Point backLowerRight2d = new Point(1578, 976);
		public static Point frontUpperLeft2d = new Point(891, 748);
		public static Point frontLowerRight2d = new Point(1457, 982);  // 1579, 1000

		internal LeapMotionCalibrationStep leapMotionCalibrationStep;
		LeapCalibrationData leapCalibrationData = new LeapCalibrationData();
		LeapMotion leapMotion;

		public LeapCalibrator()
		{

		}

		static CoreVector GetCoreVector(CoreVector backPt, CoreVector frontPt, float zPlane)
		{
			//` <formula 1.4; x = x_1 + t * \Delta x>
			//` <formula 1.4; y = y_1 + t * \Delta y>
			//` <formula 1.4; z = z_1 + t * \Delta z>

			//` <formula 2.6; t = \frac{z - z_1}{\Delta z}>

			double deltaX = frontPt.x - backPt.x;
			double deltaY = frontPt.y - backPt.y;
			double deltaZ = frontPt.z - backPt.z;

			double t = (zPlane - backPt.z) / deltaZ;
			double newX = backPt.x + t * deltaX;
			double newY = backPt.y + t * deltaY;
			double newZ = backPt.z + t * deltaZ;

			return new CoreVector(newX, newY, newZ);
		}

		public static ScaledPoint ToScaledPoint(Leap.Vector vector)
		{
			CoreVector upperLeft3D = GetCoreVector(backUpperLeft3d, frontUpperLeft3d, vector.z);
			CoreVector lowerRight3D = GetCoreVector(backLowerRight3d, frontLowerRight3d, vector.z);

			double percentageX = (vector.x - upperLeft3D.x) / (lowerRight3D.x - upperLeft3D.x);
			double percentageY = (vector.y - upperLeft3D.y) / (lowerRight3D.y - upperLeft3D.y);

			double backDeltaX = backLowerRight2d.X - backUpperLeft2d.X;
			double frontDeltaX = frontLowerRight2d.X - frontUpperLeft2d.X;
			double backDeltaY = backLowerRight2d.Y - backUpperLeft2d.Y;
			double frontDeltaY = frontLowerRight2d.Y - frontUpperLeft2d.Y;

			double backX2d = backUpperLeft2d.X + percentageX * backDeltaX;
			double backY2d = backUpperLeft2d.Y + percentageY * backDeltaY;

			double frontX2d = frontUpperLeft2d.X + percentageX * frontDeltaX;
			double frontY2d = frontUpperLeft2d.Y + percentageY * frontDeltaY;

			double percentageZ = (vector.z - backPlaneZ) / (frontPlaneZ - backPlaneZ);

			double deltaX2d = frontX2d - backX2d;
			double deltaY2d = frontY2d - backY2d;

			double x = backX2d + percentageZ * deltaX2d;
			double y = backY2d + percentageZ * deltaY2d;

			double deltaScale = frontScale - backScale;
			double scale = backScale + percentageZ * deltaScale;

			return new ScaledPoint() { X = (int)Math.Round(x), Y = (int)Math.Round(y), Scale = scale };
		}

		CoreVector fingertipPosition;
		private void LeapMotion_HandsMoved(object sender, LeapFrameEventArgs ea)
		{
			if (ea.Frame.Hands.Count == 0)
				return;
			if (ea.Frame.Hands[0].Fingers.Count == 0)
				return;

			if (leapMotionCalibrationStep != LeapMotionCalibrationStep.NotCalibrating)
			{
				Leap.Vector tipPosition = ea.Frame.Hands[0].Fingers[1].TipPosition;
				fingertipPosition = new CoreVector(tipPosition.x, tipPosition.y, tipPosition.z);
				leapCalibrationData.SetFingertipPosition(fingertipPosition);
				HubtasticBaseStation.CalibrateLeapMotion(JsonConvert.SerializeObject(leapCalibrationData));
			}
		}

		public void StartCalibration()
		{
			leapMotion = new LeapMotion();
			leapMotion.HandsMoved += LeapMotion_HandsMoved;
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

		public double ActiveScale
		{
			get
			{
				return leapCalibrationData.Scale;
			}
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
			if (e.LeftButton == MouseButtonState.Pressed)
				StorePoints();
			if (leapMotionCalibrationStep == LeapMotionCalibrationStep.FrontLowerRight)
			{
				leapMotionCalibrationStep = LeapMotionCalibrationStep.NotCalibrating;
				leapMotion.HandsMoved -= LeapMotion_HandsMoved;
				leapMotion.DisposeController();
				Calibrated = true;
				leapMotion = null;
			}
			else
			{
				leapMotionCalibrationStep++;
			}

			leapCalibrationData.SetDiscoverabilityIndex(leapMotionCalibrationStep);
			HubtasticBaseStation.CalibrateLeapMotion(JsonConvert.SerializeObject(leapCalibrationData));
			return position;
		}
	}
}