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

		string topLeftImageFileName;
		string topRightImageFileName;
		string bottomLeftImageFileName;
		string bottomRightImageFileName;

		public FourPartTexture(string baseName) : base(baseName)
		{

		}

		public override void AddImage(string fileName, string textureKey)
		{
			base.AddImage(fileName, textureKey);
			if (textureKey == "_4bl")
			{
				bottomLeftImageFileName = fileName;
				bottomLeftImage = CreateImage(fileName);
			}
			else if (textureKey == "_4tl")
			{
				topLeftImageFileName = fileName;
				topLeftImage = CreateImage(fileName);
			}
			else if (textureKey == "_4tr")
			{
				topRightImageFileName = fileName;
				topRightImage = CreateImage(fileName);
			}
			else if (textureKey == "_4br")
			{
				bottomRightImageFileName = fileName;
				bottomRightImage = CreateImage(fileName);
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
					imageFileName = bottomRightImageFileName;
					return bottomRightImage;
				}
				else
				{
					imageFileName = topRightImageFileName;
					return topRightImage;
				}
			else if (rowOdd)
			{
				imageFileName = bottomLeftImageFileName;
				return bottomLeftImage;
			}
			else
			{
				imageFileName = topLeftImageFileName;
				return topLeftImage;
			}
		}
	}
}

