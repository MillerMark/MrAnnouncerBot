using System;
using System.Linq;
using System.Windows.Controls;

namespace DndMapSpike
{
	public class EvenOddTexture : BaseTexture
	{
		Image evenImage;
		Image oddImage;

		public EvenOddTexture(string baseName) : base(baseName)
		{

		}

		public override void AddImage(string fileName, string textureKey)
		{
			base.AddImage(fileName, textureKey);
			if (textureKey == "_2even")
				evenImage = CreateImage(fileName);
			else if (textureKey == "_2odd")
				oddImage = CreateImage(fileName);
		}

		public override Image GetImage(int column, int row)
		{
			bool columnOdd = column % 2 == 1;
			bool rowOdd = row % 2 == 1;
			if (columnOdd)
				if (rowOdd)
					return evenImage;
				else
					return oddImage;
			else if (rowOdd)
				return oddImage;
			else
				return evenImage;
		}
	}
}

