using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using DndCore;
using Streamloots;
using System.Windows.Media;

namespace DHDM
{
	public class CardHandManager
	{
		public const int IntAllPlayersId = 10000000;
		const string STR_Cards = "Reveal Secret Cards";
		// TODO: I need a way to save this state when it changes during the game.
		public Dictionary<int, StreamlootsHand> Hands { get; private set; } = new Dictionary<int, StreamlootsHand>();
		public CardHandManager()
		{

		}

		void StateHasChanged()
		{
			SendCardCommand("Update Hands");
		}

		void SendCardCommand(string command)
		{
			CardHandDto cardDto = new CardHandDto();
			cardDto.Hands = Hands.Values.ToList();
			cardDto.Command = command;
			HubtasticBaseStation.CardCommand(JsonConvert.SerializeObject(cardDto));
		}

		int GetHueShift(int creatureId)
		{
			if (creatureId < 0)
			{
				InGameCreature creature = AllInGameCreatures.GetByIndex(-creatureId);
				if (creature == null)
					return 0;


				Color color = (Color)ColorConverter.ConvertFromString(creature.BackgroundHex);
				return (int)color.GetHue();
			}
			else
			{
				Character player = AllPlayers.GetFromId(creatureId);
				if (player == null)
					return 0;
				return player.hueShift;
			}
		}

		public void AddCard(int creatureId, StreamlootsCard card)
		{
			if (!Hands.TryGetValue(creatureId, out StreamlootsHand existingHand))
			{
				existingHand = new StreamlootsHand();
				existingHand.CharacterId = creatureId;
				existingHand.HueShift = GetHueShift(creatureId);
				Hands.Add(creatureId, existingHand);
			}
			existingHand.SelectedCard = null;
			existingHand.AddCard(card);
			existingHand.IsShown = true;
			StateHasChanged();
		}
		public void ToggleHandVisibility(int creatureId)
		{
			if (!Hands.ContainsKey(creatureId))
			{
				if (creatureId < 0)
					foreach (StreamlootsHand streamlootsHand in Hands.Values)
						if (streamlootsHand.CharacterId < 0)
							streamlootsHand.IsShown = false;
			}
			else if (Hands[creatureId].IsShown)
				Hands[creatureId].IsShown = false;
			else if (creatureId < 0)
			{
				foreach (StreamlootsHand streamlootsHand in Hands.Values)
					if (streamlootsHand.CharacterId < 0)
						streamlootsHand.IsShown = streamlootsHand.CharacterId == creatureId;
			}
			else
				Hands[creatureId].IsShown = !Hands[creatureId].IsShown;

			StateHasChanged();
		}
		public void HideAllNpcCards()
		{
			foreach (StreamlootsHand streamlootsHand in Hands.Values)
			{
				if (streamlootsHand.CharacterId < 0)
					streamlootsHand.IsShown = false;
			}
			StateHasChanged();
		}

		public void SelectNextCard(int creatureId)
		{
			if (!Hands.ContainsKey(creatureId))
				return;
			StreamlootsHand hand = Hands[creatureId];
			if (hand.Cards.Count == 0)
				return;

			int selectedCardIndex = hand.GetSelectedCardIndex();
			if (selectedCardIndex == -1)
			{
				hand.SelectedCard = null;
				foreach (StreamlootsCard card in hand.Cards)
				{
					if (!card.IsSecret)
					{
						hand.SelectedCard = card;
						break;
					}
				}
				if (hand.SelectedCard == null)
					hand.SelectedCard = hand.Cards[0];
			}
			else
			{
				if (selectedCardIndex < hand.Count - 1)
					selectedCardIndex++;
				else
					selectedCardIndex = 0;
				hand.SelectedCard = hand.Cards[selectedCardIndex];
			}
			StateHasChanged();
		}

		public void SelectPreviousCard(int creatureId)
		{
			if (!Hands.ContainsKey(creatureId))
				return;
			StreamlootsHand hand = Hands[creatureId];
			if (hand.Cards.Count == 0)
				return;

			int selectedCardIndex = hand.GetSelectedCardIndex();
			if (selectedCardIndex == -1)
			{
				hand.SelectedCard = null;
				for (int i = hand.Cards.Count - 1; i >= 0; i--)
				{
					StreamlootsCard card = hand.Cards[i];
					if (!card.IsSecret)
					{
						hand.SelectedCard = card;
						break;
					}
				}
				if (hand.SelectedCard == null)
					hand.SelectedCard = hand.Cards[hand.Cards.Count - 1];
			}
			else
			{
				if (selectedCardIndex > 0)
					selectedCardIndex--;
				else
					selectedCardIndex = hand.Count - 1;
				hand.SelectedCard = hand.Cards[selectedCardIndex];
			}
			StateHasChanged();
		}

		public void PlaySelectedCard(int creatureId)
		{
			if (!Hands.ContainsKey(creatureId))
				return;
			StreamlootsHand hand = Hands[creatureId];
			if (hand.SelectedCard == null)
				return;

			hand.PlaySelectedCard();
			SendCardCommand("Play Cards");
			hand.SelectedCardsHaveBeenPlayed();
		}

		int updateCount = 0;

		public void BeginUpdate()
		{
			updateCount++;
		}

		public void EndUpdate()
		{
			updateCount--;
			if (updateCount != 0)
				return;

			if (HasCardsToReveal())
			{
				SendCardCommand(STR_Cards);
				foreach (StreamlootsHand hand in Hands.Values)
					hand.SecretCardsHaveBeenRevealed();
			}
		}

		private bool HasCardsToReveal()
		{
			foreach (StreamlootsHand hand in Hands.Values)
				if (hand.CardsToReveal.Count > 0)
					return true;

			return false;
		}

		public bool Updating => updateCount > 0;

		public void RevealSecretCard(int creatureId, string cardId)
		{
			if (!Hands.ContainsKey(creatureId) && creatureId != IntAllPlayersId)
				return;
			if (creatureId == IntAllPlayersId)
			{
				bool haveRevealedAnything = false;
				foreach (StreamlootsHand hand in Hands.Values)
					if (hand.RevealSecretCard(cardId))
						haveRevealedAnything = true;

				if (haveRevealedAnything && !Updating)
				{
					SendCardCommand(STR_Cards);
					foreach (StreamlootsHand hand in Hands.Values)
						hand.SecretCardsHaveBeenRevealed();
				}
			}
			else
			{
				StreamlootsHand hand = Hands[creatureId];
				if (hand.RevealSecretCard(cardId) && !Updating)
				{
					SendCardCommand(STR_Cards);
					hand.SecretCardsHaveBeenRevealed();
				}
			}
		}
	}
}
