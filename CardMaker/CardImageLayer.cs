using Imaging;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace CardMaker
{
	public class CardImageLayer
	{
		public string PngFile { get; set; }
		public int Index { get; set; }

		public string LayerName { get; private set; }
		public CardImageLayer()
		{

		}

		public override string ToString()
		{
			return LayerName;
		}

		public UIElement CreateImage()
		{
			System.Windows.Controls.Image image = ImageUtils.CreateImage(0, 0, 0, 0, 0, 1, 1, PngFile);
			image.Width = 280;
			image.Height = 424;
			return image;
		}

		public CardImageLayer(string pngFile)
		{
			PngFile = pngFile;
			LayerName = Path.GetFileNameWithoutExtension(pngFile);
			int closeBracketPos = LayerName.IndexOf("]");
			if (closeBracketPos > 0)
			{
				string indexStr = LayerName.Substring(1, closeBracketPos - 1);
				LayerName = LayerName.Substring(closeBracketPos + 1).Trim();
				if (int.TryParse(indexStr, out int index))
					Index = index;
			}
		}
	}
}
