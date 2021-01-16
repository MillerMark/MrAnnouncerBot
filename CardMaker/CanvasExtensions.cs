using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CardMaker
{
	public static class CanvasExtensions
	{
		public static void SaveToPng(this Canvas surface, Uri path)
		{
			if (path == null) return;

			// Save current canvas transform
			Transform transform = surface.LayoutTransform;
			// reset current transform (in case it is scaled or rotated)
			surface.LayoutTransform = null;

			// Get the size of canvas
			Size size = new Size(surface.Width, surface.Height);
			// Measure and arrange the surface
			// VERY IMPORTANT
			surface.Measure(size);
			surface.Arrange(new Rect(size));

			// Create a render bitmap and push the surface to it
			RenderTargetBitmap renderBitmap =
				new RenderTargetBitmap(
					(int)size.Width,
					(int)size.Height,
					96d,
					96d,
					PixelFormats.Pbgra32);
			renderBitmap.Render(surface);

			// Create a file stream for saving image
			using (FileStream outStream = new FileStream(path.LocalPath, FileMode.Create))
			{
				// Use png encoder for our data
				PngBitmapEncoder encoder = new PngBitmapEncoder();
				// push the rendered bitmap to it
				encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
				// save the data to the stream
				encoder.Save(outStream);
			}

			// Restore previously saved layout
			surface.LayoutTransform = transform;
		}
	}
}
