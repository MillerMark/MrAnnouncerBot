using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;

namespace DndMapSpike
{
	public class RandomTexture : BaseTexture
	{
		List<Image> textures = new List<Image>();
		Random random;

		public RandomTexture(string baseName) : base(baseName)
		{
			random = new Random();
		}

		public override Image GetImage(int column, int row)
		{
			int index = random.Next(textures.Count);
			return textures[index];
		}
		public override void AddImage(string fileName, string textureKey)
		{
			base.AddImage(fileName, textureKey);
			textures.Add(CreateImage(fileName));
		}
	}
}

