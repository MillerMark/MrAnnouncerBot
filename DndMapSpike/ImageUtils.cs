using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

namespace DndMapSpike
{
	public static class ImageUtils
	{
		public static void CopyImageTo(Image sourceImage, int x, int y, WriteableBitmap target, int cropWidth = -1, int cropHeight = -1)
		{
			BitmapSource source = sourceImage.Source as BitmapSource;

			PixelFormat pixelFormat = source.Format;
			int pixelWidth = source.PixelWidth;
			int pixelHeight = source.PixelHeight;
			WritePixelsTo(source, x, y, pixelWidth, pixelHeight, pixelFormat, target, cropWidth, cropHeight);
		}

		public static void ClearRect(int x, int y, int width, int height, WriteableBitmap target)
		{
			WritePixelsTo(null, x, y, width, height, target.Format, target);
		}

		private static void WritePixelsTo(BitmapSource source, int x, int y, int pixelWidth, int pixelHeight, PixelFormat pixelFormat, WriteableBitmap target, int cropWidth = -1, int cropHeight = -1)
		{
			int sourceBytesPerPixel = GetBytesPerPixel(pixelFormat);
			int sourceBytesPerLine = pixelWidth * sourceBytesPerPixel;

			if (y < 0)
				return;

			byte[] sourcePixels = new byte[sourceBytesPerLine * pixelHeight];

			if (source != null)
				source.CopyPixels(sourcePixels, sourceBytesPerLine, 0);


			//for (int imageY = 0; imageY < pixelHeight; y++)
			//	for (int imageX = 0; imageX < pixelWidth; x++)
			//	{
			//		int sourcePixelIndex = sourceBytesPerLine * imageY + imageX;

			//		byte alpha = sourcePixels[sourcePixelIndex];
			//		byte blue = sourcePixels[sourcePixelIndex + 1];
			//		byte green = sourcePixels[sourcePixelIndex + 2];
			//		byte red = sourcePixels[sourcePixelIndex + 3];

			//		sourcePixels[sourcePixelIndex] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
			//		if (target.BackBuffer)
			//	}


			if (cropWidth == -1)
				cropWidth = pixelWidth;

			if (cropHeight == -1)
				cropHeight = pixelHeight;
			Int32Rect sourceRect = new Int32Rect(x, y, cropWidth, cropHeight);
			target.WritePixels(sourceRect, sourcePixels, sourceBytesPerLine, 0);
		}

		public static int GetBytesPerPixel(PixelFormat format)
		{
			return format.BitsPerPixel / 8;
		}

		public static void SaveImage(string path, string fileName, Image image)
		{
			string filePath = Path.Combine(path, fileName);

			var encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
			using (FileStream stream = new FileStream(filePath, FileMode.Create))
				encoder.Save(stream);
		}
	}
}

