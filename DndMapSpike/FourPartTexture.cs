using System;
using System.Linq;
using System.Windows.Controls;

namespace DndMapSpike
{
	public class FourPartTexture : BaseTexture
	{
		Image topLeftImage;
		Image topRightImage;
		Image bottomLeftImage;
		Image bottomRightImage;

		public FourPartTexture(string baseName) : base(baseName)
		{

		}

		public override void AddImage(string fileName, string textureKey)
		{
			base.AddImage(fileName, textureKey);
			if (textureKey == "_4bl")
				bottomLeftImage = CreateImage(fileName);
			else if (textureKey == "_4tl")
				topLeftImage = CreateImage(fileName);
			else if (textureKey == "_4tr")
				topRightImage = CreateImage(fileName);
			else if (textureKey == "_4br")
				bottomRightImage = CreateImage(fileName);
		}

		public override Image GetImage(int column, int row)
		{
			bool columnOdd = column % 2 == 1;
			bool rowOdd = row % 2 == 1;
			if (columnOdd)
				if (rowOdd)
					return bottomRightImage;
				else
					return topRightImage;
			else if (rowOdd)
				return bottomLeftImage;
			else
				return topLeftImage;
		}
	}
}

