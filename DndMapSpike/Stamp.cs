using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DndMapSpike
{
	public enum StampRotation
	{
		Zero,
		Ninety,
		OneEighty,
		TwoSeventy
	}
	public class Stamp
	{
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
		int zOrder = -1;

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
					image = CreateImage(GetAngle(Rotation), Scale);
				return image;
			}
		}

		Image CreateImage(int angle, double scale)
		{
			Image image = new Image();

			TransformedBitmap transformBmp = new TransformedBitmap();

			BitmapImage bmpImage = new BitmapImage();

			bmpImage.BeginInit();

			bmpImage.UriSource = new Uri(FileName, UriKind.RelativeOrAbsolute);

			bmpImage.EndInit();

			transformBmp.BeginInit();

			transformBmp.Source = bmpImage;

			RotateTransform rotation = new RotateTransform(angle);
			ScaleTransform scaling = new ScaleTransform(scale, scale);
			TransformGroup group = new TransformGroup();
			group.Children.Add(rotation);
			group.Children.Add(scaling);

			transformBmp.Transform = group;

			transformBmp.EndInit();

			image.Source = transformBmp;

			return image;
		}

		public int X { get; set; }

		public int Y { get; set; }

		public int ZOrder
		{
			get { return zOrder; }
			set
			{
				if (zOrder == value)
				{
					return;
				}

				zOrder = value;
				OnZOrderChanged();
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

		protected virtual void OnZOrderChanged()
		{
			ZOrderChanged?.Invoke(this, EventArgs.Empty);
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

		public int GetTop()
		{
			return (int)Math.Round(Y - Height / 2.0);
		}
		public void RotateRight()
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

		public event EventHandler ZOrderChanged;
	}
}

