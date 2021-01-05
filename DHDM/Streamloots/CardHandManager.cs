using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

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
			//Hands.Clear();
			//foreach (StreamlootsHand streamlootsHand in Hands.Values)
			//{
			//	streamlootsHand.IsShown = true;
			//}
			//HubtasticBaseStation.CardCommand("Hands: " + Newtonsoft.Json.JsonConvert.SerializeObject(Hands.Values));
			CardDto cardDto = new CardDto();
			cardDto.Hands = Hands.Values.ToList();
			cardDto.Command = "Update Hands";
			HubtasticBaseStation.CardCommand(JsonConvert.SerializeObject(cardDto));
		}

		public void AddCard(int creatureId, StreamlootsCard card)
		{
			if (!Hands.TryGetValue(creatureId, out StreamlootsHand existingHand))
			{
				existingHand = new StreamlootsHand();
				existingHand.CharacterId = creatureId;
				Hands.Add(creatureId, existingHand);
			}
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
			
		}

		public void PlaySelectedCard(int creatureId)
		{
			
		}

	}
}
