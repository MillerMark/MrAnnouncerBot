﻿using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TaleSpireCore
{
	public class OldPersistentEffect : IOldPersistentEffect
	{
		private const string STR_SpinLock = "SpinLock";
		
		public string EffectName { get; set; }

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

		public float LockedRotation { get; set; }

		public bool Hidden { get; set; }
		public Dictionary<string, bool> Indicators { get; set; } = new Dictionary<string, bool>();  /* Indicator Name (has to match the GameObject) mapped to its visible state */

		public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

		public OldPersistentEffect()
		{
			InitializeIndicators();
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