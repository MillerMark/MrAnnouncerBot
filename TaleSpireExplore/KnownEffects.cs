using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GoogleHelper;
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
			Talespire.Log.Debug($"KnownEffects.Create(\"{effectName}\")");
			string targetingSphereJson = Get(effectName)?.Effect;
			if (targetingSphereJson == null)
			{
				Talespire.Log.Error($"targetingSphereJson == null");
				return null;
			}

			Talespire.GameObjects.InvalidateFound();
			CompositeEffect compositeEffect = CompositeEffect.CreateFrom(targetingSphereJson);
			GameObject gameObject = compositeEffect.CreateOrFindUnsafe(instanceId);
			compositeEffect.RefreshIfNecessary(gameObject);

			if (gameObject != null)
				gameObject.name = effectName;
			return gameObject;
		}

		public static GameObject CreateUnsafe(string effectName, string instanceId = null)
		{
			string targetingSphereJson = Get(effectName)?.Effect;
			if (targetingSphereJson == null)
				return null;
			Talespire.GameObjects.InvalidateFound();
			CompositeEffect compositeEffect = CompositeEffect.CreateFrom(targetingSphereJson);
			//Talespire.Log.Debug($"GameObject gameObject = compositeEffect.CreateOrFindUnsafe(instanceId);");
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
