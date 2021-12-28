using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TaleSpireCore
{
	public static class Guard
	{
		public static bool IsNull(object instance, string instanceName, [CallerMemberName] string callerName = null)
		{
			if (instance == null)
			{
				Talespire.Log.Error($"{callerName}/{instanceName} is null!!!!");
				return true;
			}
			return false;
		}
	}
}
