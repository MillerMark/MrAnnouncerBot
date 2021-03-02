using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

namespace CardMaker
{
	public static class StampedeFactory
	{
		static List<StampedeSpecialist> stampedeSpecialists = new List<StampedeSpecialist>();
		static StampedeFactory()
		{
			stampedeSpecialists = GoogleSheets.Get<StampedeSpecialist>();
		}

		public static List<Card> CreateStampedeCards(int powerLevel, CardData cardData, Deck activeDeck)
		{
			List<Card> result = new List<Card>();
			foreach (StampedeSpecialist stampedeSpecialist in stampedeSpecialists)
			{
				switch (powerLevel)
				{
					case 1:
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Epic, "1d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Legendary, "2d8");
						break;
					case 2:
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Rare, "1d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Epic, "2d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Legendary, "3d8");
						break;
					case 3:
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Common, "1d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Rare, "2d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Epic, "3d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Legendary, "4d8");
						break;
					case 4:
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Common, "2d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Rare, "3d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Epic, "4d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Legendary, "6d8");
						break;
					case 5:
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Common, "3d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Rare, "4d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Epic, "6d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Legendary, "8d8");
						break;
					case 6:
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Common, "4d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Rare, "6d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Epic, "8d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Legendary, "9d8");
						break;
					case 7:
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Common, "6d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Rare, "8d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Epic, "9d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Legendary, "12d8");
						break;
					case 8:
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Common, "8d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Rare, "9d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Epic, "12d8");
						break;
					case 9:
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Common, "9d8");
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Rare, "12d8");
						break;
					case 10:
						AddCardIfMatching(result, cardData, activeDeck, stampedeSpecialist, Rarity.Common, "12d8");
						break;
				}
			}
			return result;
		}

		private static void AddCardIfMatching(List<Card> result, CardData cardData, Deck activeDeck, StampedeSpecialist stampedeSpecialist, Rarity rarity, string diceStrToMatch)
		{
			if (diceStrToMatch == stampedeSpecialist.TotalDamage)
				result.Add(CreateStampedeCard(stampedeSpecialist.CardName, stampedeSpecialist.Description, stampedeSpecialist.AlertMessage, stampedeSpecialist.CardPlayedMessage, stampedeSpecialist.ImageLayerName, stampedeSpecialist.TotalDamage, stampedeSpecialist.DiceDamageStr, rarity, cardData, activeDeck));
		}

		public static Card CreateStampedeCard(string cardName, string description, string alertMessage, string cardPlayedMessage, string imageLayerName, string totalDamage, string damageDiceStr, Rarity rarity, CardData CardData, Deck ActiveDeck)
		{
			Card card = CardData.AddCard(ActiveDeck);
			card.Rarity = rarity;
			card.Name = cardName;
			card.StylePath = "PreMade";
			card.Description = description;
			card.AdditionalInstructions = "Everyone in the path must roll a dexterity saving throw.!";
			card.AlertMessage = alertMessage;
			card.CardPlayed = $"QueueEffect(\"Stampede\", CardUserName, \"{cardName}\", \"{imageLayerName}\", \"{damageDiceStr}\");\n" +
											$"TellAll($\"{cardPlayedMessage}\");\n" +
											$"TellDm(\"caulfielder: Target NPCs, Monsters and Players, and press the Stampede button when ready to roll!\");";
			CardFactory.QuickAddAllLayerDetails(card);
			SetStampedeLayerVisibilities(card, imageLayerName, totalDamage);
			card.Cooldown = 5;
			card.CooldownUnits = CooldownUnits.Minutes;
			return card;
		}

		static void SetStampedeLayerVisibilities(Card card, string imageLayerName, string damageStr)
		{
			card.HideAllLayersExcept(imageLayerName);
			card.SelectAlternateLayer(imageLayerName, damageStr);
			// TODO: Make sure all the other layers are off.
		}
	}
}

