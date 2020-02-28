using System;
using MapCore;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Imaging;
using Newtonsoft.Json;

namespace DndMapSpike
{
	public class MapCharacter : BaseItemProperties, IFloatingItem
	{
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

		public MapCharacter()
		{
			Guid = Guid.NewGuid();
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

		public MapCharacter(string fileName, double x, double y) : this()
		{
			FileName = fileName;
			X = x;
			Y = y;
		}

		void TransferFrom(SerializedCharacter character)
		{
			X = character.X;
			Y = character.Y;
			Visible = character.Visible;
			FileName = character.FileName;
			Guid = character.Guid;
			Height = character.Height;
			Width = character.Width;
		}
		public static IItemProperties CreateCharacterFrom(SerializedCharacter character)
		{
			MapCharacter mapCharacter = new MapCharacter();
			mapCharacter.TransferFrom(character);
			return mapCharacter;
		}

		public void CreateFloating(Canvas canvas, double left = 0, double top = 0)
		{
			Image image = new Image();
			image.Source = new BitmapImage(new Uri(FileName));

			ScaleTransform scaleTransform = new ScaleTransform();
			scaleTransform.ScaleX = 0.5;
			scaleTransform.ScaleY = 0.5;
			image.LayoutTransform = scaleTransform;
			image.Opacity = 0.5;
			image.IsHitTestVisible = false;
			canvas.Children.Add(image);
			Canvas.SetLeft(image, left);
			Canvas.SetTop(image, top);
		}

		public void BlendStampImage(StampsLayer stampsLayer, double xOffset = 0, double yOffset = 0)
		{
			if (!Visible)
				return;
			stampsLayer.BlendStampImage(this, xOffset, yOffset);
		}

		public MapCharacter(string fileName)
		{
			FileName = fileName;
		}

		Image image;

		[JsonIgnore]
		public Image Image
		{
			get
			{
				if (image == null)
					image = ImageUtils.CreateImage(0, 0, 0, 0, 0, 0.5, 0.5, FileName);

				return image;
			}
		}
	}
}
