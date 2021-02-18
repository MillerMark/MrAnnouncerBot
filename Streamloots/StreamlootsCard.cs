using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace Streamloots
{
	public class StreamlootsCard : ICard
	{
		public bool PngFileFound { get; set; }
		public bool IsSecret { get; set; }
		public string type { get; set; }
		public string message { get; set; }
		public string imageUrl { get; set; }
		public string videoUrl { get; set; }
		public string soundUrl { get; set; }
		public string CardImageName { get; set; }
		public string CardName => data.cardName;
		string overrideUserName = null;
		public string UserName
		{
			get
			{
				if (overrideUserName != null)
					return overrideUserName;
				return data.Username;
			}

			set => overrideUserName = value;
		}
		public string Target => data.Target;
		public string Id => soundUrl;
		public string Recipient => data.Recipient;
		public string Guid { get; set; }

		public string GetField(string fieldName)
		{
			return data.GetField(fieldName);
		}

		public void Initialize()
		{
			const string cardFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\Cards";
			string pngFileName = System.IO.Path.Combine(cardFolder, $"{CardDto.GetFileName(CardName)}.png");
			PngFileFound = System.IO.File.Exists(pngFileName);
			if (PngFileFound)
				CardImageName = System.IO.Path.GetFileNameWithoutExtension(pngFileName);
			else
				System.Diagnostics.Debugger.Break();
			IsSecret = CardName.StartsWith("Secret ");
		}

		public StreamlootsCardData data { get; set; }
		public string FillColor { get; set; }
		public string OutlineColor { get; set; }
	}
}
