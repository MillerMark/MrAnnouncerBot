using System;
using System.Linq;

namespace CardMaker
{
	public abstract class BaseWeatherSpecialist
	{
		public abstract string GetCardName(WeatherLevel weatherLevel);
		public abstract string GetEffectKeyword(WeatherLevel weatherLevel);
		public abstract string GetDamageModifier(WeatherLevel weatherLevel);
		public abstract string GetDescription(WeatherLevel weatherLevel);
		public abstract string GetAlertMessage(WeatherLevel weatherLevel);
		public abstract string GetCardPlayedMessage(WeatherLevel weatherLevel);
		public abstract string GetImageLayerName();
	}
}

