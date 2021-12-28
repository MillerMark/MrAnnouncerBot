using System;
using System.Linq;

namespace TaleSpireCore
{
	public class SavedCreatureEffect
	{
		public CreatureGuid ID { get; set; }
		public IOldPersistentEffect PersistentEffect { get; set; }
		public DateTime CreationTime { get; set; }
		public string NewCreatureName { get; set; }
		public SavedCreatureEffect()
		{
			CreationTime = DateTime.Now;
		}

		public SavedCreatureEffect(CreatureGuid creatureId, IOldPersistentEffect persistentEffect, string newCreatureName)
		{
			NewCreatureName = newCreatureName;
			ID = creatureId;
			PersistentEffect = persistentEffect;
			CreationTime = DateTime.Now;
		}
	}
}