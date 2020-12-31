using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class StreamlootsCard
	{
		public bool PngFileFound { get; set; }
		public bool IsSecret { get; set; }
		public string type { get; set; }
		public string message { get; set; }
		public string imageUrl { get; set; }
		public string videoUrl { get; set; }
		public string soundUrl { get; set; }

		public string CardName => data.cardName;
		public string UserName => data.Username;
		public string Target => data.Target;

		public string GetField(string fieldName)
		{
			return data.GetField(fieldName);
		}
		public void Initialize()
		{
			const string cardFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Cards";
			string pngFileName = System.IO.Path.Combine(cardFolder, $"{CardName}.png");
			PngFileFound = System.IO.File.Exists(pngFileName);
			if (!PngFileFound)
				System.Diagnostics.Debugger.Break();
			IsSecret = CardName.StartsWith("Secret ");
		}

		public StreamlootsCardData data { get; set; }
	}
}
