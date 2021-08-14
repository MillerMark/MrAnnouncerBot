using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace TaleSpireCore
{
	public enum WhenToApply
	{
		AfterCreation,
		AfterPositioning
	}
	
	public static class EffectParameters
	{
		static Dictionary<string, BaseEffectParameter> afterCreationParameterOverrides = new Dictionary<string, BaseEffectParameter>();
		static Dictionary<string, BaseEffectParameter> afterPositioningParameterOverrides = new Dictionary<string, BaseEffectParameter>();
		static EffectParameters()
		{
			RegisterParameters();
		}

		private static void RegisterParameters()
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
				if (typeof(BaseEffectParameter).IsAssignableFrom(type) && type.Name != nameof(BaseEffectParameter))
				{
					Talespire.Log.Warning($"Registering Effect Parameter - {type.Name}!");
					EffectParameterAttribute customAttribute = type.GetCustomAttribute<EffectParameterAttribute>();
					if (customAttribute != null)
						if (customAttribute.WhenToApply == WhenToApply.AfterCreation)
							afterCreationParameterOverrides[customAttribute.Name] = Activator.CreateInstance(type) as BaseEffectParameter;
						else
							afterPositioningParameterOverrides[customAttribute.Name] = Activator.CreateInstance(type) as BaseEffectParameter;
					else
						Talespire.Log.Error($"EffectParameter attribute missing from {type.Name}!!!");
				}
		}

		//static void ApplyColorOverride(GameObject gameObject, Color colorOverride)
		//{
		//	Talespire.Log.Debug($"colorOverride: {colorOverride}");
		//	Component script = gameObject.GetScript("RFX4_EffectSettings");
		//	if (script != null)
		//	{
		//		Talespire.Log.Warning($"Found script!");
		//		ReflectionHelper.SetPublicFieldValue(script, "UseCustomColor", true);
		//		ReflectionHelper.SetPublicFieldValue(script, "EffectColor", colorOverride);
		//		if (script is MonoBehaviour monoBehaviour)
		//		{
		//			monoBehaviour.enabled = false;
		//			monoBehaviour.enabled = true;
		//		}
		//	}
		//}

		static void ApplyParameter(GameObject gameObject, string leftSide, string rightSide, WhenToApply whenToApply)
		{
			if (whenToApply == WhenToApply.AfterCreation)
			{
				if (afterCreationParameterOverrides.ContainsKey(leftSide))
					afterCreationParameterOverrides[leftSide].Apply(gameObject, rightSide);
			}
			else if (whenToApply == WhenToApply.AfterPositioning)
			{
				if (afterPositioningParameterOverrides.ContainsKey(leftSide))
					afterPositioningParameterOverrides[leftSide].Apply(gameObject, rightSide);
			}
			//if (leftSide == "color")
			//{
			//	Color colorOverride = new HueSatLight(rightSide).AsRGB.ToUnityColor();
			//	ApplyColorOverride(gameObject, colorOverride);
			//}
		}

		static void ApplyParameter(GameObject gameObject, string parameter, WhenToApply whenToApply)
		{
			int indexOfEquals = parameter.IndexOf('=');
			if (indexOfEquals > 0)
			{
				string leftSide = parameter.Substring(0, indexOfEquals).Trim();
				string rightSide = parameter.Substring(indexOfEquals + 1).Trim();
				ApplyParameter(gameObject, leftSide, rightSide, whenToApply);
			}
		}

		static void ApplyParameters(GameObject gameObject, string parameters, WhenToApply whenToApply)
		{
			string[] parameterArray = parameters.Split(';');
			foreach (string parameter in parameterArray)
				ApplyParameter(gameObject, parameter, whenToApply);
		}

		public static string GetEffectNameOnly(string effectNameWithParameters)
		{
			int colonPos = effectNameWithParameters.IndexOf(":");
			if (colonPos <= 0)
				return effectNameWithParameters;
			return effectNameWithParameters.Substring(0, colonPos).Trim();
		}

		public static void GetEffectNameAndParameters(ref string effectName, out string parameters)
		{
			int colonPos = effectName.IndexOf(":");
			parameters = "";
			if (colonPos <= 0)
			{
				effectName = Talespire.Effects.GetIndividualEffectName(effectName);
				return;
			}

			parameters = effectName.Substring(colonPos + 1).Trim();
			effectName = Talespire.Effects.GetIndividualEffectName(effectName.Substring(0, colonPos)).Trim();
			Talespire.Log.Warning($"effectName is \"{effectName}\"; parameters are \"{parameters}\"");
		}

		static Dictionary<GameObject, string> savedParameters = new Dictionary<GameObject, string>();

		public static void ApplyAfterCreation(GameObject gameObject, string parameters)
		{
			if (gameObject == null || string.IsNullOrWhiteSpace(parameters))
				return;

			Talespire.Log.Debug($"ApplyAfterCreation (\"{parameters}\")...");
			ApplyParameters(gameObject, parameters, WhenToApply.AfterCreation);
			savedParameters[gameObject] = parameters;
		}
		
		public static void ApplyAfterPositioning(GameObject gameObject, bool isMoveable = false)
		{
			if (gameObject == null)
				return;
			if (savedParameters.ContainsKey(gameObject))
			{
				Talespire.Log.Debug($"ApplyAfterPositioning (\"{savedParameters[gameObject]}\")...");
				ApplyParameters(gameObject, savedParameters[gameObject], WhenToApply.AfterPositioning);
				if (!isMoveable)
					savedParameters.Remove(gameObject);
			}
		}
		public static void EndingSpell(GameObject spellEffect)
		{
			if (spellEffect != null)
				savedParameters.Remove(spellEffect);
		}
	}
}



