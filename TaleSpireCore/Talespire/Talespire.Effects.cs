using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Effects
		{
			public static EffectManager Manager => SingletonBehaviour<EffectManager>.Instance;

			public static EffectManager.Effect Get(string effectName)
			{
				effectName = effectName.Trim();


				List<EffectManager.Effect> effects = GetAll();

				foreach (EffectManager.Effect effect in effects)
					if (effect._effectName == effectName)
						return effect;

				return null;
			}

			public static List<EffectManager.Effect> GetAll()
			{
				FieldInfo field = typeof(EffectManager).GetField("_effects", BindingFlags.NonPublic | BindingFlags.Instance);
				return field?.GetValue(Manager) as List<EffectManager.Effect>;
			}

			public static Dictionary<string, EffectManager.Effect> GetDictionary()
			{
				FieldInfo field = typeof(EffectManager).GetField("_effectDictionary", BindingFlags.NonPublic | BindingFlags.Instance);
				return field?.GetValue(Manager) as Dictionary<string, EffectManager.Effect>;
			}

			public static VisualEffect GetVisual(string effectName)
			{
				EffectManager.Effect effect = Get(effectName);
				return effect?.GetEffect();
			}

			public static void AddNew(string effectName, VisualEffect prefab)
			{
				EffectManager.Effect newEffect = new EffectManager.Effect();
				newEffect._effectName = effectName;
				newEffect._prefab = prefab;

				List<EffectManager.Effect> effectsList = GetAll();
				effectsList.Add(newEffect);

				Dictionary<string, EffectManager.Effect> dictionary = GetDictionary();

				if (dictionary != null)
				{
					// CreateEffectDictionary has already been called! 
					// We need to also add this new effect to the dictionary.
					dictionary.Add(effectName, newEffect);
				}
			}

			public static string GetList()
			{
				StringBuilder stringBuilder = new StringBuilder();
				List<EffectManager.Effect> effects = GetAll();
				stringBuilder.AppendLine($"Known Effects ({effects.Count}):");

				foreach (EffectManager.Effect effect in effects)
					stringBuilder.AppendLine("  " + effect._effectName);

				return stringBuilder.ToString();
			}

			static System.Random randomFileIndexer = new System.Random((int)DateTime.Now.Ticks);

			/// <summary>
			/// Converts "effectName[4]" into a random indexed effect name (e.g., "effectName1", "effectName2", "effectName3", or "effectName4")
			/// </summary>
			public static string GetIndividualEffectName(string effectName)
			{
				effectName = effectName.Trim();
				if (effectName.EndsWith("]"))
				{
					int openBracketPos = effectName.LastIndexOf("[");
					if (openBracketPos > 0)
					{
						string baseEffectName = effectName.Substring(0, openBracketPos);
						int numberStrLength = effectName.Length - openBracketPos - 2;
						if (numberStrLength > 0)
						{
							string effectNumberStr = effectName.Substring(openBracketPos + 1, numberStrLength);
							if (int.TryParse(effectNumberStr, out int effectNumber))
							{
								int randomIndex = randomFileIndexer.Next(effectNumber) + 1;
								return baseEffectName + randomIndex;
							}
						}
					}
				}
				return effectName;
			}
		}
	}
}