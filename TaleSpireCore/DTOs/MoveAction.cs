using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public struct MoveAction
	{
		public CreatureBoardAsset asset;
		public Vector3 DestLocation;
		public CreatureKeyMoveBoardTool.Dir dir;
		public string guid;
		public MovableHandle handle;
		public float moveTime;
		public Vector3 StartLocation;
		public float steps;
		public bool useHandle;
	}
}
