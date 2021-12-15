using System;
using System.Linq;

namespace TaleSpireCore
{
	public class QueuedEffect
	{
		public string Id { get; set; }
		public string MiniName { get; set; }
		public string EffectName { get; set; }
		public IOldPersistentEffect PersistentEffect { get; set; }
		public QueuedEffect(string id, string effectName = null)
		{
			Id = id;
			EffectName = effectName;
		}

		public QueuedEffect(string id, IOldPersistentEffect persistentEffect, string miniName)
		{
			MiniName = miniName;
			Id = id;
			PersistentEffect = persistentEffect;
		}

		public QueuedEffect()
		{

		}
	}
}