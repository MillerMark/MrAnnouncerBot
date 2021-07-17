using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	[EffectParameter("color")]
	public class ColorParameter : BaseEffectParameter
	{
		public override void Apply(GameObject gameObject, string rightSide)
		{
			Color colorOverride = new HueSatLight(rightSide).AsRGB.ToUnityColor();
			Talespire.Log.Debug($"colorOverride: {colorOverride}");

			HandleRfx4ScriptColorChanges(gameObject, colorOverride);
			// TODO: Add more color changes for particle systems, other known scripts that impact color, etc.
		}

		private static void HandleRfx4ScriptColorChanges(GameObject gameObject, Color colorOverride)
		{
			Component script = gameObject.GetScript("RFX4_EffectSettings");
			if (script != null)
			{
				Talespire.Log.Warning($"Found script!");
				ReflectionHelper.SetPublicFieldValue(script, "UseCustomColor", true);
				ReflectionHelper.SetPublicFieldValue(script, "EffectColor", colorOverride);
				if (script is MonoBehaviour monoBehaviour)
				{
					monoBehaviour.enabled = false;
					monoBehaviour.enabled = true;
				}
			}
		}

		public ColorParameter()
		{
		}
	}
}



