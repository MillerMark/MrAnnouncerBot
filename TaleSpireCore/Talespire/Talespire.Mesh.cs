using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Mesh
		{
			static Dictionary<string, UnityEngine.Mesh> allFoundMeshes;

			public static void Invalidate()
			{
				allFoundMeshes = null;
			}

			public static UnityEngine.Mesh Get(string meshName)
			{
				if (allFoundMeshes == null)
					Refresh();
				if (allFoundMeshes.TryGetValue(meshName, out UnityEngine.Mesh mesh))
					return mesh;
				Log.Debug($"Attempting to load mesh by name (\"{meshName}\")...");
				UnityEngine.Mesh loadedMesh = Resources.Load<UnityEngine.Mesh>(meshName);
				if (loadedMesh != null)
				{
					Log.Warning($"Mesh (\"{meshName}\") found!");
					allFoundMeshes.Add(meshName, loadedMesh);
				}
				else
					Log.Error($"Mesh (\"{meshName}\") NOT found!");

				return loadedMesh;
			}

			public static List<UnityEngine.Mesh> GetAll()
			{
				if (allFoundMeshes == null)
					Refresh();
				return allFoundMeshes.Values.ToList();
			}

			public static List<string> GetAllNames()
			{
				if (allFoundMeshes == null)
					Refresh();
				return allFoundMeshes.Keys.ToList();
			}

			public static void Refresh()
			{
				allFoundMeshes = new Dictionary<string, UnityEngine.Mesh>();
				Transform[] allTransforms = UnityEngine.Object.FindObjectsOfType<Transform>();
				if (allTransforms == null)
					return;
				
				foreach (Transform transform in allTransforms)
				{
					MeshFilter[] components = transform.GetComponents<MeshFilter>();
					if (components == null)
						continue;

					foreach (MeshFilter meshFilter in components)
						if (meshFilter.mesh != null)
							if (!allFoundMeshes.ContainsKey(meshFilter.mesh.name))
								allFoundMeshes.Add(meshFilter.mesh.name, meshFilter.mesh);
				}
			}
		}
	}
}