using System;
using System.IO;
using System.Linq;
using DndCore;

namespace CardMaker
{
	public static class SpellFactory
	{
		private const string userName = "{{username}}";
		private const string target = "{{target}}";
		private const string recipient = "{{recipient}}";

		private static Card CreateSpellCard(string actionStr, SpellDto spellDto, CardData CardData, Deck ActiveDeck)
		{
			Card card = CardData.AddCard(ActiveDeck);
			card.Name = $"{actionStr} {spellDto.name}";
			card.Text = spellDto.name;
			card.TextFontSize = 60;
			string fileName = $"{Folders.Spells}\\{spellDto.name}.png";
			if (File.Exists(fileName))
			{

				// Placeholder for spells is 187x187
				// TODO: Stretch image to fit placeholder
				card.Placeholder = fileName;
			}

			return card;
		}

		static string RemoveConcentration(string duration, bool isScroll)
		{
			if (isScroll)
				duration = duration.Replace("Concentration, ", "");
			return duration;
		}

		static string GetSpellDescription(SpellDto spellDto, bool isScroll)
		{
			string lowerSpellDuration = spellDto.duration.ToLower();
			string durationStr;
			if (lowerSpellDuration.Contains("instantaneous") || lowerSpellDuration.Contains("special"))
				durationStr = string.Empty;
			else
				durationStr = $" Duration: {RemoveConcentration(spellDto.duration, isScroll)}.";

			string description = spellDto.description.Replace("**", "") + durationStr;
			if (description.IndexOf('{') < 0 && description.IndexOf('«') < 0 && description.IndexOf('»') < 0)
				return description;

			Spell spell = Spell.FromDto(spellDto, Spell.GetLevel(spellDto.level), 0, 3);

			return description
				.Replace("«", "")
				.Replace("»", "")
				.Replace("your spell save dc", "a spell save dc of 13")
				.Replace("{spell_DieStrRaw}", spell.DieStrRaw)
				.Replace("{spell_DieStr}", spell.DieStr)
				.Replace("{SpellcastingAbilityModifierStr}", "+3")
				.Replace("{spell_AmmoCount_word}", spell.AmmoCount_word)
				.Replace("{spell_AmmoCount_Word}", spell.AmmoCount_Word)
				.Replace("{spell_AmmoCount}", spell.AmmoCount.ToString())
				.Replace("{spell_DoubleAmmoCount}", spell.DoubleAmmoCount.ToString());
		}

		public static Card AddGiftSpellCard(SpellDto spellDto, CardData CardData, Deck ActiveDeck)
		{
			double placeholderWidth;
			double placeholderHeight;
			Card scroll = CreateSpellCard("Gift", spellDto, CardData, ActiveDeck);
			scroll.Description = GetSpellDescription(spellDto, true);
			scroll.AdditionalInstructions = "Give this spell scroll to a player, NPC, or monster (enter their name below).";
			scroll.AlertMessage = $"{userName} gave the {spellDto.name} spell scroll to {recipient}.";
			scroll.Expires = CardExpires.Never;
			CardFactory.AddPlayerNpcRecipientField(CardData, scroll, "scroll");
			if (!CardStyles.Apply(scroll))
			{
				int randomValue = CardFactory.random.Next(0, 100);
				if (randomValue < 40)
				{
					placeholderWidth = 155;
					placeholderHeight = 152;
					scroll.StylePath = "Scrolls\\Rods";
				}
				else if (randomValue < 60)
				{
					placeholderWidth = 131;
					placeholderHeight = 130;
					scroll.StylePath = "Scrolls\\Smooth Light";
				}
				else
				{
					placeholderWidth = 128;
					placeholderHeight = 127;
					scroll.StylePath = "Scrolls\\Tan";
				}
				scroll.ScalePlaceholder(placeholderWidth, placeholderHeight);
			}
			return scroll;
		}

		private static bool TargetsOne(SpellDto spellDto)
		{
			return spellDto.target == "1" ||
						 spellDto.target == "1 willing" ||
						 spellDto.target == "1 humanoid" ||
						 spellDto.target == "1 corpse" ||
						 spellDto.target == "1 beast" ||
						 spellDto.target == "1 dead" ||
						 spellDto.target == "1 dead humanoid" ||
						 spellDto.target == "1 friendly/charmed beast" ||
						 spellDto.target == "1 beast or humanoid" ||
						 spellDto.target == "1 beast or plant (huge or smaller)" ||
						 spellDto.target == "1 (no undead)" ||
						 spellDto.target == "1 (medium or smaller)" ||
						 spellDto.target == "1 (celestial, elemental, fey, or fiend)" ||
						 spellDto.target == "*|1" ||
						 spellDto.target == "1 willing beast";
		}
		public static Card AddCastSpellCard(SpellDto spellDto, CardData cardData, Deck activeDeck)
		{
			Card card = CreateSpellCard("Cast", spellDto, cardData, activeDeck);
			card.Description = GetSpellDescription(spellDto, false);
			card.Expires = CardExpires.Immediately;
			string alertMessage;
			const double witchcraftPlaceholderSize = 174;
			if (!CardStyles.Apply(card))
				switch (CardFactory.random.Next(0, 4))
				{
					case 0:
						card.StylePath = "Witchcraft\\Common";
						break;
					case 1:
						card.StylePath = "Witchcraft\\Rare";
						break;
					case 2:
						card.StylePath = "Witchcraft\\Epic";
						break;
					case 3:
						card.StylePath = "Witchcraft\\Legendary";
						break;
				}
			card.ScalePlaceholder(witchcraftPlaceholderSize);
			if (!string.IsNullOrWhiteSpace(spellDto.targetingPrompt))
			{
				// TODO: Shorten Description MaxHeight.
				Field targetField = new Field(card)
				{
					Name = "target",
					Label = spellDto.targetingPrompt,
					Required = spellDto.targetingPrompt.ToLower().IndexOf("optional") < 0,
					IsDirty = true
				};
				if (TargetsOne(spellDto))
					targetField.Type = FieldType.Text;
				else
					targetField.Type = FieldType.LongText;

				cardData.AllKnownFields.Add(targetField);
				alertMessage = $"{userName} casts {spellDto.name}, targeting {target}.";
			}
			else
				alertMessage = $"{userName} casts {spellDto.name}.";
			card.AlertMessage = alertMessage;

			card.Rarity = Rarity.Legendary;
			return card;
		}
	}
}

