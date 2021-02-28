using System;
using System.Linq;
using GoogleHelper;

namespace CardMaker
{
	[SheetName("D&D Deck Data")]
	[TabName("WeatherCards")]
	public class WeatherSpecialist : BaseWeatherSpecialist
	{
		[Column]
		[Indexer]
		public string ImageLayerName { get; set; }

		[Column]
		public string AlertMessageLight { get; set; }
		[Column]
		public string AlertMessageMedium { get; set; }
		[Column]
		public string AlertMessageHeavy { get; set; }

		[Column]
		public string CardPlayedMessageLight { get; set; }
		[Column]
		public string CardPlayedMessageMedium { get; set; }
		[Column]
		public string CardPlayedMessageHeavy { get; set; }

		[Column]
		public string CardNameLight { get; set; }
		[Column]
		public string CardNameMedium { get; set; }
		[Column]
		public string CardNameHeavy { get; set; }

		[Column]
		public string EffectKeywordLight { get; set; }
		[Column]
		public string EffectKeywordMedium { get; set; }
		[Column]
		public string EffectKeywordHeavy { get; set; }

		[Column]
		public string DamageModifierLight { get; set; }
		[Column]
		public string DamageModifierMedium { get; set; }
		[Column]
		public string DamageModifierHeavy { get; set; }

		[Column]
		public string DescriptionLight { get; set; }
		[Column]
		public string DescriptionMedium { get; set; }
		[Column]
		public string DescriptionHeavy { get; set; }
		public WeatherSpecialist()
		{

		}

		public override string GetAlertMessage(WeatherLevel weatherLevel)
		{
			switch (weatherLevel)
			{
				case WeatherLevel.Medium:
					return AlertMessageMedium;
				case WeatherLevel.Heavy:
					return AlertMessageHeavy;
				default:
					return AlertMessageLight;
			}
		}

		public override string GetImageLayerName()
		{
			return ImageLayerName;
		}

		public override string GetCardPlayedMessage(WeatherLevel weatherLevel)
		{
			switch (weatherLevel)
			{
				case WeatherLevel.Medium:
					return CardPlayedMessageMedium;
				case WeatherLevel.Heavy:
					return CardPlayedMessageHeavy;
				default:
					return CardPlayedMessageLight;
			}
		}

		public override string GetCardName(WeatherLevel weatherLevel)
		{
			switch (weatherLevel)
			{
				case WeatherLevel.Medium:
					return CardNameMedium;
				case WeatherLevel.Heavy:
					return CardNameHeavy;
				default:
					return CardNameLight;
			}
		}

		public override string GetEffectKeyword(WeatherLevel weatherLevel)
		{
			switch (weatherLevel)
			{
				case WeatherLevel.Medium:
					return EffectKeywordMedium;
				case WeatherLevel.Heavy:
					return EffectKeywordHeavy;
				default:
					return EffectKeywordLight;
			}
		}
		public override string GetDamageModifier(WeatherLevel weatherLevel)
		{
			switch (weatherLevel)
			{
				case WeatherLevel.Medium:
					return DamageModifierMedium;
				case WeatherLevel.Heavy:
					return DamageModifierHeavy;
				default:
					return DamageModifierLight;
			}
		}

		public override string GetDescription(WeatherLevel weatherLevel)
		{
			switch (weatherLevel)
			{
				case WeatherLevel.Medium:
					return DescriptionMedium;
				case WeatherLevel.Heavy:
					return DescriptionHeavy;
				default:
					return DescriptionLight;
			}
		}
	}
}

