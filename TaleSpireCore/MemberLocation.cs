using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class MemberLocation
	{
		public Vector3 Position { get; set; }
		public float DistanceToLeader { get; set; }
		public string Name { get; set; }
		public CreatureBoardAsset Asset { get; set; }
		public MemberLocation()
		{

		}
	}
}
