using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SheetsPersist;
using TaleSpireCore;

namespace TaleSpireExplore
{
	public static class KnownEffects
	{
		static KnownEffects()
		{
			CompositeEffect.KnownEffectsBuilder = new KnownEffectsFactory();
		}

		static List<TaleSpireEffect> allKnownEffects;
		public static void Invalidate()
		{
			allKnownEffects = null;
		}

		public static List<string> GetAllNames()
		{
			LoadIfNecessary();
			return allKnownEffects.Select(x => x.Name).ToList();
		}

		public static List<string> GetNamesFromCategory(string selectedCategory)
		{
			LoadIfNecessary();
			return allKnownEffects.Where(x => x.Category == selectedCategory).Select(x => x.Name).ToList();
		}

		public static List<string> GetAllCategories()
		{
			LoadIfNecessary();
			return allKnownEffects.Select(x => x.Category).Distinct().ToList();
		}

		static void LoadIfNecessary()
		{
			if (allKnownEffects == null)
				allKnownEffects = GoogleSheets.Get<TaleSpireEffect>();
		}

		public static TaleSpireEffect Get(string effectName)
		{
			LoadIfNecessary();
			return allKnownEffects.FirstOrDefault(x => x.Name == effectName);
		}

		public static GameObject Create(string effectName, string instanceId = null)
		{
			Talespire.Log.Indent($"KnownEffects.Create(\"{effectName}\")");
			try
			{
				string effectJson = Get(effectName)?.Effect;
				if (effectJson == null)
				{
					Talespire.Log.Error($"effectJson == null");
					return null;
				}

				Talespire.GameObjects.InvalidateFound();
				CompositeEffect compositeEffect = CompositeEffect.CreateFrom(effectJson);
				GameObject gameObject = compositeEffect.CreateOrFindUnsafe(instanceId);
				compositeEffect.RefreshIfNecessary(gameObject);

				if (gameObject != null)
					gameObject.name = effectName;
				return gameObject;
			}
			finally
			{
				Talespire.Log.Unindent();
			}
		}

		public static GameObject CreateUnsafe(string effectName, string instanceId = null)
		{
			string targetingSphereJson = Get(effectName)?.Effect;
			if (targetingSphereJson == null)
				return null;
			Talespire.GameObjects.InvalidateFound();
			CompositeEffect compositeEffect = CompositeEffect.CreateFrom(targetingSphereJson);
			GameObject gameObject = compositeEffect.CreateOrFindUnsafe(instanceId);
			compositeEffect.RefreshIfNecessary(gameObject);

			if (gameObject != null)
				Talespire.Log.Debug($"gameObject: {gameObject.name}");
			else
				Talespire.Log.Error($"CreateOrFindUnsafe returned null!");

			if (gameObject != null)
				gameObject.name = effectName;
			return gameObject;
		}

		public static void Save(string name, string effect)
		{
			TaleSpireEffect taleSpireEffect = Get(name);
			if (taleSpireEffect == null)
			{
				taleSpireEffect = new TaleSpireEffect()
				{
					Name = name,
					Effect = effect
				};
				allKnownEffects.Add(taleSpireEffect);
			}
			else
				taleSpireEffect.Effect = effect;

			GoogleSheets.SaveChanges(taleSpireEffect);
		}
	}
}
