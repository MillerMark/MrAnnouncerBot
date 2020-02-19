using System;
using System.Linq;

namespace MapCore
{
	public interface IStampProperties
	{
		[EditableProperty]
		string Name { get; set; }

		[EditableProperty]
		bool Collectible { get; set; }

		[EditableProperty]
		bool Hideable { get; set; }

		[EditableProperty("Strength to Move")]
		double StrengthToMove { get; set; }

		[EditableProperty]
		Cover Cover { get; set; }

		[EditableProperty]
		StampAltitude Altitude { get; set; }

		[EditableProperty]
		double Weight { get; set; }

		/// <summary>
		/// In Gold Pieces
		/// </summary>
		[EditableProperty]
		double Value { get; set; }

		Guid Guid { get; set; }
		string TypeName { get; set; }
		IStampProperties Copy(int deltaX, int deltaY);
		int Height { get; set; }
		int Width { get; set; }
		int GetLeft();
		int GetTop();
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
		int X { get; set; }
		int Y { get; set; }
		int ZOrder { get; set; }
		bool Visible { get; set; }

		double ScaleX { get; }
		double ScaleY { get; }
		bool HasNoZOrder();
		void Move(int deltaX, int deltaY);
		void ResetZOrder();
		void RotateLeft();
		void RotateRight();
		void SwapXY();
		void AdjustScale(double scaleAdjust);
		void SetAbsoluteScaleTo(double newScale);
	}
}
