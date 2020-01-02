using System;
using System.Linq;

namespace DndMapSpike
{
	public static class TextureUtils
	{
		public const string TileFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Maps\Tiles\120x120";
		public const string DoorsFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Maps\Doors";
		public const string DebrisFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Maps\Tiles\120x120\Debris";
		public const string WallFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Maps\Walls";

		public static void GetTextureNameAndKey(string fileName, out string baseName, out string textureName, out string key)
		{
			baseName = System.IO.Path.GetFileNameWithoutExtension(fileName);
			int lastUnderscorePos = baseName.LastIndexOf('_');
			key = "";
			textureName = "Single";
			if (lastUnderscorePos >= 0)
			{
				key = baseName.Substring(lastUnderscorePos);
				baseName = baseName.Substring(0, lastUnderscorePos);
			}
			if (key == "_4bl" || key == "_4tl" || key == "_4tr" || key == "_4br")
				textureName = "Four part";
			else if (key.StartsWith("_rnd"))
				textureName = "Random";
			else if (key == "_2even" || key == "_2odd")
				textureName = "Even Odd";
		}
		public static BaseTexture CreateTexture(string baseName, string textureName)
		{
			switch (textureName)
			{
				case "Four part":
					return new FourPartTexture(baseName);
				case "Random":
					return new RandomTexture(baseName);
				case "Even Odd":
					return new EvenOddTexture(baseName);
			}
			return new SimpleTexture(baseName); ;
		}
	}
}

