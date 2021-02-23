using System;
using System.Linq;

namespace CardMaker
{
	public static class SayAnythingFactory
	{
		public static Card CreateSayAnythingCard(int actualPowerLevel, int basePowerLevel, CardData CardData, Deck ActiveDeck)
		{
			Card card = CardData.AddCard(ActiveDeck);
			CardFactory.SetRarity(card, basePowerLevel, actualPowerLevel);
			string dieName;
			int multiplier;
			GetSayAnythingDieNameAndMultiplier(actualPowerLevel, out multiplier, out dieName);
			int offset = Math.Max(actualPowerLevel / 4 + 1, 1);
			card.Name = $"Say Anything - {multiplier}{dieName} + {offset}";
			card.StylePath = "Say Anything";
			card.Description = $"Make any player, NPC, or monster think or say anything, up to {multiplier}{dieName} times (dice are rolled when you play this card). To make a player *say* something, enter “!{{name}}: \"Your custom message” into the chat room. To make a player *think* something, enter “!{{name}}: (Your custom thoughts” into the chat room. For example: “!Fred: (Yummy...”.";
			card.AdditionalInstructions = "No ads or hate speech - you could get banned!";
			card.AlertMessage = $"{{{{username}}}} played Say Anything - {multiplier}{dieName}+{offset}//!RollDie({multiplier}{dieName}+{offset}, \"Say Anything\")";
			card.CardPlayed = "AddViewerCharge(CardUserName, \"Say Anything\", ViewerDieRollTotal);\n" +
				"TellAll($\"{CardUserName} has Say Anything {ViewerDieRollTotal} times!\");";
			CardFactory.QuickAddAllLayerDetails(card);
			SetSayAnythingLayerVisibilities(card, dieName, multiplier, offset);
			return card;
		}

		static void SetSayAnythingLayerVisibilities(Card card, string dieName, int multiplier, int offset)
		{
			card.SelectAlternateLayer("Die", dieName);
			card.SelectAlternateLayer("Times", $"x{multiplier}");
			card.SelectAlternateLayer("Offset", $"+{offset}");
			int cardNum = CardFactory.random.Next(9);
			card.SelectAlternateLayer("CardBack", $"Card {cardNum}");
		}

		static void GetSayAnythingDieNameAndMultiplier(int powerLevel, out int multiplier, out string dieName)
		{
			const string d4 = "d4";
			const string d6 = "d6";
			const string d8 = "d8";
			const string d10 = "d10";
			multiplier = 1;
			dieName = d4;
			switch (powerLevel)
			{
				case 1: // 2.5
					multiplier = 1;
					dieName = d4;
					break;
				case 2: // 3.5
					multiplier = 1;
					dieName = d6;
					break;
				case 3: // 4.5
					multiplier = 1;
					dieName = d8;
					break;
				case 4: // 5
					multiplier = 2;
					dieName = d4;
					break;
				case 5: // 5.5
					multiplier = 1;
					dieName = d10;
					break;
				case 6: // 7
					multiplier = 2;
					dieName = d6;
					break;
				case 7: // 7.5
					multiplier = 3;
					dieName = d4;
					break;
				case 8: // 9
					multiplier = 2;
					dieName = d8;
					break;
				case 9: // 10
					multiplier = 4;
					dieName = d4;
					break;
				case 10: // 10.5
					multiplier = 3;
					dieName = d6;
					break;
				case 11: // 11
					multiplier = 2;
					dieName = d10;
					break;
				case 12: // 13.5
					multiplier = 3;
					dieName = d8;
					break;
				case 13: // 14
					multiplier = 4;
					dieName = d6;
					break;
				case 14: // 16.5
					multiplier = 3;
					dieName = d10;
					break;
				case 15: // 18
					multiplier = 4;
					dieName = d8;
					break;
				case 16: // 22
					multiplier = 4;
					dieName = d10;
					break;
			}
		}
	}
}

