using System;
using System.Linq;

namespace CardMaker
{
	public static class DieModFactory
	{
		public const string Skill = "Skill";
		public const string Save = "Save";
		public const string Attack = "Attack";

		static string GetModType(string rollKind)
		{
			switch (rollKind)
			{
				case Skill:
					return "Skill";
				case Save:
					return "Saving";
				case Attack:
					return "Attack";
			}
			return string.Empty;
		}

		static string GetDieModTitle(bool isSecret, string rollKind, int modifier)
		{
			string secret = isSecret ? "Secret " : "";
			string bonusPenaltyText;
			string modStr;
			if (modifier < 0)
			{
				bonusPenaltyText = "Penalty";
				modStr = modifier.ToString();
			}
			else
			{
				bonusPenaltyText = "Bonus";
				modStr = $"+{modifier}";
			}

			string modType = GetModType(rollKind);

			return $"{secret}{modType} {bonusPenaltyText} {modStr}";
		}

		static string GetDieModDescription(string rollKind, int modifier, bool isSecret)
		{
			string bonusVerb = GetBonusVerb(modifier);
			string bonusPenalty = GetBonusPenaltyNoun(modifier);

			string rollDescription = "roll";

			switch (rollKind)
			{
				case Skill:
					rollDescription = "skill check";
					break;
				case Save:
					rollDescription = "saving throw";
					break;
				case Attack:
					rollDescription = "attack";
					break;
			}
			if (isSecret)
				return $"Give this secret card to a player, NPC, or monster. On their next {rollDescription}, the player holding this card will automatically {bonusVerb} the specified {bonusPenalty}. This is a *secret card* and will not be revealed in the game until it is automatically triggered.";
			else
				return $"Give this card to a player, NPC, or monster. When the recipient plays this card later in the game, that player will {bonusVerb} the specified {bonusPenalty} on their {rollDescription}.";
		}

		private static string GetBonusPenaltyNoun(int modifier)
		{
			string bonusPenalty;
			if (modifier < 0)
				bonusPenalty = "penalty";
			else
				bonusPenalty = "bonus";

			return bonusPenalty;
		}

		private static string GetBonusVerb(int modifier)
		{
			string bonusVerb;
			if (modifier < 0)
			{
				bonusVerb = "incur";
			}
			else
			{
				bonusVerb = "gain";
			}

			return bonusVerb;
		}

		static void SetDieModLayerVisibilities(Card card, string rollKind, int modifier, bool isSecret)
		{
			if (modifier < 0)
			{
				card.SelectAlternateLayer("Penalty", (-modifier).ToString());
				card.SelectAlternateLayer("Title", $"Penalty {rollKind}");
				card.SelectAlternateLayer("Icon", "Penalty Skull");
				card.SelectAlternateLayer("Die", "Penalty");
				card.HideAllLayersStartingWith("Bonus -");
			}
			else
			{
				card.SelectAlternateLayer("Bonus", modifier.ToString());
				card.SelectAlternateLayer("Title", $"Bonus {rollKind}");
				card.SelectAlternateLayer("Icon", "Bonus Clover");
				card.SelectAlternateLayer("Die", "Bonus");
				card.HideAllLayersStartingWith("Penalty -");
			}

			if (isSecret)
			{
				card.HideAllLayersStartingWith("Normal Card Text -");
				if (modifier < 0)
					card.SelectAlternateLayer("Secret Text", $"Penalty {rollKind}");
				else
					card.SelectAlternateLayer("Secret Text", $"Bonus {rollKind}");
			}
			else
			{
				card.SelectAlternateLayer("Normal Card Text", rollKind);
				card.HideAllLayersStartingWith("Secret Text -");
				card.HideAllLayersStartingWith("Secret Card");
			}

			int cardNum = CardFactory.random.Next(9);
			card.SelectAlternateLayer("CardBack", $"Card {cardNum}");
		}

		public static Card CreateBonusPenaltyCard(string rollKind, int modifier, int powerLevel, bool isSecret, CardData cardData, Deck parentDeck)
		{
			Card card = cardData.AddCard(parentDeck);
			CardFactory.SetRarity(card, powerLevel, modifier);
			card.Name = GetDieModTitle(isSecret, rollKind, modifier);
			card.StylePath = "Die Mods";
			string cardName;
			string modType = GetModType(rollKind);
			int lastIndexOfSpace = card.Name.LastIndexOf(' ');
			string value = "0";
			if (lastIndexOfSpace > 0)
			{
				value = card.Name.Substring(lastIndexOfSpace).Trim();
				value = value.TrimStart('+');
			}

			if (isSecret)
			{
				cardName = "a secret card";
				card.CardReceived = $"GiveMagic(CardRecipient, \"SecretCardMod\", CardUserName, CardGuid, \"{modType}\", {value});";
			}
			else
			{
				cardName = card.Name;
				card.CardPlayed = $"GiveMagic(CardRecipient, \"ActiveCardMod\", CardUserName, CardGuid, \"{modType}\", {value});";
			}

			card.AlertMessage = $"{{{{username}}}} gave {cardName} to {{{{recipient}}}}.";
			card.Description = GetDieModDescription(rollKind, modifier, isSecret);
			CardFactory.QuickAddAllLayerDetails(card);
			SetDieModLayerVisibilities(card, rollKind, modifier, isSecret);
			string bonusPenalty = GetBonusPenaltyNoun(modifier);
			CardFactory.AddPlayerNpcRecipientField(cardData, card, bonusPenalty);
			return card;
		}
	}
}

