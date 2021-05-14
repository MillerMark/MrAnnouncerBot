using System;
using System.Linq;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Components
		{
			public static UnityEngine.Object[] GetAll<T>()
			{
				return UnityEngine.Object.FindObjectsOfType(typeof(T));
			}
		}
	}
}