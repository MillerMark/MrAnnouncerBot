using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TaleSpireCore
{
	public class MiniRotator
	{
		private const float MinDistanceToWatch = 40f;
		private const float TooCloseToAnimate = 1.75f;
		Dictionary<CreatureBoardAsset, Vector3> originalAngles = new Dictionary<CreatureBoardAsset, Vector3>();
		Dictionary<CreatureBoardAsset, float> lastUpdateTime = new Dictionary<CreatureBoardAsset, float>();
		List<CreatureBoardAsset> watchedCreatures = new List<CreatureBoardAsset>();
		List<CreatureBoardAsset> pausedCreatures = new List<CreatureBoardAsset>();
		List<CreatureBoardAsset> restoreRotationCreatures = new List<CreatureBoardAsset>();
		CreatureBoardAsset[] allMinis;

		void WatchCreature(CreatureBoardAsset creatureBoardAsset)
		{
			pausedCreatures.Remove(creatureBoardAsset);
			if (watchedCreatures.Contains(creatureBoardAsset))
				return;

			if (!originalAngles.ContainsKey(creatureBoardAsset))
				originalAngles.Add(creatureBoardAsset, creatureBoardAsset.GetRotation());
			watchedCreatures.Add(creatureBoardAsset);
		}

		void StopWatchingCreature(CreatureBoardAsset creatureBoardAsset)
		{
			pausedCreatures.Remove(creatureBoardAsset);
			watchedCreatures.Remove(creatureBoardAsset);
			restoreRotationCreatures.Add(creatureBoardAsset);
		}

		public void Update()
		{
			if (allMinis == null)
				return;

			WatchOrNot();
			RotateWatchedCreaturesTowardsTarget();
			RotateOtherCreaturesBack();
		}

		private void RotateOtherCreaturesBack()
		{
			for (int i = restoreRotationCreatures.Count - 1; i >= 0; i--)
			{
				CreatureBoardAsset restoreRotationCreature = restoreRotationCreatures[i];
				if (originalAngles.ContainsKey(restoreRotationCreature))
				{
					// TODO: Animate this. COME ON MARK!!!
					restoreRotationCreature.Rotator.localEulerAngles = originalAngles[restoreRotationCreature];
					restoreRotationCreatures.RemoveAt(i);
				}
			}
		}

		private void RotateWatchedCreaturesTowardsTarget()
		{
			Vector3 targetingPosition = Talespire.Target.TargetingPosition;

			foreach (CreatureBoardAsset watchedCreature in watchedCreatures)
				if (!pausedCreatures.Contains(watchedCreature))
				{
					float rotationFactor = 1;
					if (lastUpdateTime.ContainsKey(watchedCreature))
					{
						float secondsSinceLastUpdate = Time.time - lastUpdateTime[watchedCreature];
						float distanceToTargetFeet = Talespire.Target.GetTargetingCursorDistanceTo(watchedCreature.transform.position);
						float factor = Mathf.Clamp(distanceToTargetFeet, 0, MinDistanceToWatch) / MinDistanceToWatch;
						float minSecondsToUpdate = 0.15f * factor;
						bool tooSoonToUpdate = secondsSinceLastUpdate < minSecondsToUpdate;
						if (tooSoonToUpdate)
							return;

						rotationFactor += 3 - 3 * factor;
					}

					lastUpdateTime[watchedCreature] = Time.time;

					watchedCreature.RotateTowards(targetingPosition, 8f * rotationFactor);
					//watchedCreature.RotateTowards(targetingPosition);
				}
		}

		void PauseCreature(CreatureBoardAsset creatureBoardAsset)
		{
			pausedCreatures.Add(creatureBoardAsset);
		}
		private void WatchOrNot()
		{
			foreach (CreatureBoardAsset creatureBoardAsset in allMinis)
			{
				float distanceFeet = Talespire.Target.GetTargetingCursorDistanceTo(creatureBoardAsset.transform.position);
				float tooCloseToAnimate = TooCloseToAnimate * creatureBoardAsset.CreatureScale;
				
				/* 
					tooCloseToAnimate(0.5): 0.875
					tooCloseToAnimate(1): 1.75
					tooCloseToAnimate(2): 3.5
					tooCloseToAnimate(3): 5.25
					tooCloseToAnimate(4): 7
				*/

				if (distanceFeet < tooCloseToAnimate)
				{
					//Talespire.Log.Warning($"pause({creatureBoardAsset.CreatureScale}) - {tooCloseToAnimate}f");
					PauseCreature(creatureBoardAsset);
				}
				else if (distanceFeet < MinDistanceToWatch)
					WatchCreature(creatureBoardAsset);
				else
					StopWatchingCreature(creatureBoardAsset);
			}
		}

		public void Initialize()
		{
			allMinis = Talespire.Minis.GetAll();
			originalAngles.Clear();
		}

		public void Done()
		{
			allMinis = null;
		}
	}
}