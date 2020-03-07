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
		[Editable]
		public string Name { get; set; }

		[DisplayText("HP:")]
		[Precision(0)]
		public virtual double HitPoints { get; set; }

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
			TypeName = nameof(MapCharacter);
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

		public virtual void CreateFloating(Canvas canvas, double left = 0, double top = 0)
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

		public virtual void BlendStampImage(StampsLayer stampsLayer, double xOffset = 0, double yOffset = 0)
		{
			if (!Visible)
				return;
			stampsLayer.BlendStampImage(this, xOffset, yOffset);
		}

		public override void GetPropertiesFrom(IItemProperties itemProperties, bool transferGuid = true)
		{
			base.GetPropertiesFrom(itemProperties, transferGuid);
			if (itemProperties is MapCharacter mapCharacter)
				this.GetPropertiesFrom<MapCharacter>(mapCharacter);
		}

		static MapCharacter Clone(MapCharacter character)
		{
			MapCharacter result = new MapCharacter(character.FileName, character.X, character.Y);
			result.GetPropertiesFrom(character, false);
			return result;
		}

		public override IItemProperties Copy(double deltaX, double deltaY)
		{
			MapCharacter result = Clone(this);
			result.Move(deltaX, deltaY);
			return result;
		}
		public static IItemProperties From(SerializedItem stamp)
		{
			MapCharacter mapCharacter = new MapCharacter();
			stamp.AssignPropertiesTo(mapCharacter);
			return mapCharacter;
		}

		public MapCharacter(string fileName) : this()
		{
			FileName = fileName;
		}

		Image image;

		[JsonIgnore]
		public virtual Image Image
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
