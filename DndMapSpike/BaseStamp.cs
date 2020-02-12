using MapCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DndMapSpike
{
	public abstract class BaseStamp : BaseStampProperties, IStampProperties
	{
		public static IStampProperties CreateStampFrom(SerializedStamp stamp)
		{
			if (stamp.TypeName == "StampGroup")
				return StampGroup.From(stamp);
			return Stamp.From(stamp);
		}

		public abstract IStampProperties Copy(int deltaX, int deltaY);
		public abstract bool ContainsPoint(double x, double y);
		public abstract int GetLeft();
		public abstract int GetTop();
		public abstract double GetBottom();
		public abstract double GetRight();

		public abstract void ResetImage();

		public override bool FlipHorizontally
		{
			get { return flipHorizontally; }
			set
			{
				if (flipHorizontally == value)
					return;

				ResetImage();
				flipHorizontally = value;
			}
		}
		public override bool FlipVertically
		{
			get { return flipVertically; }
			set
			{
				if (flipVertically == value)
					return;

				ResetImage();
				flipVertically = value;
			}
		}
		bool flipHorizontally;
		bool flipVertically;
		StampRotation rotation;
		public override StampRotation Rotation
		{
			get
			{
				return rotation;
			}
			set
			{
				if (rotation == value)
					return;

				rotation = value;
				ResetImage();
			}
		}
		public override string FileName { get; set; }
		double scale = 1;
		public override double Scale
		{
			get
			{
				return scale;
			}
			set
			{
				if (scale == value)
					return;

				ResetImage();
				scale = value;
			}
		}
		double hueShift;
		public override double HueShift
		{
			get
			{
				return hueShift;
			}
			set
			{
				if (hueShift == value)
					return;

				ResetImage();
				hueShift = value;
			}
		}

		double saturation;
		public override double Saturation
		{
			get
			{
				return saturation;
			}
			set
			{
				if (saturation == value)
					return;

				ResetImage();
				saturation = value;
			}
		}
		double lightness;
		public override double Lightness
		{
			get
			{
				return lightness;
			}
			set
			{
				if (lightness == value)
					return;

				ResetImage();
				lightness = value;
			}
		}

		double contrast;
		public override double Contrast
		{
			get
			{
				return contrast;
			}
			set
			{
				if (contrast == value)
					return;

				ResetImage();
				contrast = value;
			}
		}

		public void SwapXY()
		{
			int oldX = X;
			X = Y;
			Y = oldX;
		}
		public bool HasNoZOrder()
		{
			return ZOrder == -1;
		}

		public void ResetZOrder()
		{
			ZOrder = -1;
		}

		public virtual double ScaleX
		{
			get
			{
				bool isFlipping;
				if (Rotation == StampRotation.Ninety || Rotation == StampRotation.TwoSeventy)
					isFlipping = FlipVertically;
				else  // Normal flip
					isFlipping = FlipHorizontally;
				double horizontalFlipFactor = 1;
				if (isFlipping)
					horizontalFlipFactor = -1;
				return Scale * horizontalFlipFactor;
			}
		}
		public virtual double ScaleY
		{
			get
			{
				bool isFlipping;
				if (Rotation == StampRotation.Ninety || Rotation == StampRotation.TwoSeventy)
					isFlipping = FlipHorizontally;
				else  // Normal flip
					isFlipping = FlipVertically;
				double verticalFlipFactor = 1;
				if (isFlipping)
					verticalFlipFactor = -1;
				return Scale * verticalFlipFactor;
			}
		}

		public virtual void RotateRight()
		{
			if (FlipHorizontally ^ FlipVertically)
				DoRotateLeft();
			else
				DoRotateRight();
		}

		private void DoRotateRight()
		{
			switch (Rotation)
			{
				case StampRotation.Zero:
					Rotation = StampRotation.Ninety;
					break;
				case StampRotation.Ninety:
					Rotation = StampRotation.OneEighty;
					break;
				case StampRotation.OneEighty:
					Rotation = StampRotation.TwoSeventy;
					break;
				case StampRotation.TwoSeventy:
					Rotation = StampRotation.Zero;
					break;
			}
		}

		public virtual void RotateLeft()
		{
			if (FlipHorizontally ^ FlipVertically)
				DoRotateRight();
			else
				DoRotateLeft();
		}

		private void DoRotateLeft()
		{
			switch (Rotation)
			{
				case StampRotation.Zero:
					Rotation = StampRotation.TwoSeventy;
					break;
				case StampRotation.Ninety:
					Rotation = StampRotation.Zero;
					break;
				case StampRotation.OneEighty:
					Rotation = StampRotation.Ninety;
					break;
				case StampRotation.TwoSeventy:
					Rotation = StampRotation.OneEighty;
					break;
			}
		}

		public virtual void Move(int deltaX, int deltaY)
		{
			X += deltaX;
			Y += deltaY;
		}
		public virtual void AdjustScale(double scaleAdjust)
		{
			Scale *= scaleAdjust;
		}
		public virtual void SetAbsoluteScaleTo(double newScale)
		{
			Scale = newScale;
		}

		protected override void TransferFrom(SerializedStamp serializedStamp)
		{
			base.TransferFrom(serializedStamp);
		}

		public BaseStamp()
		{
			Guid = Guid.NewGuid();
		}
	}
}

