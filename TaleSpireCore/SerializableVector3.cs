using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class SerializableVector3
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public SerializableVector3()
		{

		}

		public SerializableVector3(Vector3 vector3)
		{
			X = vector3.x;
			Y = vector3.y;
			Z = vector3.z;
		}

		/// <summary>
		/// Implicitly converts an instance of type MyVector3 to a new instance of type Vector3.
		/// </summary>
		/// <param name="myVector3">An instance of type MyVector3 to convert.</param>
		/// <returns>Returns a new instance of type Vector3, derived from the specified MyVector3 instance.</returns>
		public static implicit operator Vector3(SerializableVector3 myVector3)
		{
			return new Vector3(myVector3.X, myVector3.Y, myVector3.Z);
		}

		/// <summary>
		/// Implicitly converts an instance of type Vector3 to a new instance of type MyVector3.
		/// </summary>
		/// <param name="vector3">An instance of type Vector3 to convert.</param>
		/// <returns>Returns a new instance of type MyVector3, derived from the specified vector3 instance.</returns>
		public static implicit operator SerializableVector3(Vector3 vector3)
		{
			// TODO: Return a new instance of type MyVector3, derived from the vector3 parameter.
			return new SerializableVector3(vector3);
		}
	}
}
