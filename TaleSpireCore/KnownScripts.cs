using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace TaleSpireCore
{
	public static class KnownScripts
	{
		static List<Type> allKnownScriptTypes;

		public static List<string> GetAllNames()
		{
			if (allKnownScriptTypes == null)
				ReloadKnownScript();
			return allKnownScriptTypes.Select(x => $"{x.FullName}").ToList();
		}

		static void ReloadKnownScript()
		{
			allKnownScriptTypes = new List<Type>();
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
				if (typeof(MonoBehaviour).IsAssignableFrom(type) && type.Name != nameof(MonoBehaviour) && type.Name != nameof(TaleSpireBehavior))
				{
					Talespire.Log.Warning($"We found a MonoBehavior: {type.FullName}");
					allKnownScriptTypes.Add(type);
				}
		}

		public static void Invalidate()
		{
			allKnownScriptTypes = null;
		}

		public static Type GetType(string scriptName)
		{
			if (allKnownScriptTypes == null)
				ReloadKnownScript();
			Type foundType = allKnownScriptTypes.FirstOrDefault(x => x.FullName == scriptName);
			if (foundType == null)
				foundType = allKnownScriptTypes.FirstOrDefault(x => x.Name == scriptName);
			return foundType;
		}
	}
}
