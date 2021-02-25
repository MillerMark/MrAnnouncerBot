using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

namespace CardMaker
{
	public static class WeatherFactory
	{
		static List<WeatherSpecialist> weatherSpecialists = new List<WeatherSpecialist>();
		static WeatherFactory()
		{
			weatherSpecialists = GoogleSheets.Get<WeatherSpecialist>();
		}

		public static List<Card> CreateWeatherCards(int actualPowerLevel, CardData CardData, Deck ActiveDeck)
		{
			List<Card> result = new List<Card>();

			foreach (BaseWeatherSpecialist weatherSpecialist in weatherSpecialists)
			{
				switch (actualPowerLevel)
				{
					//`![](AABCE1C22EC0B3313FED58A917950BDF.png;;;0.02270,0.02270)
					case 1:
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Light, Rarity.Legendary, CardData, ActiveDeck));
						break;
					case 2:
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Light, Rarity.Epic, CardData, ActiveDeck));
						break;
					case 3:
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Light, Rarity.Rare, CardData, ActiveDeck));
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Medium, Rarity.Legendary, CardData, ActiveDeck));
						break;
					case 4:
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Light, Rarity.Rare, CardData, ActiveDeck));
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Medium, Rarity.Epic, CardData, ActiveDeck));
						break;
					case 5:
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Light, Rarity.Common, CardData, ActiveDeck));
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Medium, Rarity.Rare, CardData, ActiveDeck));
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Heavy, Rarity.Legendary, CardData, ActiveDeck));
						break;
					case 6:
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Light, Rarity.Common, CardData, ActiveDeck));
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Medium, Rarity.Rare, CardData, ActiveDeck));
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Heavy, Rarity.Epic, CardData, ActiveDeck));
						break;
					case 7:
					case 8:
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Medium, Rarity.Common, CardData, ActiveDeck));
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Heavy, Rarity.Rare, CardData, ActiveDeck));
						break;
					case 9:
					case 10:
						result.Add(CreateWeatherCard(weatherSpecialist, WeatherLevel.Heavy, Rarity.Common, CardData, ActiveDeck));
						break;
				}
			}
			return result;

		}

		public static Card CreateWeatherCard(BaseWeatherSpecialist weatherSpecialist, WeatherLevel weatherLevel, Rarity rarity, CardData CardData, Deck ActiveDeck)
		{
			Card card = CardData.AddCard(ActiveDeck);
			card.Rarity = rarity;
			card.Name = $"Change Weather - {weatherSpecialist.GetCardName(weatherLevel)}";
			card.StylePath = "PreMade";
			card.Description = $"{weatherSpecialist.GetDescription(weatherLevel)}";
			card.AdditionalInstructions = "Lasts one minute if this card is played out of battle, or one round if played during battle.";
			card.AlertMessage = weatherSpecialist.GetAlertMessage(weatherLevel);
			card.CardPlayed = $"QueueEffect(\"Weather\", CardUserName, \"{weatherSpecialist.GetCardName(weatherLevel)}\", \"{weatherSpecialist.GetEffectKeyword(weatherLevel)}\");\n" +
											$"TellAll($\"{weatherSpecialist.GetCardPlayedMessage(weatherLevel)}\");";
			CardFactory.QuickAddAllLayerDetails(card);
			SetWeatherLayerVisibilities(card, weatherSpecialist.GetImageLayerName(), (int)weatherLevel);
			return card;
		}

		static void SetWeatherLayerVisibilities(Card card, string imageLayerName, int weatherLevel)
		{
			card.HideAllLayersExcept(imageLayerName);
			card.SelectAlternateLayer(imageLayerName, weatherLevel.ToString());
		}
	}
}

