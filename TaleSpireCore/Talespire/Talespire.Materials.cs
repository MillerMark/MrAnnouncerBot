using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Materials
		{
			static Dictionary<string, Material> allFoundMaterials;
			static Dictionary<string, Shader> allFoundShaders;

			public static void Invalidate()
			{
				allFoundMaterials = null;
				allFoundShaders = null;
			}

			public static Material GetMaterial(string materialName)
			{
				if (allFoundMaterials == null)
					Refresh();
				if (allFoundMaterials.TryGetValue(materialName, out Material material))
					return material;
				Log.Debug($"Attempting to load material by name (\"{materialName}\")...");
				Material loadedMaterial = Resources.Load<Material>(materialName);
				if (loadedMaterial != null)
				{
					Log.Warning($"Material (\"{materialName}\") found!");
					allFoundMaterials.Add(materialName, loadedMaterial);
				}
				else
					Log.Error($"Material (\"{materialName}\") NOT found!");

				return loadedMaterial;
			}

			public static Shader GetShader(string shaderName)
			{
				if (allFoundShaders == null)
					Refresh();
				if (allFoundShaders.TryGetValue(shaderName, out Shader shader))
					return shader;
				Log.Debug($"Attempting to load shader by name (\"{shaderName}\")...");
				Shader loadedShader = Resources.Load<Shader>(shaderName);
				if (loadedShader != null)
				{
					Log.Warning($"Shader (\"{shaderName}\") found!");
					allFoundShaders.Add(shaderName, loadedShader);
				}
				else
					Log.Error($"Shader (\"{shaderName}\") NOT found!");

				return loadedShader;
			}

			public static List<Material> GetAllMaterials()
			{
				if (allFoundMaterials == null)
					Refresh();
				return allFoundMaterials.Values.ToList();
			}

			public static List<Shader> GetAllShaders()
			{
				if (allFoundShaders == null)
					Refresh();
				return allFoundShaders.Values.ToList();
			}

			public static List<string> GetAllMaterialNames()
			{
				if (allFoundMaterials == null)
					Refresh();
				return allFoundMaterials.Keys.ToList();
			}

			public static void Refresh()
			{
				allFoundMaterials = new Dictionary<string, Material>();
				allFoundShaders = new Dictionary<string, Shader>();
				Transform[] allTransforms = UnityEngine.Object.FindObjectsOfType<Transform>();
				if (allTransforms != null)
					foreach (Transform transform in allTransforms)
					{
						Renderer[] components = transform.GetComponents<Renderer>();
						if (components != null)
							foreach (Renderer renderer in components)
								if (renderer.materials != null)
									foreach (Material material in renderer.materials)
									{
										if (!allFoundMaterials.ContainsKey(material.name))
											allFoundMaterials.Add(material.name, material);
										if (!allFoundShaders.ContainsKey(material.shader.name))
											allFoundShaders.Add(material.shader.name, material.shader);
									}
					}
			}
		}
	}
}