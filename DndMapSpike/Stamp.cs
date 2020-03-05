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
	public class Stamp : BaseStamp, IFloatingItem, IStampProperties
	{
		// TODO: Any new writeable properties added need to be copied in the Clone method.

		public void BlendStampImage(StampsLayer stampsLayer, double xOffset = 0, double yOffset = 0)
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

		public Stamp(string fileName, double x = 0, double y = 0) : this()
		{
			X = x;
			Y = y;
			FileName = fileName;
		}

		
		static Stamp Clone(Stamp stamp)
		{
			Stamp result = new Stamp(stamp.FileName, stamp.X, stamp.Y);
			result.TransferProperties(stamp);
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
			double left = GetLeft();
			double top = GetTop();
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

		public override IItemProperties Copy(double deltaX, double deltaY)
		{
			Stamp result = Clone(this);
			result.Move(deltaX, deltaY);
			return result;
		}

		public void CreateFloating(Canvas canvas, double left = 0, double top = 0)
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
			canvas.Children.Add(image);
			Canvas.SetLeft(image, left);
			Canvas.SetTop(image, top);
		}

		/// <summary>
		/// Gets the left of this stamp (X and Y are the center points)
		/// </summary>
		/// <returns></returns>
		public override double GetLeft()
		{
			return X - Width / 2.0;
		}

		public override double Width
		{
			get
			{
				return Image.Source.Width;
			}
			set
			{
				// Do nothing. Width is read-only (from the Image) in Stamp.
			}
		}

		public override double Height
		{
			get
			{
				return Image.Source.Height;
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
		public override double GetTop()
		{
			return Y - Height / 2.0;
		}

		public static Stamp From(SerializedItem serializedStamp)
		{
			Stamp stamp = new Stamp();
			serializedStamp.AssignPropertiesTo(stamp);
			return stamp;
		}
	}
}

