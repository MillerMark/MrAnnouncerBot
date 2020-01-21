using System;
using Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DndMapSpike
{
	public class Stamp : BaseStamp, IStamp
	{
		// TODO: Any new writeable properties added need to be copied in the Clone method.

		public void BlendStampImage(StampsLayer stampsLayer, int xOffset = 0, int yOffset = 0)
		{
			if (!Visible)
				return;
			stampsLayer.BlendStampImage(this, xOffset, yOffset);
		}

		public bool FlipHorizontally
		{
			get { return flipHorizontally; }
			set
			{
				if (flipHorizontally == value)
					return;

				image = null;
				flipHorizontally = value;
			}
		}
		public bool FlipVertically
		{
			get { return flipVertically; }
			set
			{
				if (flipVertically == value)
					return;

				image = null;
				flipVertically = value;
			}
		}

		//public int RelativeX { get; set; }
		//public int RelativeY { get; set; }
		bool flipHorizontally;
		bool flipVertically;
		StampRotation rotation;
		public StampRotation Rotation
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
				image = null;
			}
		}

		Image image;

		public Stamp()
		{
		}

		public Stamp(string fileName, int x = 0, int y = 0)
		{
			X = x;
			Y = y;
			FileName = fileName;
		}

		public string FileName { get; set; }
		static Stamp Clone(Stamp stamp)
		{
			Stamp result = new Stamp(stamp.FileName, stamp.X, stamp.Y);
			result.Contrast = stamp.Contrast;
			result.Visible = stamp.Visible;
			result.FlipHorizontally = stamp.FlipHorizontally;
			result.FlipVertically = stamp.FlipVertically;
			result.HueShift = stamp.HueShift;
			result.Lightness = stamp.Lightness;
			result.Rotation = stamp.Rotation;
			result.Saturation = stamp.Saturation;
			result.Scale = stamp.Scale;
			return result;
		}

		int GetAngle(StampRotation rotation)
		{
			switch (rotation)
			{
				case StampRotation.Ninety:
					return 90;
				case StampRotation.OneEighty:
					return 180;
				case StampRotation.TwoSeventy:
					return 270;
			}
			return 0;
		}

		public Image Image
		{
			get
			{
				if (image == null)
					image = ImageUtils.CreateImage(GetAngle(Rotation), HueShift, Saturation, Lightness, Contrast, ScaleX, ScaleY, FileName);

				return image;
			}
		}

		public double ScaleX
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

		public double ScaleY
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

		double scale = 1;
		public double Scale
		{
			get
			{
				return scale;
			}
			set
			{
				if (scale == value)
					return;

				image = null;
				scale = value;
			}
		}

		public bool ContainsPoint(Point point)
		{
			int left = GetLeft();
			int top = GetTop();
			if (point.X < left)
				return false;
			if (point.Y < top)
				return false;

			if (point.X > left + Width)
				return false;
			if (point.Y > top + Height)
				return false;

			return ImageUtils.HasPixelAt(Image, (int)(point.X - left), (int)(point.Y - top));
		}

		/// <summary>
		/// Gets the left of this stamp (X and Y are the center points)
		/// </summary>
		/// <returns></returns>
		public int GetLeft()
		{
			return (int)Math.Round(X - Width / 2.0);
		}

		public int Width
		{
			get
			{
				return (int)Math.Round(Image.Source.Width);
			}
		}

		public int Height
		{
			get
			{
				return (int)Math.Round(Image.Source.Height);
			}
		}

		double hueShift;
		public double HueShift
		{
			get
			{
				return hueShift;
			}
			set
			{
				if (hueShift == value)
					return;

				image = null;
				hueShift = value;
			}
		}

		double saturation;
		public double Saturation
		{
			get
			{
				return saturation;
			}
			set
			{
				if (saturation == value)
					return;

				image = null;
				saturation = value;
			}
		}
		double lightness;
		public double Lightness
		{
			get
			{
				return lightness;
			}
			set
			{
				if (lightness == value)
					return;

				image = null;
				lightness = value;
			}
		}

		double contrast;
		public double Contrast
		{
			get
			{
				return contrast;
			}
			set
			{
				if (contrast == value)
					return;

				image = null;
				contrast = value;
			}
		}

		/// <summary>
		/// Gets the top of this group(X and Y are center points)
		/// </summary>
		/// <returns></returns>
		public int GetTop()
		{
			return (int)Math.Round(Y - Height / 2.0);
		}
		public void RotateRight()
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

		public void RotateLeft()
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

		public void Move(int deltaX, int deltaY)
		{
			X += deltaX;
			Y += deltaY;
		}

		public IStamp Copy(int deltaX, int deltaY)
		{
			Stamp result = Clone(this);
			result.Move(deltaX, deltaY);
			return result;
		}

		public void CreateFloating(Canvas canvas, int left = 0, int top = 0)
		{
			Image image = new Image();
			image.Source = new BitmapImage(new Uri(FileName));
			ScaleTransform scaleTransform = null;
			if (FlipVertically || FlipHorizontally || Scale != 1)
			{
				scaleTransform = new ScaleTransform();
				scaleTransform.ScaleX = ScaleX;
				scaleTransform.ScaleY = ScaleY;
			}
			TransformGroup transformGroup = new TransformGroup();
			RotateTransform rotation = null;
			switch (Rotation)
			{
				case StampRotation.Ninety:
					rotation = new RotateTransform(90);
					break;
				case StampRotation.OneEighty:
					rotation = new RotateTransform(180);
					break;
				case StampRotation.TwoSeventy:
					rotation = new RotateTransform(270);
					break;
			}
			if (scaleTransform != null)
				transformGroup.Children.Add(scaleTransform);

			if (rotation != null)
				transformGroup.Children.Add(rotation);

			if (transformGroup.Children.Count > 0)
				image.LayoutTransform = transformGroup;
			image.Opacity = 0.5;
			image.IsHitTestVisible = false;
			//mouseDragAdjustX = image.Source.Width / 2;
			//mouseDragAdjustY = image.Source.Height / 2;
			canvas.Children.Add(image);
			Canvas.SetLeft(image, left);
			Canvas.SetTop(image, top);
		}
		public void AdjustScale(double scaleAdjust)
		{
			Scale *= scaleAdjust;
		}
		public void SetAbsoluteScaleTo(double newScale)
		{
			Scale = newScale;
		}
		public double GetBottom()
		{
			return Y + Height / 2;
		}

		public double GetRight()
		{
			return X + Width / 2;
		}

	}
}

