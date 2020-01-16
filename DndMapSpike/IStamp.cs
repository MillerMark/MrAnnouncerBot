using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DndMapSpike
{
	public interface IStamp
	{
		bool ContainsPoint(Point point);
		IStamp Copy(int deltaX, int deltaY);
		int GetLeft();
		int GetTop();
		bool HasNoZOrder();
		void Move(int deltaX, int deltaY);
		void ResetZOrder();
		void RotateLeft();
		void RotateRight();
		void BlendStampImage(StampsLayer stampsLayer, int xOffset = 0, int yOffset = 0);
		void CreateFloating(Canvas canvas, int left = 0, int top = 0);

		double Contrast { get; set; }
		string FileName { get; set; }
		bool FlipHorizontally { get; set; }
		bool FlipVertically { get; set; }
		int Height { get; }
		double HueShift { get; set; }
		Image Image { get; }
		double Lightness { get; set; }
		//int RelativeX { get; set; }
		//int RelativeY { get; set; }
		StampRotation Rotation { get; set; }
		double Saturation { get; set; }
		double Scale { get; set; }
		double ScaleX { get; }
		double ScaleY { get; }
		int Width { get; }
		int X { get; set; }
		int Y { get; set; }
		int ZOrder { get; set; }
		void SwapXY();
		void AdjustScale(double scaleAdjust);
		void SetAbsoluteScaleTo(double newScale);
	}
}

