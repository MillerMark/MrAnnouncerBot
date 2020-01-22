using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;

namespace DndMapSpike
{
	public class RandomTexture : BaseTexture
	{
		List<Image> textures = new List<Image>();
		List<string> textureFileNames = new List<string>();
		Random random;

		public RandomTexture(string baseName) : base(baseName)
		{
			random = new Random();
		}

		public override Image GetImage(int column, int row, ref string imageFileName)
		{
			if (imageFileName != null)
				return GetImageFromFilename(imageFileName);

			int index = random.Next(textures.Count);
			imageFileName = textureFileNames[index];
			return textures[index];
		}
		public override void AddImage(string fileName, string textureKey)
		{
			base.AddImage(fileName, textureKey);
			textures.Add(CreateImage(fileName));
			textureFileNames.Add(fileName);
		}
	}
}

