using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

namespace Imaging
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

		public static void MergeImageWith(Image sourceImage, int x, int y, WriteableBitmap target, int cropWidth = -1, int cropHeight = -1)
		{
			BitmapSource source = sourceImage.Source as BitmapSource;
			MergePixelsWith(source, x, y, target, cropWidth, cropHeight);
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

		public static void MergePixelsWith(BitmapSource source, int x, int y, WriteableBitmap target, int cropWidth = -1, int cropHeight = -1)
		{
			if (cropWidth == -1)
				cropWidth = source.PixelWidth;

			if (cropHeight == -1)
				cropHeight = source.PixelHeight;

			// copy the source image into a byte buffer...
			int src_stride = source.PixelWidth * (source.Format.BitsPerPixel >> 3);
			byte[] src_buffer = new byte[src_stride * source.PixelHeight];
			source.CopyPixels(src_buffer, src_stride, 0);

			// copy the dest image into a byte buffer
			int dest_stride = source.PixelWidth * (target.Format.BitsPerPixel >> 3);
			byte[] target_buffer = new byte[(source.PixelWidth * source.PixelHeight) << 2];
			target.CopyPixels(new Int32Rect(x, y, cropWidth, cropHeight), target_buffer, dest_stride, 0);

			// merge
			for (int i = 0; i < src_buffer.Length; i = i + 4)
			{
				byte alphaSource = src_buffer[i + 3];
				byte blueSource = src_buffer[i + 2];
				byte greenSource = src_buffer[i + 1];
				byte redSource = src_buffer[i + 0];

				byte alphaTarget = target_buffer[i + 3];
				byte blueTarget = target_buffer[i + 2];
				byte greenTarget = target_buffer[i + 1];
				byte redTarget = target_buffer[i + 0];

				const int Square255 = 255 * 255;
				int inverseAlphaSource = 255 - alphaSource;

				byte rOut = (byte)((redSource * alphaSource / 255) + (redTarget * alphaTarget * inverseAlphaSource / Square255));
				byte gOut = (byte)((greenSource * alphaSource / 255) + (greenTarget * alphaTarget * inverseAlphaSource / Square255));
				byte bOut = (byte)((blueSource * alphaSource / 255) + (blueTarget * alphaTarget * inverseAlphaSource / Square255));
				byte aOut = (byte)(alphaSource + (alphaTarget * (255 - alphaSource) / 255));

				target_buffer[i + 0] = rOut;
				target_buffer[i + 1] = gOut;
				target_buffer[i + 2] = bOut;
				target_buffer[i + 3] = aOut;
			}


			Int32Rect sourceRect = new Int32Rect(x, y, cropWidth, cropHeight);

			target.WritePixels(sourceRect, target_buffer, dest_stride, 0);
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

		public static Color GetPixelColor(BitmapSource source, int x, int y)
		{
			Color result = Colors.Transparent;
			if (source == null)
				return result;

			try
			{
				CroppedBitmap croppedBitmap = new CroppedBitmap(source, new Int32Rect(x, y, 1, 1));
				const int bytesPerPixel = 4;
				var pixels = new byte[bytesPerPixel];
				croppedBitmap.CopyPixels(pixels, bytesPerPixel, 0);
				result = Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
			}
			catch (Exception) {
			}
			return result;
		}

		public static bool HasPixelAt(Image image, int x, int y)
		{
			Color pixelColor = GetPixelColor(image.Source as BitmapSource, x, y);
			return pixelColor.A > 128;
		}
		public static Image CreateImage(int angle, double scaleX, double scaleY, string fileName)
		{
			Image image = new Image();

			TransformedBitmap transformBmp = new TransformedBitmap();

			BitmapImage bmpImage = new BitmapImage();

			bmpImage.BeginInit();

			bmpImage.UriSource = new Uri(fileName, UriKind.RelativeOrAbsolute);

			bmpImage.EndInit();

			transformBmp.BeginInit();

			transformBmp.Source = bmpImage;

			RotateTransform rotation = new RotateTransform(angle);
			ScaleTransform scaling = new ScaleTransform(scaleX, scaleY);
			TransformGroup group = new TransformGroup();
			group.Children.Add(rotation);
			group.Children.Add(scaling);

			transformBmp.Transform = group;

			transformBmp.EndInit();

			image.Source = transformBmp;

			return image;
		}
	}
}

