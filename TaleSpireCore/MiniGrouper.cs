using System;
using System.Linq;
using System.Collections.Generic;
using Bounce.ManagedCollections;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace TaleSpireCore
{
	public class MiniGrouper : TaleSpireBehavior
	{
		System.Timers.Timer updateMiniIndicatorTimer;
		System.Timers.Timer checkMiniAltitudeTimer;
		bool moveToolActive;
		Vector3 lastGroupPosition;
		float lastAltitude;
		int baseColorIndex;
		bool isFlying;
		bool needToClearGroupIndicators;
		string lastMemberSelected;
		public List<string> Members { get; set; } = new List<string>();
		public Color IndicatorColor { get; set; } = UnityEngine.Color.red;
		public MiniGrouper()
		{
			BoardToolManager.OnSwitchTool += BoardToolManager_OnSwitchTool;
			CreatureManager.OnRequiresSyncStatusChanged += CreatureManager_OnRequiresSyncStatusChanged;
			Talespire.Minis.NewMiniSelected += Minis_NewMiniSelected;
			updateMiniIndicatorTimer = new System.Timers.Timer();
			updateMiniIndicatorTimer.Interval = 250;
			updateMiniIndicatorTimer.Elapsed += UpdateMiniIndicatorTimer_Elapsed;

			checkMiniAltitudeTimer = new System.Timers.Timer();
			checkMiniAltitudeTimer.Interval = 250;
			checkMiniAltitudeTimer.Elapsed += CheckMiniAltitudeTimer_Elapsed;
		}

		private void Minis_NewMiniSelected(object sender, CreatureBoardAssetEventArgs ea)
		{
			if (ea.Mini?.CreatureId.ToString() == OwnerID)
			{
				Talespire.Log.Warning($"MiniGrouper selected - RefreshIndicators();");
				RefreshIndicators();
			}
			Talespire.Log.Debug("NON-Grouper selected.");
		}

		private void CreatureManager_OnRequiresSyncStatusChanged(bool obj)
		{
			if (!obj)
				return;
			CreatureBoardAsset selected = Talespire.Minis.GetSelected();
			if (selected?.Creature.CreatureId.ToString() == OwnerID)
				CompareChanges(selected);
		}

		void UpdateAllBaseColors()
		{
			foreach (string memberId in Members)
				Talespire.Minis.SetBaseColorWithIndex(memberId, baseColorIndex);
		}

		void UpdateFlyingState()
		{
			CreatureBoardAsset owner = Talespire.Minis.GetCreatureBoardAsset(OwnerID);
			if (Guard.IsNull(owner, "owner")) return;

			float altitude = owner.GetCharacterPosition().Position.y;

			foreach (string memberId in Members)
			{
				Talespire.Minis.SetFlying(memberId, isFlying);
				if (!isFlying)
					Talespire.Minis.MoveVertically(memberId, altitude);
			}
		}

		private void CompareChanges(CreatureBoardAsset selected)
		{
			int newBaseColorIndex = selected.GetBaseColorIndex();
			if (baseColorIndex != newBaseColorIndex)
			{
				Talespire.Log.Debug($"BaseColor changed from {baseColorIndex} to {newBaseColorIndex}!!!");
				baseColorIndex = newBaseColorIndex;
				UpdateAllBaseColors();
			}

			if (isFlying != selected.IsFlying)
			{
				isFlying = !isFlying;
				if (isFlying)
					Talespire.Log.Debug($"Group is now FLYING!!!");
				else
					Talespire.Log.Debug($"Group is now GROUNDED!!!");
				UpdateFlyingState();
			}
		}

		~MiniGrouper()
		{
			UnhookEvents();
		}

		void SaveOwnerDetails(CreatureBoardAsset owner)
		{
			if (Guard.IsNull(owner, "owner")) return;

			lastGroupPosition = owner.PlacedPosition;
			baseColorIndex = owner.GetBaseColorIndex();
			isFlying = owner.IsFlying;
		}

		internal override void Initialize(string scriptData)
		{
			Talespire.Log.Debug($"MiniGrouper.Initialize(\"{scriptData}\")");
			base.Initialize(scriptData);
			CreatureBoardAsset owner = Talespire.Minis.GetCreatureBoardAsset(OwnerID);
			SaveOwnerDetails(owner);
		}

		private void UnhookEvents()
		{
			Talespire.Log.Debug($"MiniGrouper.UnhookEvents!!!!!");
			BoardToolManager.OnSwitchTool -= BoardToolManager_OnSwitchTool;
			CreatureManager.OnRequiresSyncStatusChanged -= CreatureManager_OnRequiresSyncStatusChanged;
			Talespire.Minis.NewMiniSelected += Minis_NewMiniSelected;
		}

		void OnDestroy()
		{
			UnhookEvents();
		}

		void MoveGroup(Vector3 deltaMove)
		{
			if (deltaMove.x == 0 && deltaMove.y == 0 && deltaMove.z == 0)
				return;  // No movement.

			foreach (string memberId in Members)
				Talespire.Minis.MoveRelative(memberId, deltaMove);
		}

		void RemoveMember(CreatureBoardAsset creatureBoardAsset)
		{
			string id = creatureBoardAsset.CreatureId.ToString();
			Members.Remove(creatureBoardAsset.CreatureId.ToString());
			Talespire.Minis.IndicatorChangeColor(id, Color.black);
			UpdateMiniColorsSoon();
		}

		void AddMember(CreatureBoardAsset creatureBoardAsset)
		{
			string id = creatureBoardAsset.CreatureId.ToString();
			Members.Add(id);
			UpdateMiniColors();
			UpdateMiniColorsSoon();
		}

		void UpdateMiniColorsSoon()
		{
			updateMiniIndicatorTimer.Start();
		}

		private void UpdateMiniColorsSafe()
		{
			UnityMainThreadDispatcher.ExecuteOnMainThread(() =>
			{
				UpdateMiniColors();
			});
		}

		private void UpdateMiniColors()
		{
			foreach (string memberId in Members)
				Talespire.Minis.IndicatorChangeColor(memberId, IndicatorColor);

			Talespire.Minis.IndicatorChangeColor(OwnerID, IndicatorColor);
		}

		private void UpdateMiniIndicatorTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			updateMiniIndicatorTimer.Stop();
			UpdateMiniColorsSafe();
		}

		public void ToggleMember(CreatureBoardAsset creatureBoardAsset)
		{
			if (Members.Contains(creatureBoardAsset.CreatureId.ToString()))
				RemoveMember(creatureBoardAsset);
			else
				AddMember(creatureBoardAsset);
		}

		public void RefreshIndicators()
		{
			needToClearGroupIndicators = true;
			UpdateMiniColors();
		}

		void ClearGroupIndicators()
		{
			needToClearGroupIndicators = false;
			foreach (string memberId in Members)
				if (memberId != lastMemberSelected)
					Talespire.Minis.IndicatorChangeColor(memberId, Color.black);
			lastMemberSelected = null;
		}

		void CheckAltitudeAgainSoon()
		{
			checkMiniAltitudeTimer.Start();
		}

		private void BoardToolManager_OnSwitchTool(BoardTool obj)
		{
			if (obj is CreatureMoveBoardTool || obj is CreatureKeyMoveBoardTool)
			{
				moveToolActive = true;
				// Something is moving!
			}
			else if (obj is DefaultBoardTool)
			{
				CreatureBoardAsset selected = Talespire.Minis.GetSelected();
				if (selected?.Creature.CreatureId.ToString() == OwnerID)
				{
					float newAltitude = selected.GetFlyingAltitude();

					if (newAltitude != lastAltitude)
						Talespire.Log.Warning($"Altitude changed from {lastAltitude} to {newAltitude}!");

					lastAltitude = newAltitude;
					CheckAltitudeAgainSoon();

					if (moveToolActive)
					{
						RefreshIndicators();
						Vector3 newPosition = selected.PlacedPosition;
						Vector3 deltaMove = newPosition - lastGroupPosition;
						MoveGroup(deltaMove);
						lastGroupPosition = newPosition;
					}
				}
				else
				{
					lastMemberSelected = selected?.CreatureId.ToString();
					if (needToClearGroupIndicators)
						ClearGroupIndicators();
				}

				moveToolActive = false;
			}
		}

		private void CheckMiniAltitudeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			Talespire.Log.Indent();
			checkMiniAltitudeTimer.Stop();

			CreatureBoardAsset selected = Talespire.Minis.GetSelected();
			if (selected?.Creature.CreatureId.ToString() == OwnerID)
			{
				float newAltitude = selected.GetFlyingAltitude();

				if (newAltitude != lastAltitude)
				{
					Talespire.Log.Warning($"Altitude changed from {lastAltitude} to {newAltitude}!");
					lastGroupPosition = selected.PlacedPosition;
				}

				lastAltitude = newAltitude;
			}

			Talespire.Log.Unindent();
		}

		public void MatchAltitude()
		{
			CreatureBoardAsset owner = Talespire.Minis.GetCreatureBoardAsset(OwnerID);
			if (Guard.IsNull(owner, "owner")) return;

			float altitude = owner.GetCharacterPosition().Position.y;

			foreach (string memberId in Members)
			{
				Talespire.Minis.MoveVertically(memberId, altitude);
			}
		}

		public void ShowAll()
		{
			foreach (string memberId in Members)
				Talespire.Minis.Show(memberId);
		}
		public void HideAll()
		{
			foreach (string memberId in Members)
				Talespire.Minis.Hide(memberId);
		}
	}
}
