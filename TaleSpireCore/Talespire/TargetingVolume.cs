using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public abstract class TargetingVolume
	{
		public Vector3 Center
		{
			get
			{
				return Transform.position;
			}
		}

		public Transform PrefabTransform
		{
			get
			{
				return targetingPrefab?.transform;
			}
		}

		public bool activeSelf
		{
			get
			{
				if (targetingPrefab != null)
					return targetingPrefab.activeSelf;
				return false;
			}
		}

		protected float offsetY = 0;
		public abstract CharacterPositions GetAllCreaturesInVolume();
		public GameObject targetingPrefab { get; set; }
		public Transform Transform { get; }

		public TargetingVolume()
		{

		}

		protected TargetingVolume(Transform transform)
		{
			Transform = transform;
		}
		
		protected void CreateTargetSelector(CompositeEffect compositeEffect)
		{
			try
			{
				targetingPrefab = compositeEffect?.CreateOrFindUnsafe();
				compositeEffect?.RefreshIfNecessary(targetingPrefab);
				if (targetingPrefab == null)
					Talespire.Log.Error($"targetingPrefab is NULL!!!");
				else
				{
					targetingPrefab.transform.SetParent(Transform);
					targetingPrefab.transform.localPosition = new Vector3(0, offsetY, 0);
				}
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
			}
		}
		public void SetActive(bool isActive)
		{
			if (targetingPrefab != null)
				targetingPrefab.SetActive(isActive);
		}
	}
}