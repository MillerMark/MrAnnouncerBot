using System;
using System.Linq;

namespace MapCore
{
	public interface IStampProperties : IItemProperties
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

		string TypeName { get; set; }
		IStampProperties Copy(double deltaX, double deltaY);

		double GetBottom();
		double GetRight();

		void ResetImage();
		double Contrast { get; set; }
		bool FlipHorizontally { get; set; }
		bool FlipVertically { get; set; }
		double HueShift { get; set; }
		double Lightness { get; set; }
		StampRotation Rotation { get; set; }
		double Saturation { get; set; }
		double Scale { get; set; }
	
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
