using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TaleSpireCore;

namespace TaleSpireCore
{
	public class ProjectileOptions
	{
		public string effectName;
		public string taleSpireId;
		public ProjectileKind kind;
		public string spellId;
		public int count;
		public float speed;
		public FireCollisionEventOn fireCollisionEventOn;
		public float launchTimeVariance;
		public float targetVariance;
		public float projectileSizeMultiplier;
		public float bezierPathMultiplier;
		public ProjectileSizeOption projectileSize;
		public List<Vector3> targetLocations = new List<Vector3>();
		public void AddTarget(string str)
		{
			Talespire.Log.Warning($"AddTarget: \"{str}\"");
			if (str.Contains(','))
				targetLocations.Add(Talespire.Convert.ToVector3(str));
			else
				targetLocations.Add(Talespire.Minis.GetHitTargetVector(str));
		}
		

	}
}
