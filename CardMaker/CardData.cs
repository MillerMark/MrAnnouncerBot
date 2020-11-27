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
		static int numCardsCreated = 0;
		static CardData()
		{
			GoogleSheets.RegisterSpreadsheetID(Constants.SheetName_DeckData, Constants.SheetId_DeckData);
		}

		public void LoadData()
		{
			AllKnownDecks = new ObservableCollection<Deck>(GoogleSheets.Get<Deck>());
		}

		public void Save(Deck deck = null)
		{
			GoogleSheets.SaveChanges(AllKnownDecks.ToArray());
			if (deck != null)
				GoogleSheets.SaveChanges(deck.Cards.ToArray());
		}

		public void Delete(Deck deck)
		{
			GoogleSheets.DeleteRow(deck);
			AllKnownDecks.Remove(deck);
		}

		public Deck AddDeck()
		{
			Deck deck = new Deck();
			deck.Name = $"Deck {numDecksCreated++}";
			AllKnownDecks.Add(deck);
			return deck;
		}

		public Card AddCard(Deck deck)
		{
			if (deck == null)
				throw new ArgumentNullException("deck");

			Card card = new Card(deck);
			card.Name = $"Card {numCardsCreated++}";
			return card;
		}
	}
}
