using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaleSpireExploreDecompiles
{
	[SingletonResource("EffectManager/_EffectManager")]
	public class EffectManager : SingletonBehaviour<EffectManager>
	{
		// Fields
		[SerializeField]
		private List<Effect> _effects;
		private Dictionary<string, Effect> _effectDictionary;

		// Methods
		private void CreateEffectDictionary()
		{
			this._effectDictionary = new Dictionary<string, Effect>();
			foreach (Effect effect in this._effects)
			{
				this._effectDictionary.Add(effect._effectName, effect);
			}
		}

		private Effect GetEffect(string effectName)
		{
			if (this._effectDictionary == null)
			{
				this.CreateEffectDictionary();
			}
			if (this._effectDictionary.ContainsKey(effectName))
			{
				return this._effectDictionary[effectName];
			}
			Debug.LogError("Can't find requested effect in Effect Manager");
			return null;
		}

		public static VisualEffect HireEffect(string effect) =>
				SingletonBehaviour<EffectManager>.Instance.GetEffect(effect)?.GetEffect();

		public static void PlayEffect(string effectName, Vector3 location)
		{
			SingletonBehaviour<EffectManager>.Instance.GetEffect(effectName).GetEffect().Play(location);
		}

		public static void PlayRelationEffect(string effectName, Transform origin, Transform target)
		{
			SingletonBehaviour<EffectManager>.Instance.GetEffect(effectName).GetEffect().PlayFromOriginToTarget(origin, target);
		}

		// Nested Types
		[Serializable]
		public class Effect
		{
			// Fields
			public string _effectName;
			public VisualEffect _prefab;
			private SpawnFactory<VisualEffect> _effects;

			// Methods
			public VisualEffect GetEffect()
			{
				if (this._effects == null)
					this._effects = new SpawnFactory<VisualEffect>(this._prefab, null, 0, true, null);
				return this._effects.HireItem(false);
			}
		}
	}
}
