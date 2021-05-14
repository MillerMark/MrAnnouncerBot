﻿using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class ChangeMinMaxGradient : BasePropertyChanger
	{
		public ChangeMinMaxGradient()
		{
		}

		public ChangeMinMaxGradient(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}

		public override object GetValue()
		{
			string trimmedValueStr = Value.Trim();
			int indexOfGradientIndicator = trimmedValueStr.IndexOf("->");
			if (indexOfGradientIndicator > 0)
			{
				string startColorStr = trimmedValueStr.Substring(0, indexOfGradientIndicator).Trim();
				if (!ColorUtility.TryParseHtmlString(startColorStr, out Color startColor))
				{
					Talespire.Log.Error($"Unable to parse \"{startColorStr}\" as an HTML color or gradient.");
					return new ParticleSystem.MinMaxGradient(Color.black);
				}
				string stopColorStr = trimmedValueStr.Substring(indexOfGradientIndicator + 2).Trim();
				if (!ColorUtility.TryParseHtmlString(stopColorStr, out Color stopColor))
				{
					Talespire.Log.Error($"Unable to parse \"{stopColorStr}\" as an HTML color or gradient.");
					return new ParticleSystem.MinMaxGradient(Color.black);
				}

				return new ParticleSystem.MinMaxGradient(startColor, stopColor);
			}
			else if (ColorUtility.TryParseHtmlString(Value, out Color color))
				return new ParticleSystem.MinMaxGradient(color);

			Talespire.Log.Error($"Unable to parse \"{Value}\" as an HTML color or gradient.");
			return new ParticleSystem.MinMaxGradient(Color.black);
		}
	}
}
