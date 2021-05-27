using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class ChangeColor : BasePropertyChanger
	{
		public ChangeColor()
		{
		}

		public ChangeColor(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}

		protected override bool TrySetProperty(object instance, string propertyName)
		{
			if (instance is Material material)
			{
				// TODO: Verify property name is in the shader????
				material.SetColor(propertyName, (Color)GetValue());

				return true;
			}

			return false;
		}

		public override object GetValue()
		{
			// TODO: Support color multipliers, seemingly needed for shaders and materials.
			// "#aabbcc x4.24"
			if (ColorUtility.TryParseHtmlString(Value, out Color color))
				return color;

			return 0;
		}
	}
}
