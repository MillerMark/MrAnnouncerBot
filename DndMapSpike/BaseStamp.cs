using MapCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DndMapSpike
{
	public abstract class BaseStamp : BaseStampProperties, IStampProperties
	{
		public abstract IStampProperties Copy(double deltaX, double deltaY);
		public abstract bool ContainsPoint(double x, double y);
		public abstract double GetBottom();
		public abstract double GetRight();

		public abstract void ResetImage();

		public override bool FlipHorizontally
		{
			get { return flipHorizontally; }
			set
			{
				if (Locked)
					return;

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
				if (Locked)
					return;

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
				if (Locked)
					return;

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
				if (Locked)
					return;

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
				if (Locked)
					return;

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
				if (Locked)
					return;

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
				if (Locked)
					return;

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
				if (Locked)
					return;

				if (contrast == value)
					return;

				ResetImage();
				contrast = value;
			}
		}

		public void SwapXY()
		{
			double oldX = X;
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
			if (Locked)
				return;

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
			if (Locked)
				return;

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

		public virtual void Move(double deltaX, double deltaY)
		{
			if (Locked)
				return;

			X += deltaX;
			Y += deltaY;
		}
		public virtual void AdjustScale(double scaleAdjust)
		{
			if (Locked)
				return;

			Scale *= scaleAdjust;
		}
		public virtual void SetAbsoluteScaleTo(double newScale)
		{
			if (Locked)
				return;

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

