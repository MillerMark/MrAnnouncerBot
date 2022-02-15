using System;
using System.Linq;
using System.Collections.ObjectModel;
using SheetsPersist;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CardMaker
{
	[Document(Constants.DocumentName_DeckData)]
	[Sheet("Decks")]
	public class Deck : BaseNameId
	{
		public ObservableCollection<Card> Cards { get; set; } = new ObservableCollection<Card>();


		string setId;

		[Column]
		public string SetId
		{
			get => setId;
			set
			{
				if (setId == value)
					return;
				setId = value;
				OnPropertyChanged();
			}
		}
		

		public override string ToString()
		{
			return Name;
		}

		public void AddCard(Card card)
		{
			IsDirty = true;
			Cards.Add(card);
		}

		public void RemoveCard(Card card)
		{
			Cards.Remove(card);
			card.ParentDeck = null;
			card.DeckId = string.Empty;
			IsDirty = true;
		}

		public Deck()
		{

		}
	}
}
