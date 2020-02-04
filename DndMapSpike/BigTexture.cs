using System;
using MapCore;
using System.Linq;
using System.Windows.Controls;
using Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace DndMapSpike
{
	public class BigTexture : SimpleTexture
	{

		public BigTexture(string baseName) : base(baseName)
		{

		}

		public override Image GetImage(int column, int row, ref string imageFileName)
		{
			imageFileName = FileName;
			double width = textureImage.Source.Width;
			double height = textureImage.Source.Height;
			int x = column * Tile.Width;
			int y = row * Tile.Height;
			while (x + Tile.Width > width)
				x -= (int)width;
			while (y + Tile.Height > height)
				y -= (int)height;
			if (x < 0)
				x = 0;
			if (y < 0)
				y = 0;
			Image image = new Image();
			const int dpi = 96;
			WriteableBitmap writeableBitmap = new WriteableBitmap(Tile.Width, Tile.Height, dpi, dpi, PixelFormats.Pbgra32, null);
			ImageUtils.CopyImageTo(textureImage, 0, 0, writeableBitmap, Tile.Width, Tile.Height, x, y);
			image.Source = writeableBitmap;
			return image;
		}
	}
}

