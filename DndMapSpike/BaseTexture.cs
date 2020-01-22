using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace DndMapSpike
{
	public abstract class BaseTexture
	{
		Dictionary<string, Image> images = new Dictionary<string, Image>();
		public string BaseName { get; set; }
		public string FileName { get; set; }
		public abstract Image GetImage(int column, int row, ref string imageFileName);
		public virtual void AddImage(string fileName, string textureKey)
		{
			if (FileName == null)
				FileName = fileName;
		}
		public BaseTexture(string baseName)
		{
			BaseName = baseName;
		}

		public Image GetImageFromFilename(string fileName)
		{
			return images[fileName];
		}

		public Image CreateImage(string fileName)
		{
			Image textureImage = new Image();
			textureImage.Source = new BitmapImage(new Uri(fileName));
			images.Add(fileName, textureImage);
			return textureImage;
		}
	}
}

