using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TaleSpireCore
{
	public class CharacterPosition
	{
		public string Name { get; set; }
		public string ID { get; set; }
		public VectorDto Position { get; set; }
		public float FlyingAltitude { get; set; }

		public float GetFloorY()
		{
			return Position.y - FlyingAltitude;
		}

		public bool IsInsideSquare(VectorDto volumeCenter, float sideEdgeLength)
		{
			float halfSideLength = sideEdgeLength / 2.0f;
			if (!OnSameLevel(volumeCenter))
				return false;

			if (Position.x < volumeCenter.x - halfSideLength)
				return false;
			if (Position.x > volumeCenter.x + halfSideLength)
				return false;
			if (Position.z < volumeCenter.z - halfSideLength)
				return false;
			if (Position.z > volumeCenter.z + halfSideLength)
				return false;

			return true;
		}

		private bool OnSameLevel(VectorDto volumeCenter)
		{
			float distanceFromY = Math.Abs(volumeCenter.y - Position.y);
			return distanceFromY <= 3;
		}

		public bool IsInsideSphere(VectorDto volumeCenter, float radius)
		{
			float distance = (float)Math.Sqrt(volumeCenter.x * volumeCenter.x + volumeCenter.y * volumeCenter.y + volumeCenter.z * volumeCenter.z);
			return distance < radius;
		}

		public bool IsInsideCircle(VectorDto volumeCenter, float radius)
		{
			if (!OnSameLevel(volumeCenter))
				return false;

			float distance = (float)Math.Sqrt(volumeCenter.x * volumeCenter.x + volumeCenter.z * volumeCenter.z);
			return distance < radius;
		}

		public bool IsInsideCube(VectorDto volumeCenter, float sideEdgeLength)
		{
			float halfSideLength = sideEdgeLength / 2.0f;

			if (Position.x < volumeCenter.x - halfSideLength)
				return false;
			if (Position.x > volumeCenter.x + halfSideLength)
				return false;

			if (Position.y < volumeCenter.y - halfSideLength)
				return false;
			if (Position.y > volumeCenter.y + halfSideLength)
				return false;

			if (Position.z < volumeCenter.z - halfSideLength)
				return false;
			if (Position.z > volumeCenter.z + halfSideLength)
				return false;

			return true;
		}
	}
}
