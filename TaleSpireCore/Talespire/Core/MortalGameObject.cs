using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class MortalGameObject
	{
		public DateTime ParticleShutoffTime { get; set; }
		public DateTime ExpireTime { get; set; }
		public GameObject GameObject { get; set; }
		public float ShrinkOnDeleteTimeSeconds { get; set; }
		public bool ParticlesShutDown { get; set; }
		public VectorDto OriginalScale;
		public MortalGameObject(GameObject gameObject, DateTime expireTime, DateTime particleShutoffTime, float shrinkOnDeleteTimeSeconds)
		{
			ShrinkOnDeleteTimeSeconds = shrinkOnDeleteTimeSeconds;
			ParticleShutoffTime = particleShutoffTime;
			ExpireTime = expireTime;
			GameObject = gameObject;
			SetOriginalScale();
		}
		public MortalGameObject()
		{

		}
		public void ScaleToExpire(float secondsToExpiration)
		{
			if (GameObject == null || !GameObject.activeInHierarchy)
				return;
			
			if (GameObject.transform == null)
				return;

			float percentRemaining = secondsToExpiration / ShrinkOnDeleteTimeSeconds;
			float x = 1;
			float y = 1;
			float z = 1;
			if (OriginalScale != null)
			{
				x = OriginalScale.x;
				y = OriginalScale.y;
				z = OriginalScale.z;
			}
			GameObject.transform.localScale = new Vector3(x * percentRemaining, y * percentRemaining, z * percentRemaining);
		}

		void SetOriginalScale()
		{
			OriginalScale = new VectorDto(1, 1, 1);
			if (GameObject == null)
				return;
			Transform transform = GameObject.transform;
			if (transform == null)
				return;
			Vector3 localScale = transform.localScale;
			if (localScale == null)
				return;
			OriginalScale = localScale.GetVectorDto();
		}
	}
}