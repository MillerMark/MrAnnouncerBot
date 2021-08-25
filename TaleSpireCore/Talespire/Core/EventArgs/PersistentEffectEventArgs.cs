using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public delegate void PersistentEffectEventHandler(object sender, PersistentEffectEventArgs ea);
	public class PersistentEffectEventArgs : EventArgs
	{
		public CreatureBoardAsset CreatureAsset { get; set; }
		public GameObject AssetLoader { get; set; }
		public GameObject EffectOrb { get; set; }
		public GameObject AttachedNode { get; set; }
		public PersistentEffect PersistentEffect { get; set; }

		public PersistentEffectEventArgs()
		{

		}

		public void Set(CreatureBoardAsset creatureAsset, GameObject assetLoader, GameObject effectOrb, GameObject attachedNode, PersistentEffect persistentEffect)
		{
			CreatureAsset = creatureAsset;
			AssetLoader = assetLoader;
			EffectOrb = effectOrb;
			AttachedNode = attachedNode;
			PersistentEffect = persistentEffect;
		}
	}
}