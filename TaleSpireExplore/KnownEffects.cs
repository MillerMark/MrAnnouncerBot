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
			CompositeEffect.EffectsBuilder = new KnownEffectsFactory();
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
			string targetingSphereJson = Get(effectName)?.Effect;
			if (targetingSphereJson == null)
				return null;
			Talespire.GameObjects.InvalidateFound();
			CompositeEffect compositeEffect = CompositeEffect.CreateFrom(targetingSphereJson);
			GameObject gameObject = compositeEffect.CreateOrFind(instanceId);
			if (gameObject != null)
				gameObject.name = effectName;
			return gameObject;
		}
	}
}
