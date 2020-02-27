using System;
using System.Linq;

namespace MapCore
{
	public interface IStampProperties
	{
		string Name { get; set; }

		bool Collectible { get; set; }

		bool Locked { get; set; }

		bool Hideable { get; set; }

		double MinStrengthToMove { get; set; }

		Cover Cover { get; set; }

		StampAltitude Altitude { get; set; }

		double Weight { get; set; }

		/// <summary>
		/// In Gold Pieces
		/// </summary>
		double Value { get; set; }

		Guid Guid { get; set; }
		string TypeName { get; set; }
		IStampProperties Copy(double deltaX, double deltaY);
		double Height { get; set; }
		double Width { get; set; }
		double GetLeft();
		double GetTop();
		double GetBottom();
		double GetRight();

		bool ContainsPoint(double x, double y);
		void ResetImage();
		double Contrast { get; set; }
		string FileName { get; set; }
		bool FlipHorizontally { get; set; }
		bool FlipVertically { get; set; }
		double HueShift { get; set; }
		double Lightness { get; set; }
		StampRotation Rotation { get; set; }
		double Saturation { get; set; }
		double Scale { get; set; }
		double X { get; set; }
		double Y { get; set; }
		int ZOrder { get; set; }
		bool Visible { get; set; }

		double ScaleX { get; }
		double ScaleY { get; }
		bool HasNoZOrder();
		void Move(double deltaX, double deltaY);
		void ResetZOrder();
		void RotateLeft();
		void RotateRight();
		void SwapXY();
		void AdjustScale(double scaleAdjust);
		void SetAbsoluteScaleTo(double newScale);
	}
}
