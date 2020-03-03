using System;
using System.Linq;

namespace MapCore
{
	public interface IStampProperties : IItemProperties
	{
		string Name { get; set; }

		bool Collectible { get; set; }

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
		void RotateLeft();
		void RotateRight();
		void SwapXY();
		void AdjustScale(double scaleAdjust);
		void SetAbsoluteScaleTo(double newScale);
	}
}
