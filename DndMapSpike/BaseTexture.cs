using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DndMapSpike
{
	public abstract class BaseTexture
	{
		public string BaseName { get; set; }
		public string FileName { get; set; }
		public abstract Image GetImage(int column, int row);
		public virtual void AddImage(string fileName, string textureKey)
		{
			if (FileName == null)
				FileName = fileName;
		}
		public BaseTexture(string baseName)
		{
			BaseName = baseName;
		}

		public Image CreateImage(string fileName)
		{
			Image textureImage = new Image();
			textureImage.Source = new BitmapImage(new Uri(fileName));
			return textureImage;
		}
	}
}

