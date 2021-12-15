using System;
using System.Linq;
using System.Collections.Generic;

namespace TaleSpireCore
{
	public interface IOldPersistentEffect
	{
		string EffectName { get; }
		Dictionary<string, string> Properties { get; }
		bool Hidden { get; set; }
		float LockedRotation { get; set; }
		bool RotationIsLocked { get; set; }
		Dictionary<string, bool> Indicators { get; set; }
		void Initialize(CreatureBoardAsset creatureBoardAsset);
	}
}