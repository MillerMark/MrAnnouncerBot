using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using DndCore;
using System.Windows.Media;

namespace DHDM
{
	public class CardHandManager
	{
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
			CardDto cardDto = new CardDto();
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
				hand.SelectedCard = hand.Cards[0];
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
	}
}
