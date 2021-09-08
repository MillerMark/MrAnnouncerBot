using System;
using System.Linq;

namespace TaleSpireCore
{
	public class QueuedEffect
	{
		public string Id { get; set; }
		public string EffectName { get; set; }
		public PersistentEffect PersistentEffect { get; set; }
		public string Name { get; set; }
		public QueuedEffect(string id, string effectName = null)
		{
			Id = id;
			EffectName = effectName;
		}
		public QueuedEffect(string id, PersistentEffect persistentEffect, string name)
		{
			Name = name;
			Id = id;
			PersistentEffect = persistentEffect;
		}
		public QueuedEffect()
		{

		}
	}
}