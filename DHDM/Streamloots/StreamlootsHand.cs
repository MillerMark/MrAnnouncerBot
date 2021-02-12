using System;
using Streamloots;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class StreamlootsHand
	{
		public int CharacterId { get; set; }

		bool isShown;

		public bool IsShown
		{
			get
			{
				return isShown;
			}
			set
			{
				if (isShown == value)
					return;
				isShown = value;
				if (!isShown)
					SelectedCard = null;
			}
		}
		
		public StreamlootsCard SelectedCard { get; set; }
		public List<StreamlootsCard> Cards { get; set; } = new List<StreamlootsCard>();
		public List<StreamlootsCard> CardsToPlay { get; set; } = new List<StreamlootsCard>();
		public List<StreamlootsCard> CardsToReveal { get; set; } = new List<StreamlootsCard>();
		public int Count => Cards.Count;
		public int HueShift { get; set; }
		public StreamlootsHand()
		{

		}
		public void AddCard(StreamlootsCard card)
		{
			card.Guid = Guid.NewGuid().ToString();
			Cards.Add(card);
		}

		public void PlayCard(StreamlootsCard cardToPlay)
		{
			if (cardToPlay == null)
				return;
			if (cardToPlay.IsSecret)
			{
				HubtasticBaseStation.ShowValidationIssue(CharacterId, DndCore.ValidationAction.Stop, "Cannot play secret cards.");
				return;
			}
			Cards.Remove(cardToPlay);
			CardsToPlay.Add(cardToPlay);
		}

		public void RevealSecretCard(StreamlootsCard cardToReveal)
		{
			if (cardToReveal == null)
				return;
			
			if (!cardToReveal.IsSecret)
				return;

			Cards.Remove(cardToReveal);
			CardsToReveal.Add(cardToReveal);
		}

		List<StreamlootsCard> FindCardsByName(string cardName)
		{
			return Cards.Where(x => x.CardName.StartsWith(cardName)).ToList();
		}

		StreamlootsCard FindCardById(string cardId)
		{
			return Cards.FirstOrDefault(x => x.Guid == cardId);
		}

		public bool RevealSecretCard(string cardId)
		{
			StreamlootsCard card = FindCardById(cardId);
			if (card == null)
				return false;
			Cards.Remove(card);
			CardsToReveal.Add(card);
			if (SelectedCard != null && CardsToReveal.Contains(SelectedCard))
				SelectedCard = null;
			return true;
		}

		public void PlaySelectedCard()
		{
			PlayCard(SelectedCard);
			SelectedCard = null;
		}

		public int GetSelectedCardIndex()
		{
			if (SelectedCard == null)
				return -1;
			for (int i = 0; i < Cards.Count; i++)
				if (Cards[i] == SelectedCard)
				{
					return i;
				}
			return -1;
		}

		public void SelectedCardsHaveBeenPlayed()
		{
			CardsToPlay.Clear();
		}

		public void SecretCardsHaveBeenRevealed()
		{
			CardsToReveal.Clear();
		}

	}
}
