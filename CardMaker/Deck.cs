using System;
using System.Linq;
using GoogleHelper;

namespace CardMaker
{
	[SheetName(Constants.SheetName_DeckData)]
	[TabName("Decks")]
	public class Deck
	{
		[Indexer]
		[Column]
		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}

		public Deck()
		{

		}
	}
}
