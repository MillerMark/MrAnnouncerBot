using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MapCore;

namespace DndMapSpike
{
	public class Layer
	{
		//public string Name { get; set; }
		public int WidthPx { get; set; }
		public int HeightPx { get; set; }
		public WriteableBitmap WriteableBitmap { get; private set; }
		public Image Image { get; private set; }
		public int ZIndexOffset { get; set; }
		public Layer()
		{

		}
		public void DrawImageOverTile(Image image, Tile tile)
		{
			ImageUtils.CopyImageTo(image, tile.PixelX, tile.PixelY, WriteableBitmap);
		}

		public void ClearAtTile(Tile tile)
		{
			ImageUtils.ClearRect(tile.PixelX, tile.PixelY, Tile.Width, Tile.Height, WriteableBitmap);
		}

		public void ClearAll()
		{
			ImageUtils.ClearRect(0, 0, WidthPx, HeightPx, WriteableBitmap);
		}

		public void SetSize(int widthPx, int heightPx)
		{
			HeightPx = heightPx;
			WidthPx = widthPx;
			WriteableBitmap = new WriteableBitmap(widthPx, heightPx, 96, 96, PixelFormats.Bgra32, null);
			Image.Source = WriteableBitmap;
		}

		public void AddImageToCanvas(Canvas canvas)
		{
			Image = new Image { IsHitTestVisible = false };
			canvas.Children.Add(Image);
		}
		public void SetZIndex(int count)
		{
			Panel.SetZIndex(Image, count + ZIndexOffset);
		}
	}
}

