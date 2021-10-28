using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Resources;
using System.Collections.Generic;

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
			int offset = 0;
			if (y < 0)
			{
				int yAdjust = -y;
				y = 0;
				cropHeight -= yAdjust;
			}
			target.CopyPixels(new Int32Rect(x, y, cropWidth, cropHeight), target_buffer, dest_stride, offset);

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

		/// <summary>
		/// Creates an Image with specified adjustments based on an image file on disk.
		/// </summary>
		/// <param name="angle">The angle to rotate the image. One of 0, 90, 180, or 270.</param>
		/// <param name="hueShift">The degrees to rotate the hue.</param>
		/// <param name="saturationAdjust">A value between -100 and 100. Negative values reduce saturation; positive values increase saturation.</param>
		/// <param name="lightnessAdjust">A value between -100 and 100. Negative values darken the color; positive values lighten the color.</param>
		/// <param name="contrast">A value between -100 and 100. Negative values reduce contrast (move more toward the threshold value); positive values increase the contrast (move away from the threshold value).</param>
		/// <param name="scaleX">The horizontal scale of the image.</param>
		/// <param name="scaleY">The vertical scale of the image.</param>
		/// <param name="fileName">The image file to load.</param>
		/// <returns></returns>
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
				int stride = firstFrame.PixelWidth * bytesPerPixel;
				byte[] pixels = new byte[stride * firstFrame.PixelHeight];
				firstFrame.CopyPixels(pixels, stride, 0);

				for (int i = 0; i < pixels.Length / 4; ++i)
				{
					int index = i * 4;
					const int blueOffset = 0;
					const int greenOffset = 1;
					const int redOffset = 2;
					const int alphaOffset = 3;

					// Diagnostic pixel testing:
					//int y = i / firstFrame.PixelWidth;
					//int x = i - y * firstFrame.PixelWidth;
					//if (x == 229 && y == 254)
					//{
					//	System.Diagnostics.Debugger.Break();
					//}
					byte b = pixels[index + blueOffset];
					byte g = pixels[index + greenOffset];
					byte r = pixels[index + redOffset];
					byte a = pixels[index + alphaOffset];

					if (a == 0)
						continue;

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

				bitmapSource = CreateBitmap(firstFrame, stride, pixels);
			}

			transformBmp.BeginInit();
			transformBmp.Source = bitmapSource;
			RotateTransform rotation = null;
			if (angle != 0)
				rotation = new RotateTransform(angle);

			ScaleTransform scaling = null;
			
			if (scaleX != 1 || scaleY != 1)
				scaling = new ScaleTransform(scaleX, scaleY);

			if (scaling != null || rotation != null)
			{
				TransformGroup group = new TransformGroup();
				if (scaling != null)
					group.Children.Add(scaling);
				
				if (rotation != null)
					group.Children.Add(rotation);

				transformBmp.Transform = group;
			}
			transformBmp.EndInit();

			Image result = new Image();

			result.Source = transformBmp;

			return result;
		}

		private static BitmapSource CreateBitmap(BitmapFrame firstFrame, int stride, byte[] pixels)
		{
			//! Important - PixelFormats.Bgra32 format preserves the alpha and rgb values. PixelFormats.Pbgra32 turns rgb values to white when alpha values are low.
			WriteableBitmap writeableBitmap = new WriteableBitmap(firstFrame.PixelWidth, firstFrame.PixelHeight, firstFrame.DpiX, firstFrame.DpiY, PixelFormats.Bgra32, null);
			writeableBitmap.WritePixels(new Int32Rect(0, 0, firstFrame.PixelWidth, firstFrame.PixelHeight), pixels, stride, 0);
			return writeableBitmap;
		}

		public static Size GetImageSize(string file)
		{
			using (var imageStream = File.OpenRead(file))
			{
				var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
				BitmapFrame firstFrame = decoder.Frames[0];
				return new Size(firstFrame.PixelWidth, firstFrame.PixelHeight);
			}
		}

		public static void GetImageSize(string file, out double width, out double height)
		{
			Size imageSize = GetImageSize(file);
			width = imageSize.Width;
			height = imageSize.Height;
		}

		/// <summary>
		/// Takes a file name (e.g., "Mark002.png"), and returns all files matching ("Mark*.png") 
		/// as well as the base name and the folder name.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="directoryName"></param>
		/// <param name="rootName"></param>
		/// <param name="files"></param>
		public static void GetFilesToAnalyze(string fileName, out string directoryName, out string rootName, out List<string> files)
		{
			directoryName = System.IO.Path.GetDirectoryName(fileName);
			string baseFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
			char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
			rootName = baseFileName.TrimEnd(numbers);
			string searchPattern = $"{rootName}*.png";
			files = System.IO.Directory.GetFiles(directoryName, searchPattern).OrderBy(x => x).ToList();
		}

		public static ObsTransform GetVisualProcessingResults(string fileName)
		{
			ObsTransform results = new ObsTransform();
			IntermediateResults intermediateResults = new IntermediateResults();

			using (DirectBitmap bitmap = DirectBitmap.FromFile(fileName))
			{
				int line = 0;
				int bitmapWidth = bitmap.Width;
				int bitmapHeight = bitmap.Height;
				int xOffset = -(bitmapWidth - 1920) / 2;
				int yOffset = -(bitmapHeight - 1080) / 2;

				while (line < bitmapHeight)
				{
					ProcessLine(bitmap, line, intermediateResults, xOffset, yOffset);
					line++;
				}
				//intermediateResults.FindCircleCenters(bitmap);
			}
			
			results.Calculate(intermediateResults);
			return results;
		}

		static bool AreClose(byte r, byte g)
		{
			return Math.Abs(r - g) < 30;
		}

		private static void ProcessLine(DirectBitmap bitmap, int y, IntermediateResults intermediateResults, int xOffset, int yOffset)
		{
			// We have to detect four colors: red, blue, green, yellow (means use profile camera)
			for (int x = 0; x < bitmap.Width; x++)
			{
				System.Drawing.Color pixel = bitmap.GetPixel(x, y);
				//if (pixel.A < 5)
				//	continue;
				if (pixel.A > intermediateResults.GreatestOpacity)
					intermediateResults.GreatestOpacity = pixel.A;

				if (pixel.R > 0) // Could be red or yellow.
				{
					if (pixel.G > 0 && AreClose(pixel.R, pixel.G))
					{
						intermediateResults.Yellow.Add(x + xOffset, y + yOffset, pixel.A);
						continue;
					}
					intermediateResults.Red.Add(x + xOffset, y + yOffset, pixel.A);
					continue;
				}
				if (pixel.G > 0)
				{
					intermediateResults.Green.Add(x + xOffset, y + yOffset, pixel.A);
					continue;
				}
				if (pixel.B > 0)
				{
					intermediateResults.Blue.Add(x + xOffset, y + yOffset, pixel.A);
					continue;
				}
			}
		}
		
	}
}

