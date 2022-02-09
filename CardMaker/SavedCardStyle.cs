using System;
using System.Linq;
using SheetsPersist;

namespace CardMaker
{
	[DocumentName(Constants.DocumentName_DeckData)]
	[SheetName("SavedStyles")]
	public class SavedCardStyle
	{
		[Indexer]
		[Column]
		public string CardName { get; set; }

		[Column]
		public double TextFontHue { get; set; }

		[Column]
		public double TextFontSat { get; set; }

		[Column]
		public double TextFontLight { get; set; }

		[Column]
		public double TextFontOpacity { get; set; }

		[Column]
		public double TextFontSize { get; set; }

		[Column]
		public string Placeholder { get; set; }

		[Column]
		public string LayerMods { get; set; }

		[Column]
		public string StylePath { get; set; }

		public SavedCardStyle()
		{

		}

		public SavedCardStyle(Card card)
		{
			LoadFrom(card);
		}

		public void LoadFrom(Card card)
		{
			CardName = card.Name;
			TextFontHue = card.TextFontHue;
			TextFontSat = card.TextFontSat;
			TextFontLight = card.TextFontLight;
			TextFontOpacity = card.TextFontOpacity;
			TextFontSize = card.TextFontSize;
			Placeholder = card.Placeholder;
			LayerMods = card.LayerMods;
			StylePath = card.StylePath;
		}

		public void ApplyTo(Card card)
		{
			card.TextFontHue = TextFontHue;
			card.TextFontSat = TextFontSat;
			card.TextFontLight = TextFontLight;
			card.TextFontOpacity = TextFontOpacity;
			card.TextFontSize = TextFontSize;
			card.Placeholder = Placeholder;
			card.LayerMods = LayerMods;
			card.StylePath = StylePath;
		}

	}
}
