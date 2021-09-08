using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	[PropertyType(typeof(UnityEngine.Color))]
	public class ChangeColor : BasePropertyChanger
	{
		public ChangeColor()
		{
		}

		public ChangeColor(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}

		public override bool TrySetProperty(object instance, string propertyName)
		{
			if (instance is Material material)
			{
				// TODO: Verify property name is in the shader????
				material.SetColor(propertyName, (Color)GetValue());

				return true;
			}

			return false;
		}

		public override bool CanSetProperty(object instance, string propertyName)
		{
			return instance is Material;
		}

		protected override object ParseValue()
		{
			// TODO: Support color multipliers, seemingly needed for shaders and materials.
			// "#aabbcc x4.24"
			//Talespire.Log.Debug($"ChangeColor.ParseValue");
			int indexOfMultiplier = Value.IndexOf(" x");
			string html = Value;
			float multiplier = 1;
			if (indexOfMultiplier > -1)
			{
				html = Value.Substring(0, indexOfMultiplier);
				string multiplierStr = Value.Substring(indexOfMultiplier + 2);
				float.TryParse(multiplierStr, out multiplier);
			}

			if (ColorUtility.TryParseHtmlString(html, out Color color))
			{
				if (multiplier > 1)
				{
					//Talespire.Log.Debug($"Returning multiplied (x{multiplier}) value!");
					return color * multiplier;    // Bil is never wrong!
					//new Color(color.r * multiplier, color.g * multiplier, color.b * multiplier);
				}
				return color;
			}

			return 0;
		}

		public void SetValue(string html, float multiplier)
		{
			string multiplierStr = string.Empty;
			if (multiplier > 1)
				multiplierStr = " x" + multiplier;

			Value = $"{html}{multiplierStr}";
		}
	}
}
