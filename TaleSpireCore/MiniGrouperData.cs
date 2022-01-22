using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

namespace TaleSpireCore
{
	public class MiniGrouperData
	{
		private const int MaxFollowTheLeaderSavedEntries = 25;

		[DefaultValue(null)]
		public List<SerializableVector3> followTheLeaderMovementCache { get; set; }

		[DefaultValue(false)]
		public bool Hidden { get; set; }

		[DefaultValue(false)]
		public bool Flying { get; set; }

		[DefaultValue(FormationStyle.FreeForm)]
		public FormationStyle FormationStyle { get; set; } = FormationStyle.FreeForm;

		[DefaultValue(1)]
		public int ColumnRadius { get; set; } = 1;

		[DefaultValue(GroupMovementMode.Formation)]
		public GroupMovementMode Movement { get; set; } = GroupMovementMode.Formation;

		[DefaultValue(LookTowardMode.Movement)]
		public LookTowardMode Look { get; set; } = LookTowardMode.Movement;

		/// <summary>
		/// The target the group will look at if the Look property is set to LookTowardMode.Creature
		/// </summary>
		[DefaultValue(null)]
		public string Target { get; set; }

		[DefaultValue(-1)]
		public int BaseIndex { get; set; } = -1;

		[DefaultValue(0)]
		public int RingHue { get; set; }

		public List<string> Members { get; set; } = new List<string>();

		[DefaultValue(0)]
		public int Spacing { get; set; }

		[DefaultValue(360)]
		public int ArcAngle { get; set; } = 360;

		public int NumFollowTheLeaderCacheEntries
		{
			get
			{
				if (followTheLeaderMovementCache == null)
					return 0;
				return followTheLeaderMovementCache.Count;
			}
		}
		

		public MiniGrouperData()
		{

		}

		public void AddFollowTheLeaderPoint(Vector3 deltaMove)
		{
			if (followTheLeaderMovementCache == null)
				followTheLeaderMovementCache = new List<SerializableVector3>();

			followTheLeaderMovementCache.Add(deltaMove);
			while (followTheLeaderMovementCache.Count > MaxFollowTheLeaderSavedEntries)
				followTheLeaderMovementCache.RemoveAt(0);
		}

		public Vector3 GetFollowTheLeaderEntryByIndex(int cacheIndex)
		{
			if (followTheLeaderMovementCache == null || cacheIndex < 0 || cacheIndex >= followTheLeaderMovementCache.Count)
				return Vector3.zero;

			return followTheLeaderMovementCache[cacheIndex];
		}

		public void ClearFollowTheLeaderCache()
		{
			if (followTheLeaderMovementCache == null)
				return;

			followTheLeaderMovementCache.Clear();
		}
	}
}
