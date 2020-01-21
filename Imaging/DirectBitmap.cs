using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Imaging
{
	public class DirectBitmap : IDisposable
	{
		public Bitmap Bitmap { get; private set; }
		public Int32[] Bits { get; private set; }
		public byte[] Bytes { get; private set; }
		public bool Disposed { get; private set; }
		public int Height { get; private set; }
		public int Width { get; private set; }

		protected GCHandle BitsHandle { get; private set; }

		public DirectBitmap(int width, int height)
		{
			Width = width;
			Height = height;
			Bits = new Int32[width * height];
			BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
			Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
		}

		public static DirectBitmap FromFile(string file)
		{
			Image image; // = Image.FromFile(file);
			using (var bmpTemp = new Bitmap(file))
			{
				image = new Bitmap(bmpTemp);
			}

			DirectBitmap directBitmap = new DirectBitmap(image.Width, image.Height);
			Graphics graphics = Graphics.FromImage(directBitmap.Bitmap);
			graphics.DrawImage(image, new Point(0, 0));
			return directBitmap;
		}

		public void SetPixel(int x, int y, Color color)
		{
			int index = x + (y * Width);
			int col = color.ToArgb();

			Bits[index] = col;
		}

		public Color GetPixel(int x, int y)
		{
			int index = x + (y * Width);
			int col = Bits[index];
			Color result = Color.FromArgb(col);

			return result;
		}

		public void Dispose()
		{
			if (Disposed) return;
			Disposed = true;
			Bitmap.Dispose();
			BitsHandle.Free();
		}
	}
}
