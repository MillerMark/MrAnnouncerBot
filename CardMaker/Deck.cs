using System;
using System.Linq;
using System.Collections.ObjectModel;
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
		
		public ObservableCollection<Card> Cards { get; set; } = new ObservableCollection<Card>();

		public override string ToString()
		{
			return Name;
		}

		public void AddCard(Card card)
		{
			Cards.Add(card);
		}

		public Deck()
		{

		}
	}
}
