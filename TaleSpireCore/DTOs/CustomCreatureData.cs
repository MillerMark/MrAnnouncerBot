using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public struct CustomCreatureData
	{
		public string Alias;
		public string AvatarThumbnailUrl;
		public string BoardAssetId;
		public Color[] Colors;
		public string CreatureId;
		public bool ExplicitlyHidden;
		public CreatureStat Hp;
		public string Inventory;
		public VectorDto Position;
		public Euler Rotation;
		public CreatureStat Stat0;
		public CreatureStat Stat1;
		public CreatureStat Stat2;
		public CreatureStat Stat3;
		public bool TorchState;
		public string UniqueId;
	}
}
