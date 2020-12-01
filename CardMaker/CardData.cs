using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CardMaker
{
	public class CardData
	{
		public BindingList<Deck> AllKnownDecks { get; set; }
		public BindingList<Field> AllKnownFields { get; set; }
		public BindingList<Card> AllKnownCards { get; set; }
		public CardData()
		{

		}
		static int numDecksCreated = 0;
		static int numCardsCreated = 0;
		static int numFieldsCreated = 0;
		static CardData()
		{
			GoogleSheets.RegisterSpreadsheetID(Constants.SheetName_DeckData, Constants.SheetId_DeckData);
		}

		Deck GetParentDeck(Card card)
		{
			Deck parentDeck = AllKnownDecks.FirstOrDefault(x => x.ID == card.DeckId);
			if (parentDeck == null)
			{
				parentDeck = new Deck();
				parentDeck.CreateNewId();
				parentDeck.Name = card.DeckId;
				AllKnownDecks.Add(parentDeck);
			}
			return parentDeck;
		}

		void AddCardToDeck(Card card)
		{
			Deck parentDeck = GetParentDeck(card);
			parentDeck.Cards.Add(card);
			card.ParentDeck = parentDeck;
		}

		void AddCardsToDecks()
		{
			foreach (Card card in AllKnownCards)
			{
				AddCardToDeck(card);
			}
		}

		Card GetCard(string iD)
		{
			return AllKnownCards.FirstOrDefault(x => x.ID == iD);
		}

		void AddFieldToCard(Field field)
		{
			Card parentCard = GetCard(field.CardId);
			parentCard?.Fields.Add(field);
			field.ParentCard = parentCard;
		}

		void AddFieldsToCards()
		{
			foreach (Field field in AllKnownFields)
				AddFieldToCard(field);
		}

		public void LoadData()
		{
			AllKnownDecks = LoadData<Deck>();
			AllKnownCards = LoadData<Card>();
			AllKnownFields = LoadData<Field>();

			numDecksCreated = AllKnownDecks.Count;
			numCardsCreated = AllKnownCards.Count;
			numFieldsCreated = AllKnownFields.Count;

			AddFieldsToCards();
			AddCardsToDecks();
			ClearIsDirty();
		}

		private static BindingList<T> LoadData<T>() where T : new()
		{
			return new BindingList<T>(GoogleSheets.Get<T>());
		}

		public void Save()
		{
			GoogleSheets.SaveChanges(AllKnownDecks.Where(x => x.IsDirty).ToArray());
			GoogleSheets.SaveChanges(AllKnownCards.Where(x => x.IsDirty).ToArray());
			GoogleSheets.SaveChanges(AllKnownFields.Where(x => x.IsDirty).ToArray());
			ClearIsDirty();
		}

		private void ClearIsDirty()
		{
			foreach (Deck deck in AllKnownDecks)
				deck.IsDirty = false;

			foreach (Card card in AllKnownCards)
				card.IsDirty = false;

			foreach (Field field in AllKnownFields)
				field.IsDirty = false;
		}

		public void Delete(Deck deck)
		{
			GoogleSheets.DeleteRow(deck);
			AllKnownDecks.Remove(deck);
		}

		public void Delete(Card card)
		{
			GoogleSheets.DeleteRow(card);
			card.ParentDeck?.Cards.Remove(card);
			AllKnownCards.Remove(card);
		}

		public void Delete(Field field)
		{
			GoogleSheets.DeleteRow(field);
			field.ParentCard?.RemoveField(field);
			AllKnownFields.Remove(field);
		}

		public Deck AddDeck()
		{
			Deck deck = new Deck();
			deck.CreateNewId();
			deck.Name = $"Deck {++numDecksCreated}";
			AllKnownDecks.Add(deck);
			return deck;
		}

		public Card AddCard(Deck deck)
		{
			if (deck == null)
				throw new ArgumentNullException("deck");

			Card card = new Card(deck);
			card.Name = $"Card {++numCardsCreated}";
			AllKnownCards.Add(card);
			return card;
		}

		public Field AddField(Card parentCard)
		{
			if (parentCard == null)
				throw new ArgumentNullException("parentCard");

			Field field = new Field(parentCard);
			field.Name = $"Field {++numFieldsCreated}";
			AllKnownFields.Add(field);
			return field;
		}

		public void MoveCardToDeck(Card activeCard, Deck deck)
		{
			activeCard.ParentDeck?.RemoveCard(activeCard);
			activeCard.AddToDeck(deck);
		}
	}
}
