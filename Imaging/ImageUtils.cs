using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Resources;

namespace Imaging
{
	public static class ImageUtils
	{
		public static void CopyImageTo(Image sourceImage, int x, int y, WriteableBitmap target, int cropWidth = -1, int cropHeight = -1, int sourceX = 0, int sourceY = 0)
		{
			BitmapSource source = sourceImage.Source as BitmapSource;

			PixelFormat pixelFormat = source.Format;
			int pixelWidth = source.PixelWidth;
			int pixelHeight = source.PixelHeight;
			WritePixelsTo(source, x, y, pixelWidth, pixelHeight, pixelFormat, target, cropWidth, cropHeight, sourceX, sourceY);
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

		private static void WritePixelsTo(BitmapSource source, int destinationX, int destinationY, int pixelWidth, int pixelHeight, PixelFormat pixelFormat, WriteableBitmap target, int cropWidth = -1, int cropHeight = -1, int sourceX = 0, int sourceY = 0)
		{
			int sourceBytesPerPixel = GetBytesPerPixel(pixelFormat);
			int sourceBytesPerLine = pixelWidth * sourceBytesPerPixel;

			if (destinationY < 0)
				return;

			byte[] sourcePixels = new byte[sourceBytesPerLine * pixelHeight];

			if (source != null)
				source.CopyPixels(sourcePixels, sourceBytesPerLine, 0);

			if (cropWidth == -1)
				cropWidth = pixelWidth;

			if (cropHeight == -1)
				cropHeight = pixelHeight;
			int targetWidth = cropWidth;
			if (destinationX + targetWidth > target.PixelWidth)
				targetWidth = target.PixelWidth - destinationX;
			int targetHeight = cropHeight;
			if (destinationY + targetHeight > target.PixelHeight)
				targetHeight = target.PixelHeight - destinationY;
			Int32Rect targetRect = new Int32Rect(destinationX, destinationY, targetWidth, targetHeight);
			int offset = sourceBytesPerPixel * sourceX + sourceY * sourceBytesPerLine;
			target.WritePixels(targetRect, sourcePixels, sourceBytesPerLine, offset);
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

		public static Image CreateImage(int angle, double hueShift, double saturationAdjust, double lightnessAdjust, double contrast, double scaleX, double scaleY, string fileName)
		{
			TransformedBitmap transformBmp = new TransformedBitmap();

			BitmapSource bitmapSource;

			if (hueShift == 0 && saturationAdjust == 0 && lightnessAdjust == 0 && contrast == 0)
			{
				BitmapImage bmpImage = new BitmapImage();
				bmpImage.BeginInit();
				bmpImage.UriSource = new Uri(fileName, UriKind.RelativeOrAbsolute);
				bmpImage.EndInit();
				bitmapSource = bmpImage;
			}
			else
			{
				BitmapDecoder dec = BitmapDecoder.Create(new Uri(fileName), BitmapCreateOptions.None, BitmapCacheOption.Default);
				BitmapFrame firstFrame = dec.Frames[0];
				const int bytesPerPixel = 4;
				byte[] pixels = new byte[firstFrame.PixelWidth * firstFrame.PixelHeight * bytesPerPixel];
				firstFrame.CopyPixels(pixels, firstFrame.PixelWidth * bytesPerPixel, 0);

				for (int i = 0; i < pixels.Length / 4; ++i)
				{
					int index = i * 4;
					const int blueOffset = 0;
					const int greenOffset = 1;
					const int redOffset = 2;
					const int alphaOffset = 3;
					
					byte b = pixels[index + blueOffset];
					byte g = pixels[index + greenOffset];
					byte r = pixels[index + redOffset];
					byte a = pixels[index + alphaOffset];
					HueSatLight hueSatLight = new HueSatLight(Color.FromArgb(a, r, g, b));
					if (hueShift != 0)
						hueSatLight.HueShiftDegrees(hueShift);
					if (saturationAdjust != 0)
						hueSatLight.AdjustSaturation(saturationAdjust);
					if (lightnessAdjust != 0)
						hueSatLight.AdjustLightness(lightnessAdjust);

					if (contrast != 0)
						hueSatLight.AdjustContrast(contrast, (lightnessAdjust + 100) / 200);
					Color shifted = hueSatLight.AsRGB;
					pixels[index + blueOffset] = shifted.B;
					pixels[index + greenOffset] = shifted.G;
					pixels[index + redOffset] = shifted.R;
					pixels[index + alphaOffset] = shifted.A;
				}

				// Write the modified pixels into a new bitmap and set that to the source
				var writeableBitmap = new WriteableBitmap(firstFrame.PixelWidth, firstFrame.PixelHeight, firstFrame.DpiX, firstFrame.DpiY, PixelFormats.Pbgra32, null);
				writeableBitmap.WritePixels(new Int32Rect(0, 0, firstFrame.PixelWidth, firstFrame.PixelHeight), pixels, firstFrame.PixelWidth * bytesPerPixel, 0);
				bitmapSource = writeableBitmap;
			}

			transformBmp.BeginInit();
			transformBmp.Source = bitmapSource;
			RotateTransform rotation = new RotateTransform(angle);

			ScaleTransform scaling = new ScaleTransform(scaleX, scaleY);
			TransformGroup group = new TransformGroup();
			group.Children.Add(scaling);
			group.Children.Add(rotation);
			transformBmp.Transform = group;
			transformBmp.EndInit();

			Image result = new Image();

			result.Source = transformBmp;

			return result;
		}
	}
}

