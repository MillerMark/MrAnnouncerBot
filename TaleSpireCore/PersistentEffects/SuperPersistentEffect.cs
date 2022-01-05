using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TaleSpireCore
{
	public class SuperPersistentEffect: IOldPersistentEffect
	{
		private const string STR_SpinLock = "SpinLock";
		// TODO: EffectName is going to get way more sophisticated

		[JsonIgnore]
		public bool RotationIsLocked
		{
			get
			{
				if (Indicators.ContainsKey(STR_SpinLock))
					return Indicators[STR_SpinLock];
				Talespire.Log.Error($"Indicators is missing STR_SpinLock key!!!");
				return false;
			}
			set
			{
				Indicators[STR_SpinLock] = value;
			}
		}

		/// <summary>
		/// The rotation value (in degrees) that this mini is set to if rotation is locked.
		/// </summary>
		public float LockedRotation { get; set; }

		public bool Hidden { get; set; }

		[JsonIgnore]
		public Dictionary<string, string> Properties { get => EffectProperties.FirstOrDefault()?.Properties; }

		public Dictionary<string, string> ScriptData { get; set; } = new Dictionary<string, string>();

		[JsonIgnore]
		public string EffectName { get => EffectProperties.FirstOrDefault()?.EffectName; }

		public Dictionary<string, bool> Indicators { get; set; } = new Dictionary<string, bool>();  /* Indicator Name (has to match the GameObject) mapped to its visible state */

		public List<EffectProperties> EffectProperties = new List<EffectProperties>();

		public SuperPersistentEffect()
		{
			InitializeIndicators();
		}

		public SuperPersistentEffect(IOldPersistentEffect persistentEffect)
		{
			EffectProperties = new List<EffectProperties>();

			Talespire.Log.Debug($"");
			Talespire.Log.Debug($"new SuperPersistentEffect(from old)...");
			Talespire.Log.Debug($"");
			Talespire.Log.Debug($"Property Keys:");
			foreach (string propertyPath in persistentEffect.Properties.Keys)
				Talespire.Log.Debug($"  {propertyPath}");
			Talespire.Log.Debug($"");

			EffectProperties.Add(new EffectProperties(persistentEffect.EffectName, persistentEffect.Properties));

			Hidden = persistentEffect.Hidden;
			RotationIsLocked = persistentEffect.RotationIsLocked;
			LockedRotation = persistentEffect.LockedRotation;
			Indicators = new Dictionary<string, bool>(persistentEffect.Indicators);
		}

		private void InitializeIndicators()
		{
			// Must initialize all indicators here.
			Indicators.Add(STR_SpinLock, false);
		}

		public void Initialize(CreatureBoardAsset creatureBoardAsset)
		{
			if (RotationIsLocked)
				creatureBoardAsset.SetRotationDegrees(LockedRotation);
		}
	}
}