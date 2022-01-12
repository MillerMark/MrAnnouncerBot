﻿using System;
using System.Linq;
using System.Collections.Generic;
using Bounce.ManagedCollections;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace TaleSpireCore
{
	public class MiniGrouper : TaleSpireBehavior
	{
		CreatureBoardAsset ownerCreature;
		public MiniGrouperData Data { get; set; } = new MiniGrouperData();
		System.Timers.Timer updateMiniIndicatorTimer;
		System.Timers.Timer checkMiniAltitudeTimer;
		bool moveToolActive;
		Vector3 lastGroupPosition;
		float lastAltitude;
		int baseColorIndex;
		bool isFlying;
		bool needToClearGroupIndicators;
		bool editing;
		string lastMemberSelected;
		public Color IndicatorColor { get; set; } = Color.red;

		public CreatureBoardAsset OwnerCreature
		{
			get
			{
				if (ownerCreature == null)
					ownerCreature = Talespire.Minis.GetCreatureBoardAsset(OwnerID);
				return ownerCreature;
			}
		}

		public MiniGrouper()
		{
			Talespire.Log.Indent();
			BoardSessionManager.OnStateChange += BoardSessionManager_OnStateChange;
			BoardToolManager.OnSwitchTool += BoardToolManager_OnSwitchTool;
			CreatureManager.OnRequiresSyncStatusChanged += CreatureManager_OnRequiresSyncStatusChanged;
			Talespire.Minis.NewMiniSelected += Minis_NewMiniSelected;
			updateMiniIndicatorTimer = new System.Timers.Timer();
			updateMiniIndicatorTimer.Interval = 250;
			updateMiniIndicatorTimer.Elapsed += UpdateMiniIndicatorTimer_Elapsed;

			checkMiniAltitudeTimer = new System.Timers.Timer();
			checkMiniAltitudeTimer.Interval = 250;
			checkMiniAltitudeTimer.Elapsed += CheckMiniAltitudeTimer_Elapsed;
			Talespire.Log.Unindent();
		}

		private void BoardSessionManager_OnStateChange(PhotonSimpleSingletonStateMBehaviour<BoardSessionManager>.State obj)
		{
			if (obj.ToString() == "Active")
			{
				ownerCreature = null;
			}
		}

		private void Minis_NewMiniSelected(object sender, CreatureBoardAssetEventArgs ea)
		{
			Talespire.Log.Debug($"\"{ea.Mini?.GetOnlyCreatureName()}\" selected.");
			if (ea.Mini?.CreatureId.ToString() == OwnerID)
			{
				Talespire.Log.Warning($"MiniGrouper.RefreshIndicators();");
				RefreshIndicators();
			}
		}

		private void CreatureManager_OnRequiresSyncStatusChanged(bool obj)
		{
			if (!obj)
				return;

			CompareChanges();
		}

		void UpdateAllBaseColors()
		{
			foreach (string memberId in Data.Members)
				Talespire.Minis.SetBaseColorWithIndex(memberId, baseColorIndex);
		}

		void UpdateFlyingState()
		{
			Talespire.Log.Indent();
			try
			{
				CreatureBoardAsset owner = Talespire.Minis.GetCreatureBoardAsset(OwnerID);
				if (Guard.IsNull(owner, "owner")) return;

				float altitude = owner.GetCharacterPosition().Position.y;

				foreach (string memberId in Data.Members)
					Talespire.Minis.SetFlying(memberId, isFlying);

				if (!isFlying)
					foreach (string memberId in Data.Members)
					{
						Talespire.Log.Debug($"Moving relative...");
						Talespire.Minis.MoveRelative(memberId, new Vector3(0.01f, 0, 0.01f));
					}
			}
			finally
			{
				Talespire.Log.Unindent();
			}
		}

		public void DataChanged()
		{
			OnStateChanged(Data);
		}

		private void CompareChanges()
		{
			if (OwnerCreature == null)
				return;
			int newBaseColorIndex = OwnerCreature.GetBaseColorIndex();
			if (baseColorIndex != newBaseColorIndex)
			{
				Talespire.Log.Debug($"BaseColor changed from {baseColorIndex} to {newBaseColorIndex}!!!");
				baseColorIndex = newBaseColorIndex;
				Data.BaseIndex = baseColorIndex;
				UpdateAllBaseColors();
				Talespire.Log.Warning($"DataChanged();");
				DataChanged();
			}

			if (isFlying != OwnerCreature.IsFlying)
			{
				isFlying = OwnerCreature.IsFlying;
				Data.Flying = isFlying;
				if (isFlying)
					Talespire.Log.Debug($"Group is now FLYING!!!");
				else
					Talespire.Log.Debug($"Group is now GROUNDED!!!");
				UpdateFlyingState();
				DataChanged();
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
			if (!string.IsNullOrWhiteSpace(scriptData))
			{
				Data = Newtonsoft.Json.JsonConvert.DeserializeObject<MiniGrouperData>(scriptData);
				Talespire.Log.Warning($"We got some data!");
				if (Data.BaseIndex >= 0)
				{
					baseColorIndex = Data.BaseIndex;
					UpdateAllBaseColors();
				}

				isFlying = Data.Flying;
				//UpdateFlyingState();

				UpdateRingHue(Data.RingHue);
			}
		}

		private void UnhookEvents()
		{
			Talespire.Log.Debug($"MiniGrouper.UnhookEvents!!!!!");
			BoardToolManager.OnSwitchTool -= BoardToolManager_OnSwitchTool;
			CreatureManager.OnRequiresSyncStatusChanged -= CreatureManager_OnRequiresSyncStatusChanged;
			Talespire.Minis.NewMiniSelected -= Minis_NewMiniSelected;
		}

		void OnDestroy()
		{
			UnhookEvents();
		}

		void MoveGroup(Vector3 deltaMove)
		{
			if (editing)
				return;

			if (deltaMove.x == 0 && deltaMove.y == 0 && deltaMove.z == 0)
				return;  // No movement.

			if (Data.Movement == GroupMovementMode.FollowTheLeader)
				FollowTheLeader(deltaMove);
			else
				StayInFormation(deltaMove);
		}

		MemberLocation RemoveMemberClosestTo(List<MemberLocation> sourceMemberLocations, Vector3 targetPosition)
		{
			MemberLocation member = sourceMemberLocations.OrderBy(x => (x.Position - targetPosition).magnitude).FirstOrDefault();

			if (member == null)
				Talespire.Log.Error($"RemoveMemberClosestTo / member == null");

			sourceMemberLocations.Remove(member);
			return member;
		}

		void MoveMembersByProximity(List<MemberLocation> destinationMembers, List<MemberLocation> sourceMemberLocations, Vector3 targetPosition)
		{
			if (sourceMemberLocations.Count == 0)
				return;
			MemberLocation member = RemoveMemberClosestTo(sourceMemberLocations, targetPosition);
			if (member != null)
			{
				destinationMembers.Add(member);
				MoveMembersByProximity(destinationMembers, sourceMemberLocations, targetPosition);
			}
		}

		List<MemberLocation> GetMemberLocationsSortedByQueue(Vector3 targetPosition, List<MemberLocation> sourceMemberLocations)
		{
			List<MemberLocation> destinationMembers = new List<MemberLocation>();
			MoveMembersByProximity(destinationMembers, sourceMemberLocations, targetPosition);
			return destinationMembers;
		}

		/// <summary>
		/// Returns a list of MemberLocations, sorted by distance to the specified leader.
		/// </summary>
		private List<MemberLocation> GetMemberLocations(CreatureBoardAsset leaderAsset)
		{
			List<MemberLocation> memberLocations = new List<MemberLocation>();
			foreach (string memberId in Data.Members)
			{
				MemberLocation memberLocation = new MemberLocation();
				CreatureBoardAsset creatureBoardAsset = Talespire.Minis.GetCreatureBoardAsset(memberId);
				if (creatureBoardAsset == null)
					continue;
				memberLocation.Asset = creatureBoardAsset;
				memberLocation.Name = creatureBoardAsset.GetOnlyCreatureName();
				memberLocation.Position = creatureBoardAsset.PlacedPosition;
				memberLocation.DistanceToLeader = (leaderAsset.PlacedPosition - creatureBoardAsset.PlacedPosition).magnitude;
				memberLocations.Add(memberLocation);
			}
			memberLocations = memberLocations.OrderBy(x => x.DistanceToLeader).ToList();
			return memberLocations;
		}

		private void StayInFormation(Vector3 deltaMove)
		{
			foreach (string memberId in Data.Members)
				Talespire.Minis.MoveRelative(memberId, deltaMove);
		}

		void RemoveMember(CreatureBoardAsset creatureBoardAsset)
		{
			string id = creatureBoardAsset.CreatureId.ToString();
			Data.Members.Remove(creatureBoardAsset.CreatureId.ToString());
			Talespire.Minis.IndicatorRingChangeColor(id, Color.black);
			UpdateMiniColorsSoon();
		}

		void AddMember(CreatureBoardAsset creatureBoardAsset)
		{
			string id = creatureBoardAsset.CreatureId.ToString();
			Data.Members.Add(id);
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
			foreach (string memberId in Data.Members)
				Talespire.Minis.IndicatorRingChangeColor(memberId, IndicatorColor);

			CreatureBoardAsset selected = Talespire.Minis.GetSelected();

			if (selected?.Creature.CreatureId.ToString() == OwnerID)
				Talespire.Minis.IndicatorRingChangeColor(OwnerID, IndicatorColor);
		}

		private void UpdateMiniIndicatorTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			updateMiniIndicatorTimer.Stop();
			UpdateMiniColorsSafe();
		}

		public void ToggleMember(CreatureBoardAsset creatureBoardAsset)
		{
			if (Data.Members.Contains(creatureBoardAsset.CreatureId.ToString()))
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
			foreach (string memberId in Data.Members)
				if (memberId != lastMemberSelected)
					Talespire.Minis.IndicatorRingChangeColor(memberId, Color.black);
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
				if (OwnerCreature != null)
				{
					float newAltitude = OwnerCreature.GetFlyingAltitude();

					if (newAltitude != lastAltitude)
						Talespire.Log.Warning($"Altitude changed from {lastAltitude} to {newAltitude}!");

					lastAltitude = newAltitude;
					CheckAltitudeAgainSoon();

					if (moveToolActive)
					{
						RefreshIndicators();
						Vector3 newPosition = OwnerCreature.PlacedPosition;
						Vector3 deltaMove = newPosition - lastGroupPosition;
						MoveGroup(deltaMove);
						lastGroupPosition = newPosition;
					}
				}

				CreatureBoardAsset selected = Talespire.Minis.GetSelected();
				
				if (selected?.Creature.CreatureId.ToString() != OwnerID)
				{
					if (selected == null)
						lastMemberSelected = null;
					else
						lastMemberSelected = selected.CreatureId.ToString();

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

			if (OwnerCreature != null)
			{
				float newAltitude = OwnerCreature.GetFlyingAltitude();

				if (newAltitude != lastAltitude)
				{
					Talespire.Log.Warning($"Altitude changed from {lastAltitude} to {newAltitude}!");
					lastGroupPosition = OwnerCreature.PlacedPosition;
				}

				lastAltitude = newAltitude;
			}

			Talespire.Log.Unindent();
		}

		public void MatchAltitude()
		{
			if (Guard.IsNull(OwnerCreature, "OwnerCreature")) return;

			float altitude = OwnerCreature.GetCharacterPosition().Position.y;

			foreach (string memberId in Data.Members)
				Talespire.Minis.MoveVertically(memberId, altitude);
		}

		internal override void OwnerSelected()
		{
			RefreshIndicators();
		}

		public void ShowAll()
		{
			foreach (string memberId in Data.Members)
			{
				Talespire.Minis.Show(memberId);
				Talespire.Minis.MoveRelative(memberId, Vector3.zero);
			}
		}

		public void HideAll()
		{
			foreach (string memberId in Data.Members)
				Talespire.Minis.Hide(memberId);
		}

		public void UpdateRingHue(int ringHue)
		{
			Talespire.Log.Warning($"Setting Data.RingHue to {ringHue}");
			Data.RingHue = ringHue;
			HueSatLight hueSatLight = new HueSatLight(ringHue / 360.0, 1, 0.5);
			System.Drawing.Color asRGB = hueSatLight.AsRGB;
			IndicatorColor = new Color(asRGB.R / 255.0f, asRGB.G / 255.0f, asRGB.B / 255.0f);
			RefreshIndicators();
		}

		public void CommitRingHue(int ringHue)
		{
			UpdateRingHue(ringHue);
			DataChanged();
		}

		public void UpdateMemberList()
		{
			DataChanged();
		}

		public void SetHp(int hp, int maxHp)
		{
			foreach (string memberId in Data.Members)
				Talespire.Minis.SetHp(memberId, hp, maxHp);
		}

		public void Heal(int hp)
		{
			foreach (string memberId in Data.Members)
				Talespire.Minis.Heal(memberId, hp, true);
		}

		public void Damage(int hp)
		{
			foreach (string memberId in Data.Members)
				Talespire.Minis.Damage(memberId, hp, true);
		}

		public void KnockdownToggle()
		{
			Talespire.Minis.KnockDown(Data.Members);
		}

		public void RenameAll(string newName)
		{
			if (!string.IsNullOrWhiteSpace(newName))
				newName += " ";

			int creatureNum = 1;
			foreach (string member in Data.Members)
				if (Talespire.Minis.Rename(member, $"{newName}{creatureNum}"))
					creatureNum++;
		}

		public void RemoveMembers(List<string> creaturesToRemove)
		{
			foreach (string creatureId in creaturesToRemove)
			{
				Talespire.Log.Debug($"Removing {creatureId}.");
				Data.Members.Remove(creatureId);
			}
		}

		public void SetGroupName(string newName)
		{
			Talespire.Minis.Rename(OwnerID, newName);
		}

		public string GetGroupName()
		{
			CreatureBoardAsset asset = Talespire.Minis.GetCreatureBoardAsset(OwnerID);
			if (asset != null)
				return asset.GetOnlyCreatureName();

			Talespire.Log.Error($"GetGroupName - Asset with OwnerID {OwnerID} not found!");
			return string.Empty;
		}

		public void StartEditing()
		{
			editing = true;
		}

		public void StopEditing()
		{
			editing = false;
		}

		public void DestroyAll()
		{
			foreach (string member in Data.Members)
				Talespire.Minis.Delete(member);

			Talespire.Minis.Delete(OwnerID);
		}

		List<MemberLocation> memberLocationsBeforeRotation;
		float degreesOffsetForRotation;

		public void MarkPositionsForLiveRotation(float degreesStart)
		{
			degreesOffsetForRotation = degreesStart;
			CreatureBoardAsset leaderAsset = Talespire.Minis.GetCreatureBoardAsset(OwnerID);
			if (Guard.IsNull(leaderAsset, "leaderAsset")) return;
			memberLocationsBeforeRotation = GetMemberLocations(leaderAsset);
		}

		public void RotateFormation(float degrees)
		{
			if (Guard.IsNull(memberLocationsBeforeRotation, "memberLocationsBeforeRotation")) return;

			float degreesToRotate = degrees - degreesOffsetForRotation + 180;
			CreatureBoardAsset leaderAsset = Talespire.Minis.GetCreatureBoardAsset(OwnerID);
			if (Guard.IsNull(leaderAsset, "leaderAsset")) return;
			Vector3 centerPoint = leaderAsset.PlacedPosition;
			foreach (MemberLocation memberLocation in memberLocationsBeforeRotation)
			{
				Vector3 offset = centerPoint - memberLocation.Position;
				Vector3 newOffset = Quaternion.Euler(0, degreesToRotate, 0) * offset; // rotate it
				Vector3 newPosition = centerPoint + newOffset; // calculate rotated point
				Vector3 delta = newPosition - memberLocation.Asset.PlacedPosition;
				Talespire.Minis.MoveRelative(memberLocation.Asset.CreatureId.ToString(), delta);
			}
		}

		public void ArrangeInRectangle(int columns, int spacingFeet, float percentVariance)
		{
			CreatureBoardAsset leaderAsset = Talespire.Minis.GetCreatureBoardAsset(OwnerID);
			if (Guard.IsNull(leaderAsset, "leaderAsset")) return;

			List<MemberLocation> memberLocations = GetMemberLocations(leaderAsset);
			if (memberLocations == null || memberLocations.Count == 0)
				return;
			float baseDiameterFeet = GetLargestBaseDiameterFeet(memberLocations);

			float startingX = memberLocations[0].Position.x;
			float startingZ = memberLocations[0].Position.z;
			int rowCount = 0;
			int columnCount = 0;

			foreach (MemberLocation memberLocation in memberLocations)
			{
				float x = startingX + columnCount * GetSpaceTiles(spacingFeet, baseDiameterFeet, percentVariance);
				float z = startingZ + rowCount * GetSpaceTiles(spacingFeet, baseDiameterFeet, percentVariance);
				Vector3 newPosition = new Vector3(x, leaderAsset.PlacedPosition.y, z);
				Vector3 delta = newPosition - memberLocation.Position;
				Talespire.Minis.MoveRelative(memberLocation.Asset.CreatureId.ToString(), delta);
				columnCount++;
				if (columnCount >= columns)
				{
					columnCount = 0;
					rowCount++;
				}
			}
		}

		private static float GetLargestBaseDiameterFeet(List<MemberLocation> memberLocations)
		{
			float largestBaseDiameter = 0.5f;
			foreach (MemberLocation memberLocation in memberLocations)
				if (memberLocation.Asset.Creature.Scale > largestBaseDiameter)
					largestBaseDiameter = memberLocation.Asset.GetBaseDiameterFeet();

			return largestBaseDiameter;
		}

		private static float GetSpaceTiles(int spacingFeet, float baseDiameter, float percentVariance = 0)
		{
			float spaceBetweenMinisFeet;
			if (percentVariance == 0)
				spaceBetweenMinisFeet = baseDiameter + spacingFeet;
			else
				spaceBetweenMinisFeet = baseDiameter + UnityEngine.Random.Range(spacingFeet * (1 - percentVariance), spacingFeet * (1 + percentVariance));
			return Talespire.Convert.FeetToTiles(spaceBetweenMinisFeet);
		}

		List<Vector3> followTheLeaderMovementCache = new List<Vector3>();

		void PositionMember(MemberLocation memberLocation, Vector3 referencePosition, Vector3 totalDeltaMove, ref float offset)
		{
			Vector3 delta = totalDeltaMove * offset / totalDeltaMove.magnitude;
			Vector3 newPosition = referencePosition - delta;
			memberLocation.Asset.MoveCreatureTo(newPosition);
			offset += Talespire.Convert.FeetToTiles(memberLocation.Asset.GetBaseDiameterFeet() + Data.Spacing);
		}

		void MoveMembersToPreviousMemberPosition(List<MemberLocation> memberLocations, int startIndex)
		{
			int indexHead = 0;
			for (int i = startIndex; i < memberLocations.Count; i++)
			{
				memberLocations[i].Asset.MoveCreatureTo(memberLocations[indexHead].Position);
				indexHead++;
			}
		}

		Vector3 previousDeltaMove = Vector3.zero;

		void FollowTheLeader(Vector3 deltaMove)
		{
			if ((previousDeltaMove + deltaMove).magnitude < Talespire.Convert.FeetToTiles(2.5f))
			{
				previousDeltaMove += deltaMove;
				return;
			}

			deltaMove += previousDeltaMove;
			previousDeltaMove = Vector3.zero;

			CreatureBoardAsset leaderAsset = Talespire.Minis.GetCreatureBoardAsset(OwnerID);
			if (Guard.IsNull(leaderAsset, "leaderAsset")) return;

			followTheLeaderMovementCache.Add(deltaMove);

			List<MemberLocation> memberLocations = GetMemberLocationsSortedByQueue(leaderAsset.PlacedPosition - deltaMove, GetMemberLocations(leaderAsset));

			float offset = Talespire.Convert.FeetToTiles(leaderAsset.GetBaseDiameterFeet());
			Vector3 referencePosition = leaderAsset.PlacedPosition;
			int cacheIndex = followTheLeaderMovementCache.Count - 1;
			Vector3 cachedDeltaMove = followTheLeaderMovementCache[cacheIndex];
			for (int i = 0; i < memberLocations.Count; i++)
			{
				PositionMember(memberLocations[i], referencePosition, cachedDeltaMove, ref offset);
				if (offset > cachedDeltaMove.magnitude)
				{
					offset -= cachedDeltaMove.magnitude;  // To maintain the spacing.
																								// Now we need the next position in the cache!!!
					referencePosition -= cachedDeltaMove;
					cacheIndex--;
					if (cacheIndex < 0)
					{
						// We are out of cached locations!!!
						MoveMembersToPreviousMemberPosition(memberLocations, i + 1);
						break;
					}
					cachedDeltaMove = followTheLeaderMovementCache[cacheIndex];
				}
			}

			// memberLocations now has the members in the order they need to be positioned.

			Talespire.Log.Debug($"Sorted member locations:");
			foreach (MemberLocation memberLocation in memberLocations)
			{
				Talespire.Log.Debug($"  {memberLocation.DistanceToLeader * 5}ft - {memberLocation.Name}");
			}
			Talespire.Log.Debug($"");
			/* 
			There are lines of leader travel (points plus a delta) and there are previous member's positions (which are also safe).

			So I think we need to first track all the previous member positions, and sort them by distance to the leader.

			Next we try to position members along the line indicated by deltaMove.
			If we run out of space, we next move to previously saved lines.

			If we run out of previously saved lines, we next move to all our previous member positions.

			We clean up any saved lines we don't need anymore.
			 */
		}

		public void ClearLeaderMovementCache()
		{
			if (OwnerCreature != null)
				lastGroupPosition = OwnerCreature.PlacedPosition;
			followTheLeaderMovementCache.Clear();
			previousDeltaMove = Vector3.zero;
		}
	}
}
