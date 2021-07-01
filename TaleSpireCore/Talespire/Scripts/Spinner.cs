using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore.Scripts
{
	public class Spinner : MonoBehaviour
	{
		public float DegreesPerSecondX = 0;
		public float DegreesPerSecondY = 0;
		public float DegreesPerSecondZ = 0;

		// Start is called before the first frame update
		void Start()
		{
			lastUpdateTime = Time.time;
		}

		float lastUpdateTime;

		void FixedUpdate()
		{
			if (DegreesPerSecondX == 0 && DegreesPerSecondY == 0 && DegreesPerSecondZ == 0)
				return;
			float secondsSinceLastUpdate = Time.time - lastUpdateTime;
			float newX = DegreesPerSecondX * secondsSinceLastUpdate;
			float newY = DegreesPerSecondY * secondsSinceLastUpdate;
			float newZ = DegreesPerSecondZ * secondsSinceLastUpdate;
			transform.Rotate(Vector3.right, newX);
			transform.Rotate(Vector3.up, newY);
			transform.Rotate(Vector3.forward, newZ);
			lastUpdateTime = Time.time;
		}
	}
}