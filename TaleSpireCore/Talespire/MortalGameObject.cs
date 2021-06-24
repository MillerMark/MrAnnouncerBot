using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class MortalGameObject
	{
		public DateTime ExpireTime { get; set; }
		public GameObject GameObject { get; set; }
		public MortalGameObject(GameObject gameObject, DateTime expireTime)
		{
			ExpireTime = expireTime;
			GameObject = gameObject;
		}
		public MortalGameObject()
		{

		}
	}
}