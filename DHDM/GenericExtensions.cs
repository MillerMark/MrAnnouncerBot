//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public static class GenericExtensions
	{
		public static T Next<T>(this List<T> list, T afterThisOne)
		{
			if (list == null)
				return default(T);

			if (afterThisOne == null)
				return list[0];

			int nextTargetIndex = list.IndexOf(afterThisOne) + 1;
			if (nextTargetIndex == list.Count)
				return list[0];
			else
				return list[nextTargetIndex];
		}

		public static T Previous<T>(this List<T> list, T afterThisOne)
		{
			if (list == null)
				return default(T);

			if (afterThisOne == null)
				return list[list.Count - 1];

			int nextTargetIndex = list.IndexOf(afterThisOne) - 1;
			if (nextTargetIndex == -1)
				return list[list.Count - 1];
			else
				return list[nextTargetIndex];
		}
	}
}