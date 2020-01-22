using System;
using System.Linq;
using System.Windows.Controls;

namespace DndMapSpike
{
	public class EvenOddTexture : BaseTexture
	{
		Image evenImage;
		Image oddImage;
		string evenImageFileName;
		string oddImageFileName;

		public EvenOddTexture(string baseName) : base(baseName)
		{

		}

		public override void AddImage(string fileName, string textureKey)
		{
			base.AddImage(fileName, textureKey);
			if (textureKey == "_2even")
			{
				evenImageFileName = fileName;
				evenImage = CreateImage(fileName);
			}
			else if (textureKey == "_2odd")
			{
				oddImageFileName = fileName;
				oddImage = CreateImage(fileName);
			}
		}

		public override Image GetImage(int column, int row, ref string imageFileName)
		{
			if (imageFileName != null)
				return GetImageFromFilename(imageFileName);

			bool columnOdd = column % 2 == 1;
			bool rowOdd = row % 2 == 1;
			if (columnOdd)
				if (rowOdd)
				{
					imageFileName = evenImageFileName;
					return evenImage;
				}
				else
				{
					imageFileName = oddImageFileName;
					return oddImage;
				}
			else if (rowOdd)
			{
				imageFileName = oddImageFileName;
				return oddImage;
			}
			else
			{
				imageFileName = evenImageFileName;
				return evenImage;
			}
		}
	}
}

