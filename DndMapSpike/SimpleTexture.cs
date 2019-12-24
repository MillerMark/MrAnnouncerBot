using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace DndMapSpike
{
	public class SimpleTexture: BaseTexture
	{
		Image textureImage;
		public SimpleTexture(string baseName) : base(baseName)
		{

		}

		public override Image GetImage(int column, int row)
		{
			return textureImage;
		}
		public override void AddImage(string fileName, string textureKey)
		{
			base.AddImage(fileName, textureKey);
			textureImage = CreateImage(fileName);
		}
	}
}

