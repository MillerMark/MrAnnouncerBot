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
		public System.Timers.Timer checkInteractionTimer;
		public const int IntAllPlayersId = 10000000;
		const string STR_Cards = "Reveal Secret Cards";
		// TODO: I need a way to save this state when it changes during the game.
		public Dictionary<int, StreamlootsHand> Hands { get; private set; } = new Dictionary<int, StreamlootsHand>();
		Dictionary<int, DateTime> LastInteraction = new Dictionary<int, DateTime>();
		public CardHandManager()
		{
			CreateInteractionTimer();
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
			ShowHand(creatureId);
			StateHasChanged();
		}

		public void ToggleHandVisibility(int creatureId)
		{
			if (!Hands.ContainsKey(creatureId))
			{
				if (IsNpc(creatureId))
					HideAllNpcHands();
			}
			else if (Hands[creatureId].IsShown)
				HideHand(creatureId);
			else if (creatureId < 0)
				HideAllNpcsExcept(creatureId);
			else
				ShowHand(creatureId);

			StateHasChanged();
		}

		private void HideHand(int creatureId)
		{
			Hands[creatureId].IsShown = false;
			LastInteraction.Remove(creatureId);
		}

		void UpdateLastInteraction(int creatureId)
		{
			if (checkInteractionTimer == null) // EnC
				CreateInteractionTimer(); // EnC

			LastInteraction[creatureId] = DateTime.Now;
		}

		private void CreateInteractionTimer()
		{
			checkInteractionTimer = new System.Timers.Timer();
			checkInteractionTimer.Interval = 2000;
			checkInteractionTimer.Elapsed += CheckInteractionTimer_Elapsed;
			checkInteractionTimer.Start();
		}

		private void CheckInteractionTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			checkInteractionTimer.Stop();
			try
			{
				bool anythingHidden = false;
				foreach (int creatureId in LastInteraction.Keys.ToList())
				{
					TimeSpan timeSinceLastInteraction = DateTime.Now - LastInteraction[creatureId];
					const double maxIdleToHideHandSeconds = 6.5d;
					if (timeSinceLastInteraction.TotalSeconds > maxIdleToHideHandSeconds)
					{
						HideHand(creatureId);
						anythingHidden = true;
					}
				}


				if (anythingHidden)
					StateHasChanged();
			}
			finally
			{
				checkInteractionTimer.Start();
			}
		}

		private void ShowHand(int creatureId)
		{
			Hands[creatureId].IsShown = true;
			UpdateLastInteraction(creatureId);
		}

		private void HideAllNpcsExcept(int creatureId)
		{
			ShowHand(creatureId);
			foreach (StreamlootsHand streamlootsHand in Hands.Values)
				if (streamlootsHand.CharacterId < 0)
					if (streamlootsHand.CharacterId != creatureId)
						HideHand(streamlootsHand.CharacterId);
		}

		private static bool IsNpc(int creatureId)
		{
			return creatureId < 0;
		}

		private void HideAllNpcHands()
		{
			foreach (StreamlootsHand streamlootsHand in Hands.Values)
				if (streamlootsHand.CharacterId < 0)
					HideHand(streamlootsHand.CharacterId);
		}

		public void HideAllNpcCards()
		{
			foreach (StreamlootsHand streamlootsHand in Hands.Values)
				if (streamlootsHand.CharacterId < 0)
					HideHand(streamlootsHand.CharacterId);
			StateHasChanged();
		}

		public enum CardDirection
		{
			Next,
			Previous
		}

		public void SelectNextCard(int creatureId)
		{
			SelectCard(creatureId, CardDirection.Next);
		}

		public void SelectPreviousCard(int creatureId)
		{
			SelectCard(creatureId, CardDirection.Previous);
		}

		private void SelectCard(int creatureId, CardDirection direction)
		{
			if (!Hands.ContainsKey(creatureId))
				return;
			StreamlootsHand hand = Hands[creatureId];
			if (hand.Cards.Count == 0)
				return;

			if (!hand.IsShown)
				hand.IsShown = true;

			UpdateLastInteraction(creatureId);

			int selectedCardIndex = hand.GetSelectedCardIndex();

			if (selectedCardIndex == -1)
			{
				hand.SelectedCard = null;
				StreamlootsCard firstCard;
				if (direction == CardDirection.Next)
				{
					firstCard = hand.Cards.First();
					SelectFirstAvailableCard(hand);
				}
				else
				{
					firstCard = hand.Cards.Last();
					SelectLastAvailableCard(hand);
				}
				
				if (hand.SelectedCard == null)  // Only secret cards in the hand.
					hand.SelectedCard = firstCard;
			}
			else
			{
				if (direction == CardDirection.Next)
					selectedCardIndex = GetNextCardIndex(hand, selectedCardIndex);
				else
					selectedCardIndex = SelectPreviousCardIndex(hand, selectedCardIndex);

				hand.SelectedCard = hand.Cards[selectedCardIndex];
			}
			StateHasChanged();
		}

		private static int GetNextCardIndex(StreamlootsHand hand, int selectedCardIndex)
		{
			if (selectedCardIndex < hand.Count - 1)
				selectedCardIndex++;
			else
				selectedCardIndex = 0;
			return selectedCardIndex;
		}

		private static void SelectFirstAvailableCard(StreamlootsHand hand)
		{
			for (int i = 0; i < hand.Cards.Count; i++)
			{
				StreamlootsCard card = hand.Cards[i];
				if (!card.IsSecret)
				{
					hand.SelectedCard = card;
					break;
				}
			}
		}

		private static int SelectPreviousCardIndex(StreamlootsHand hand, int selectedCardIndex)
		{
			if (selectedCardIndex > 0)
				selectedCardIndex--;
			else
				selectedCardIndex = hand.Count - 1;
			return selectedCardIndex;
		}

		private static void SelectLastAvailableCard(StreamlootsHand hand)
		{
			for (int i = hand.Cards.Count - 1; i >= 0; i--)
			{
				StreamlootsCard card = hand.Cards[i];
				if (!card.IsSecret)
				{
					hand.SelectedCard = card;
					break;
				}
			}
		}

		public void PlaySelectedCard(int creatureId, Creature creature)
		{
			if (!Hands.ContainsKey(creatureId))
				return;
			StreamlootsHand hand = Hands[creatureId];
			if (!hand.IsShown)
			{
				hand.IsShown = true;
			}
			if (hand.SelectedCard == null)
			{
				SelectFirstAvailableCard(hand);
				UpdateLastInteraction(creatureId);
				StateHasChanged();
				return;
			}

			UpdateLastInteraction(creatureId);
			hand.PlaySelectedCard(creature);
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
