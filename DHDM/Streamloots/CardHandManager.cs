using System;
using System.Linq;
using System.Collections.Generic;

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
			foreach (StreamlootsHand streamlootsHand in Hands.Values)
			{
				streamlootsHand.IsShown = true;
			}
			// TODO: Implement this!
			HubtasticBaseStation.CardCommand("Hands: " + Newtonsoft.Json.JsonConvert.SerializeObject(Hands.Values));
		}

		public void AddCard(int creatureId, StreamlootsCard card)
		{
			if (!Hands.TryGetValue(creatureId, out StreamlootsHand existingHand))
			{
				existingHand = new StreamlootsHand();
				existingHand.CharacterId = creatureId;
				Hands.Add(creatureId, existingHand);
			}
			existingHand.Cards.Add(card);
			StateHasChanged();
		}
		public void ToggleHandVisibility(int creatureId)
		{
			
		}

	}
}
