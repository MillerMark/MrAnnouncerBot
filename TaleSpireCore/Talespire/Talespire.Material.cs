using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Material
		{
			static Dictionary<string, UnityEngine.Material> allFoundMaterials;

			public static void Invalidate()
			{
				allFoundMaterials = null;
			}

			public static UnityEngine.Material Get(string materialName)
			{
				if (allFoundMaterials == null)
					Refresh();
				if (allFoundMaterials.TryGetValue(materialName, out UnityEngine.Material material))
					return material;
				Log.Debug($"Attempting to load material by name (\"{materialName}\")...");
				UnityEngine.Material loadedMaterial = Resources.Load<UnityEngine.Material>(materialName);
				if (loadedMaterial != null)
				{
					Log.Warning($"Material (\"{materialName}\") found!");
					allFoundMaterials.Add(materialName, loadedMaterial);
				}
				else
					Log.Error($"Material (\"{materialName}\") NOT found!");

				return loadedMaterial;
			}

			public static List<UnityEngine.Material> GetAll()
			{
				if (allFoundMaterials == null)
					Refresh();
				return allFoundMaterials.Values.ToList();
			}

			public static List<string> GetAllNames()
			{
				if (allFoundMaterials == null)
					Refresh();
				return allFoundMaterials.Keys.ToList();
			}

			public static void Refresh()
			{
				allFoundMaterials = new Dictionary<string, UnityEngine.Material>();
				Transform[] allTransforms = UnityEngine.Object.FindObjectsOfType<Transform>();
				if (allTransforms != null)
					foreach (Transform transform in allTransforms)
					{
						Renderer[] components = transform.GetComponents<Renderer>();
						if (components != null)
							foreach (Renderer renderer in components)
								if (renderer.materials != null)
									foreach (UnityEngine.Material material in renderer.materials)
										if (!allFoundMaterials.ContainsKey(material.name))
										{
											//if (material.name.StartsWith("MFX_LineIndicator"))
											//	Log.Debug($"MFX_LineIndicator is in {transform.gameObject.name}");
											if (material.name.StartsWith("SH_VagrantSphere"))
											{
												Log.Debug($"SH_VagrantSphere is in {transform.gameObject.name}");
												Transform parentTransform = transform.gameObject.GetComponentInParent<Transform>();
												if (parentTransform != null && parentTransform.gameObject != null)
													Log.Debug($"SH_VagrantSphere's parent is \"{parentTransform.gameObject.name}\"");
											}
											//if (material.name.StartsWith("FXM_RulerSphere"))
											//	Log.Debug($"FXM_RulerSphere is in {transform.gameObject.name}");
											allFoundMaterials.Add(material.name, material);
										}
					}
			}
		}
	}
}