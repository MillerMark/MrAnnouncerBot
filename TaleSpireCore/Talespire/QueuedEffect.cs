using System;
using System.Linq;
using static TaleSpireCore.Talespire;

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

		private static void InitializeFromEffectName(string creatureId, string effectName, string name)
		{
			Log.Indent();
			CreaturePresenter.TryGetAsset(new CreatureGuid(creatureId), out CreatureBoardAsset creatureAsset);

			if (creatureAsset != null)
				PersistentEffects.InitializeMiniAsEffect(creatureAsset, effectName, name);
			else
				Log.Debug($"creatureAsset is null this update cycle....");
			Log.Unindent();
		}

		private static void InitializeFromPersistentEffect(string creatureId, IOldPersistentEffect persistentEffect, string newCreatureName)
		{
			Log.Indent();
			CreaturePresenter.TryGetAsset(new CreatureGuid(creatureId), out CreatureBoardAsset creatureAsset);

			if (creatureAsset != null)
				PersistentEffects.InitializeMiniFromPersistentEffect(creatureAsset, persistentEffect, newCreatureName);
			else
				Log.Warning($"creatureAsset is null (ID = {creatureId}) this update cycle....");

			Log.Unindent();
		}

		public void Initialize()
		{
			Log.Indent();
			if (EffectName != null)
				InitializeFromEffectName(Id, EffectName, MiniName);
			else
				InitializeFromPersistentEffect(Id, PersistentEffect, MiniName);

			Log.Unindent();
		}
	}
}