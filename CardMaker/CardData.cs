using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;
using System.Collections.ObjectModel;

namespace CardMaker
{
	public class CardData
	{
		public ObservableCollection<Deck> AllKnownDecks { get; set; }
		public CardData()
		{

		}
		static int numDecksCreated = 0;
		static CardData()
		{
			GoogleSheets.RegisterSpreadsheetID(Constants.SheetName_DeckData, Constants.SheetId_DeckData);
		}

		public void LoadData()
		{
			AllKnownDecks = new ObservableCollection<Deck>(GoogleSheets.Get<Deck>());
		}

		public Deck AddDeck()
		{
			Deck deck = new Deck();
			deck.Name = $"Unnamed{numDecksCreated++}";
			AllKnownDecks.Add(deck);
			return deck;
		}

		public void Save()
		{
			GoogleSheets.SaveChanges(AllKnownDecks.ToArray());
		}
	}
}
