using System;
using Imaging;
using MapCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

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


		[JsonIgnore]
		Image image;

		public Stamp()
		{
			TypeName = nameof(Stamp);
		}

		public Stamp(string fileName, int x = 0, int y = 0) : this()
		{
			X = x;
			Y = y;
			FileName = fileName;
		}

		
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

		public override void ResetImage()
		{
			image = null;
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

		[JsonIgnore]
		public Image Image
		{
			get
			{
				if (image == null)
					image = ImageUtils.CreateImage(GetAngle(Rotation), HueShift, Saturation, Lightness, Contrast, ScaleX, ScaleY, FileName);

				return image;
			}
		}

		public override bool ContainsPoint(double x, double y)
		{
			int left = GetLeft();
			int top = GetTop();
			if (x < left)
				return false;
			if (y < top)
				return false;

			if (x > left + Width)
				return false;
			if (y > top + Height)
				return false;

			return ImageUtils.HasPixelAt(Image, (int)(x - left), (int)(y - top));
		}

		public override IStampProperties Copy(int deltaX, int deltaY)
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

		/// <summary>
		/// Gets the left of this stamp (X and Y are the center points)
		/// </summary>
		/// <returns></returns>
		public override int GetLeft()
		{
			return (int)Math.Round(X - Width / 2.0);
		}

		public override int Width
		{
			get
			{
				return (int)Math.Round(Image.Source.Width);
			}
			set
			{
				// Do nothing. Width is read-only (from the Image) in Stamp.
			}
		}

		public override int Height
		{
			get
			{
				return (int)Math.Round(Image.Source.Height);
			}
			set
			{
				// Do nothing. Height is read-only (from the Image) in Stamp.
			}
		}

		public override double GetBottom()
		{
			return Y + Height / 2;
		}

		public override double GetRight()
		{
			return X + Width / 2;
		}


		/// <summary>
		/// Gets the top of this group(X and Y are center points)
		/// </summary>
		/// <returns></returns>
		public override int GetTop()
		{
			return (int)Math.Round(Y - Height / 2.0);
		}
		protected override void TransferFrom(SerializedStamp serializedStamp)
		{
			base.TransferFrom(serializedStamp);
		}
		public static Stamp From(SerializedStamp serializedStamp)
		{
			Stamp stamp = new Stamp();
			stamp.TransferFrom(serializedStamp);
			return stamp;
		}
	}
}

