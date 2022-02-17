using System;
using System.Linq;
using System.Collections.Generic;
using Bounce.Unmanaged;
using Bounce.ManagedCollections;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using Unity.Entities.UniversalDelegates;
using Spaghet.Compiler.Ops;
using TMPro;

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
		string lastCreatureSelected;
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
				CreatureBoardAsset owner = OwnerCreature;
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
			CreatureBoardAsset owner = OwnerCreature;
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
			Talespire.Log.Indent();
			try
			{
				if (editing)
					return;

				if (deltaMove.x == 0 && deltaMove.y == 0 && deltaMove.z == 0)
				{
					Talespire.Log.Warning($"deltaMove is zero.");
					return;  // No movement.
				}

				if (Data.Movement == GroupMovementMode.FollowTheLeader)
					FollowTheLeader(deltaMove);
				else
					StayInFormation(deltaMove);
			}
			finally
			{
				Talespire.Log.Unindent();
			}
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
				MoveMembersByProximity(destinationMembers, sourceMemberLocations, member.Position /* was targetPosition */); 
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
			if (lastMemberLocations != null)
				return lastMemberLocations;
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
			if (OwnerCreature == null)
				return;

			float rotationDegrees = OwnerCreature.GetRotationDegrees();
			foreach (string memberId in Data.Members)
			{
				Talespire.Minis.MoveRelative(memberId, deltaMove);
				if (Data.Look == LookTowardMode.Movement)
					Talespire.Minis.RotateTo(memberId, rotationDegrees);
			}

			UpdateLook();

			ClearLastMemberLocationCache();
		}

		private List<CreatureBoardAsset> GetMemberAssets()
		{
			return Data.Members.Select(id => Talespire.Minis.GetCreatureBoardAsset(id)).Where(z => z != null).ToList();
		}

		void RemoveMember(CreatureBoardAsset creatureBoardAsset)
		{
			string id = creatureBoardAsset.CreatureId.ToString();
			Data.Members.Remove(creatureBoardAsset.CreatureId.ToString());
			Talespire.Minis.IndicatorRingChangeColor(id, Color.black);
			if (Data.BaseIndex > 0)
				Talespire.Minis.SetBaseColorWithIndex(id, 0);
			UpdateMiniColorsSoon();
		}

		public void AddMemberToGroup(CreatureBoardAsset member)
		{
			Data.Members.Add(member.CreatureId.ToString());
		}

		void AddMember(CreatureBoardAsset member)
		{
			AddMemberToGroup(member);
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
			{
				Talespire.Minis.IndicatorRingChangeColor(memberId, IndicatorColor);
				if (Data.BaseIndex > 0)
					Talespire.Minis.SetBaseColorWithIndex(memberId, Data.BaseIndex);
			}

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
				if (memberId != lastCreatureSelected)
					Talespire.Minis.IndicatorRingChangeColor(memberId, Color.black);
			lastCreatureSelected = null;
		}

		void CheckAltitudeAgainSoon()
		{
			checkMiniAltitudeTimer.Start();
		}

		private void BoardToolManager_OnSwitchTool(BoardTool obj)
		{
			Talespire.Log.Indent();
			try
			{
				if (obj is CreatureMoveBoardTool || obj is CreatureKeyMoveBoardTool)
				{
					if (Data.Movement == GroupMovementMode.FollowTheLeader)
					{
						CreatureBoardAsset selectedMini = Talespire.Minis.GetSelected();
						if (selectedMini == OwnerCreature)
						{
							Talespire.Log.Debug($"Owner selected and moving!");
						}
						else
							Talespire.Log.Debug($"Owner NOT selected!");
					}
					moveToolActive = true;
					// Something is moving!
					Talespire.Log.Debug($"// Something is moving!");
					return;
				}

				if (!(obj is DefaultBoardTool))
					return;

				if (OwnerCreature != null)
				{
					Talespire.Log.Debug($"OwnerCreature != null");
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
					else
						Talespire.Log.Debug($"moveTool is not Active");
				}

				CreatureBoardAsset selected = Talespire.Minis.GetSelected();

				if (selectingLookTarget && selected != null)
				{
					selectingLookTarget = false;
					if (OwnerCreature != null)
						OwnerCreature.Creature.Speak($"Now looking at {selected.GetOnlyCreatureName()}.");
					Data.Target = selected.CreatureId.ToString();
					DataChanged();
				}

				if (selected?.Creature.CreatureId.ToString() != OwnerID)
				{
					if (selected == null)
						lastCreatureSelected = null;
					else
					{
						lastCreatureSelected = selected.CreatureId.ToString();

						if (Data.Look == LookTowardMode.NearestOutsider)
							LookToNearestOutsiders(GetMemberAssets());
						else if (Data.Look == LookTowardMode.NearestMember)
						{
							List<CreatureBoardAsset> assets = GetMemberAssets();
							if (assets.Contains(selected)) // We just moved a member...
								FaceClosest(assets, assets);
						}
						else if (Data.Look == LookTowardMode.Creature && lastCreatureSelected == Data.Target)
						{
							List<CreatureBoardAsset> assets = GetMemberAssets();
							foreach (CreatureBoardAsset asset in assets)
								asset.RotateToFacePosition(selected.PlacedPosition);
						}
					}

					if (needToClearGroupIndicators)
						ClearGroupIndicators();
				}

				moveToolActive = false;
			}
			finally
			{
				Talespire.Log.Unindent();
			}
		}

		private void CheckMiniAltitudeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			checkMiniAltitudeTimer.Stop();

			if (OwnerCreature == null)
				return;

			float newAltitude = OwnerCreature.GetFlyingAltitude();

			if (newAltitude != lastAltitude)
			{
				//Talespire.Log.Warning($"Altitude changed from {lastAltitude} to {newAltitude}!");
				Talespire.Log.Warning($"CheckMiniAltitudeTimer_Elapsed / lastGroupPosition = OwnerCreature.PlacedPosition;  <-----------------------------");
				lastGroupPosition = OwnerCreature.PlacedPosition;
			}

			lastAltitude = newAltitude;
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
			CreatureBoardAsset asset = OwnerCreature;
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
			CreatureBoardAsset leaderAsset = OwnerCreature;
			if (Guard.IsNull(leaderAsset, "leaderAsset")) return;
			memberLocationsBeforeRotation = GetMemberLocations(leaderAsset);
		}

		public void RotateFormation(float degrees)
		{
			if (Guard.IsNull(memberLocationsBeforeRotation, "memberLocationsBeforeRotation")) return;

			// TODO: This 180 may be needed because of "centerPoint - memberLocation.Position", below. Reverse this and we may be able to remove the 180.
			float degreesToRotate = degrees - degreesOffsetForRotation + 180;
			CreatureBoardAsset leaderAsset = OwnerCreature;
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

		void CacheLastMemberLocations(List<MemberLocation> memberLocations, CreatureBoardAsset leaderAsset)
		{
			UpdateMemberLocations(memberLocations, leaderAsset);
			lastMemberLocations = memberLocations;
		}

		public void ArrangeInCircle(float radiusFeet, float arcAngleDegrees, float spacingFeet, float rotationOffsetDegrees)
		{
			CreatureBoardAsset leaderAsset = OwnerCreature;
			if (Guard.IsNull(leaderAsset, "leaderAsset")) return;

			List<MemberLocation> memberLocations = GetMemberLocations(leaderAsset);
			if (memberLocations == null || memberLocations.Count == 0)
				return;

			float baseDiameterFeet = GetLargestBaseDiameterFeet(memberLocations);

			int minisToPlaceTotal = memberLocations.Count;
			GetCircleFormationVariables(radiusFeet, arcAngleDegrees, spacingFeet, baseDiameterFeet, minisToPlaceTotal, out Vector3 startingPoint, out float circumferenceFeet, out int numMinisCanFitInThisCircle, out float angleBetweenMinis, out float degreesToRotate);
			List<MemberLocation> allMembers = new List<MemberLocation>(memberLocations);

			int minisToPlaceThisCircle = Math.Min(allMembers.Count, numMinisCanFitInThisCircle);

			while (allMembers.Count > 0)
			{
				Vector3 newOffset = Quaternion.Euler(0, rotationOffsetDegrees + degreesToRotate, 0) * startingPoint; // rotate it
				Vector3 newPosition = leaderAsset.PlacedPosition + newOffset; // calculate rotated point

				MemberLocation member = RemoveMemberClosestTo(allMembers, newPosition);

				Vector3 delta = newPosition - member.Asset.PlacedPosition;
				Talespire.Minis.MoveRelative(member.Asset.CreatureId.ToString(), delta);
				minisToPlaceTotal--;
				minisToPlaceThisCircle--;

				degreesToRotate += angleBetweenMinis;
				if (minisToPlaceThisCircle <= 0 && allMembers.Count > 0)
				{
					radiusFeet += baseDiameterFeet + spacingFeet;
					GetCircleFormationVariables(radiusFeet, arcAngleDegrees, spacingFeet, baseDiameterFeet, minisToPlaceTotal, out startingPoint, out circumferenceFeet, out numMinisCanFitInThisCircle, out angleBetweenMinis, out degreesToRotate);
					minisToPlaceThisCircle = Math.Min(allMembers.Count, numMinisCanFitInThisCircle);
				}
			}
			UpdateLook();
		}


		private static void GetCircleFormationVariables(float radiusFeet, float arcAngleDegrees, float spacingFeet, float baseDiameterFeet, int minisToPlace, out Vector3 startingPoint, out float perimeterFeet, out int numMinisCanFitInThisCircle, out float angleBetweenMinis, out float degreesToRotate)
		{
			//Talespire.Log.Debug($"radiusFeet: {radiusFeet}ft");
			startingPoint = new Vector3(-Talespire.Convert.FeetToTiles(radiusFeet), 0, 0);
			perimeterFeet = (float)Math.PI * (radiusFeet * 2) * arcAngleDegrees / 360f;
			numMinisCanFitInThisCircle = (int)Math.Floor(perimeterFeet / (spacingFeet + baseDiameterFeet));
			if (numMinisCanFitInThisCircle < 1)
				numMinisCanFitInThisCircle = 1;

			if (minisToPlace < numMinisCanFitInThisCircle)
				numMinisCanFitInThisCircle = minisToPlace;

			float denominator = numMinisCanFitInThisCircle;
			if (arcAngleDegrees < 360 && numMinisCanFitInThisCircle > 1)
				denominator = numMinisCanFitInThisCircle - 1;  // For example, I can place five in the space of four (because the last one doesn't overlap the first in this row.

			angleBetweenMinis = arcAngleDegrees / denominator;
			degreesToRotate = 0;

			//Talespire.Log.Vector("startingPoint", startingPoint);
			//Talespire.Log.Debug($"perimeterFeet: {perimeterFeet}ft");
			//Talespire.Log.Debug($"numMinisCanFitInThisCircle: {numMinisCanFitInThisCircle}");
			//Talespire.Log.Debug($"angleBetweenMinis: {angleBetweenMinis}");
			//Talespire.Log.Debug($"degreesToRotate: {degreesToRotate}");
		}

		public void ArrangeInRectangle(int columns, int spacingFeet, float percentVariance, int rotationDegrees)
		{
			CreatureBoardAsset leaderAsset = OwnerCreature;
			if (Guard.IsNull(leaderAsset, "leaderAsset")) return;

			List<MemberLocation> memberLocations = GetMemberLocations(leaderAsset);
			if (memberLocations == null || memberLocations.Count == 0)
				return;
			float baseDiameterFeet = GetLargestBaseDiameterFeet(memberLocations);

			float startingX = memberLocations[0].Position.x;
			float startingZ = memberLocations[0].Position.z;
			int rowIndex = 0;
			int columnIndex = 0;

			Vector3 centerPoint = leaderAsset.PlacedPosition;
			int minisInLastRow = memberLocations.Count % columns;
			int lastRowIndex = (int)Math.Floor((double)memberLocations.Count / columns);
			float normalRowLength = (columns - 1) * GetSpaceTiles(spacingFeet, baseDiameterFeet);
			float lengthLastRow = (minisInLastRow - 1) * GetSpaceTiles(spacingFeet, baseDiameterFeet);

			List<MemberLocation> allMembers = new List<MemberLocation>(memberLocations);
			while (allMembers.Count > 0)
			{
				float x;

				float gaggleFactor = 1;
				if (rowIndex == 0 && columnIndex == 0)
					gaggleFactor = 0;
				if (rowIndex == lastRowIndex)
				{
					if (minisInLastRow == 1)
						x = startingX + normalRowLength / 2f + gaggleFactor * GetGaggleVariance(spacingFeet, percentVariance);
					else
					{
						float lastRowStretchFactor = normalRowLength / lengthLastRow;
						x = startingX + columnIndex * GetSpaceTiles(spacingFeet, baseDiameterFeet) * lastRowStretchFactor + gaggleFactor * GetGaggleVariance(spacingFeet, percentVariance);
					}
				}
				else
					x = startingX + columnIndex * GetSpaceTiles(spacingFeet, baseDiameterFeet) + gaggleFactor * GetGaggleVariance(spacingFeet, percentVariance);

				float z = startingZ + rowIndex * GetSpaceTiles(spacingFeet, baseDiameterFeet) + gaggleFactor * GetGaggleVariance(spacingFeet, percentVariance);
				Vector3 newPosition = new Vector3(x, leaderAsset.PlacedPosition.y, z);

				if (rotationDegrees != 0)
				{
					Vector3 offset = centerPoint - newPosition;
					Vector3 newOffset = Quaternion.Euler(0, rotationDegrees + 180, 0) * offset; // rotate it
					newPosition = centerPoint + newOffset; // calculate rotated point
				}

				MemberLocation member = RemoveMemberClosestTo(allMembers, newPosition);
				Vector3 delta = newPosition - member.Position;
				Talespire.Minis.MoveRelative(member.Asset.CreatureId.ToString(), delta);
				columnIndex++;
				if (columnIndex >= columns)
				{
					columnIndex = 0;
					rowIndex++;
				}
			}

			CacheLastMemberLocations(memberLocations, leaderAsset);
			UpdateLook();
		}

		private static float GetLargestBaseDiameterFeet(List<MemberLocation> memberLocations)
		{
			float largestBaseDiameterFeet = 0;
			foreach (MemberLocation memberLocation in memberLocations)
			{
				float baseDiameterFeet = memberLocation.Asset.GetBaseDiameterFeet();
				if (baseDiameterFeet > largestBaseDiameterFeet)
					largestBaseDiameterFeet = baseDiameterFeet;
			}

			if (largestBaseDiameterFeet == 0)
			{
				Talespire.Log.Error($"Largest Base Diameter was 0ft???!!");
				largestBaseDiameterFeet = 5;
			}

			return largestBaseDiameterFeet;
		}

		private static float GetSpaceTiles(int spacingFeet, float baseDiameter)
		{
			return Talespire.Convert.FeetToTiles(baseDiameter + spacingFeet);
		}

		private static float GetGaggleVariance(int spacingFeet, float percentVariance)
		{
			float spaceBetweenMinisFeet;
			if (percentVariance == 0 || spacingFeet == 0)
				return 0;
			else
				spaceBetweenMinisFeet = UnityEngine.Random.Range(spacingFeet * -percentVariance, spacingFeet * percentVariance);
			return Talespire.Convert.FeetToTiles(spaceBetweenMinisFeet);
		}

		void PositionMember(MemberLocation memberLocation, Vector3 referencePosition, Vector3 totalDeltaMove, ref float offset)
		{
			Vector3 delta = totalDeltaMove * offset / totalDeltaMove.magnitude;
			Vector3 newPosition = referencePosition - delta;
			if (Data.Look == LookTowardMode.Movement)
				memberLocation.Asset.RotateToFacePosition(newPosition);
			memberLocation.Asset.MoveCreatureTo(newPosition);
			offset += Talespire.Convert.FeetToTiles(memberLocation.Asset.GetBaseRadiusFeet() + Data.Spacing);
		}

		void MoveMembersToPreviousMemberPosition(List<MemberLocation> memberLocations, int startIndex)
		{
			int indexHead = 0;
			for (int i = startIndex; i < memberLocations.Count; i++)
			{
				Vector3 newPosition = memberLocations[indexHead].Position;
				if (Data.Look == LookTowardMode.Movement)
					memberLocations[i].Asset.RotateToFacePosition(newPosition);
				memberLocations[i].Asset.MoveCreatureTo(newPosition);
				indexHead++;
			}
		}

		Vector3 previousDeltaMove = Vector3.zero;
		List<MemberLocation> followTheLeaderMemberLocations;
		List<MemberLocation> lastMemberLocations;
		bool selectingLookTarget;

		public void RefreshFollowTheLeaderLine()
		{
			CreatureBoardAsset leaderAsset = OwnerCreature;
			if (Guard.IsNull(leaderAsset, "leaderAsset")) return;
			List<MemberLocation> memberLocations = GetFollowTheLeaderMemberLocations(Vector3.zero, leaderAsset);

			if (Guard.IsNull(followTheLeaderMemberLocations, "followTheLeaderMemberLocations")) return;

			LayoutFollowTheLeaderLine(leaderAsset, memberLocations);
		}

		public void ReverseFollowTheLeaderLine()
		{
			CreatureBoardAsset leaderAsset = OwnerCreature;
			if (Guard.IsNull(leaderAsset, "leaderAsset")) return;
			if (Guard.IsNull(followTheLeaderMemberLocations, "followTheLeaderMemberLocations")) return;

			List<MemberLocation> followTheLeaderMemberLocationsPlusLeader = new List<MemberLocation>(followTheLeaderMemberLocations);
			followTheLeaderMemberLocationsPlusLeader.Add(new MemberLocation() { Asset = leaderAsset, DistanceToLeader = 0, Name = "Leader", Position = leaderAsset.PlacedPosition });
			LayoutFollowTheLeaderLine(leaderAsset, followTheLeaderMemberLocationsPlusLeader);

			followTheLeaderMemberLocations.Reverse();
			List<MemberLocation> saveFollowTheLeaderMemberLocations = followTheLeaderMemberLocations;
			ClearLeaderMovementCache();
			followTheLeaderMemberLocations = saveFollowTheLeaderMemberLocations;


			UpdateMemberLocations(followTheLeaderMemberLocations, leaderAsset);

			if (Data.Look == LookTowardMode.Movement)
				foreach (MemberLocation member in followTheLeaderMemberLocations)
					member.Asset.Rotator.Rotate(Vector3.forward, 180);
		}

		void UpdateMemberLocations(List<MemberLocation> memberLocations, CreatureBoardAsset leaderAsset)
		{
			foreach (MemberLocation memberLocation in memberLocations)
			{
				memberLocation.DistanceToLeader = (leaderAsset.PlacedPosition - memberLocation.Asset.PlacedPosition).magnitude;
				memberLocation.Position = memberLocation.Asset.PlacedPosition;
			}
		}

		void FollowTheLeader(Vector3 deltaMove)
		{
			Talespire.Log.Indent();
			try
			{
				if ((previousDeltaMove + deltaMove).magnitude < Talespire.Convert.FeetToTiles(2.5f))
				{
					Talespire.Log.Warning($"Hey, we are only saving the deltaMove");
					Talespire.Log.Vector("deltaMove", deltaMove);
					previousDeltaMove += deltaMove;
					return;
				}

				deltaMove += previousDeltaMove;
				previousDeltaMove = Vector3.zero;

				CreatureBoardAsset leaderAsset = OwnerCreature;
				if (Guard.IsNull(leaderAsset, "leaderAsset"))
				{
					Talespire.Log.Error($"leaderAsset is null!");
					return;
				}

				Data.AddFollowTheLeaderPoint(deltaMove);
				DataChanged();

				List<MemberLocation> memberLocations = GetFollowTheLeaderMemberLocations(deltaMove, leaderAsset);

				LayoutFollowTheLeaderLine(leaderAsset, memberLocations);

				// memberLocations now has the members in the order they need to be positioned.

				/* 
					We clean up any saved lines we don't need anymore.
				*/
			}
			finally
			{
				Talespire.Log.Unindent();
			}
		}

		private static void ShowSortedMemberLocations(List<MemberLocation> memberLocations)
		{
			Talespire.Log.Debug($"Sorted member locations:");
			foreach (MemberLocation memberLocation in memberLocations)
				Talespire.Log.Debug($"  {memberLocation.DistanceToLeader * 5}ft - {memberLocation.Name}");
			Talespire.Log.Debug($"");
		}

		public void UpdateLook()
		{
			List<CreatureBoardAsset> assets = GetMemberAssets();
			LookTowards(assets);
		}

		void LookTowards(List<CreatureBoardAsset> assets)
		{
			switch (Data.Look)
			{
				case LookTowardMode.Movement:
					return;  // Already set by movement code.

				case LookTowardMode.Leader:
					LookAt(assets, OwnerCreature);
					break;
				case LookTowardMode.Creature:
					LookAt(assets, Talespire.Minis.GetCreatureBoardAsset(Data.Target));
					break;
				case LookTowardMode.NearestMember:
					FaceClosest(assets, assets);
					break;
				case LookTowardMode.NearestOutsider:
					LookToNearestOutsiders(assets);
					break;
			}
		}

		private void LookAt(List<CreatureBoardAsset> assets, CreatureBoardAsset target)
		{
			if (target != null)
				foreach (CreatureBoardAsset asset in assets)
					asset.RotateToFacePosition(target.PlacedPosition);
		}

		private static void LookToNearestOutsiders(List<CreatureBoardAsset> assets)
		{
			CreatureBoardAsset[] allMinis = Talespire.Minis.GetAll();
			List<CreatureBoardAsset> allOtherMinis = allMinis.FilterList(x => !x.IsPersistentEffect() && !assets.Contains(x));
			FaceClosest(assets, allOtherMinis);
		}

		private static void FaceClosest(List<CreatureBoardAsset> assets, List<CreatureBoardAsset> targets)
		{
			foreach (CreatureBoardAsset asset in assets)
			{
				CreatureBoardAsset closestTarget = asset.GetClosest(targets);
				if (closestTarget != null)
					asset.RotateToFacePosition(closestTarget.PlacedPosition);
			}
		}

		private void LayoutFollowTheLeaderLine(CreatureBoardAsset leaderAsset, List<MemberLocation> memberLocations)
		{
			float offset = Talespire.Convert.FeetToTiles(leaderAsset.GetBaseDiameterFeet());
			Vector3 referencePosition = leaderAsset.PlacedPosition;
			int cacheIndex = Data.NumFollowTheLeaderCacheEntries - 1;
			if (cacheIndex < 0)
			{
				Talespire.Log.Error($"cacheIndex < 0 !!!");
				MoveMembersToPreviousMemberPosition(memberLocations, 0);
				return;
			}

			Vector3 cachedDeltaMove = Data.GetFollowTheLeaderEntryByIndex(cacheIndex);
			bool outOfCachedLocations = false;
			int startIndex = 0;
			for (int i = 0; i < memberLocations.Count; i++)
			{
				if (i >= memberLocations.Count)
				{
					Talespire.Log.Error($"Index {i} exceeds memberLocations.Count ({memberLocations.Count})");
					return;
				}
				Talespire.Log.Debug(memberLocations[i]);
				PositionMember(memberLocations[i], referencePosition, cachedDeltaMove, ref offset);

				NextMember(i);

				if (outOfCachedLocations)
					break;
			}
			
			if (outOfCachedLocations)
				MoveMembersToPreviousMemberPosition(memberLocations, startIndex);

			LookTowards(memberLocations.Select(x => x.Asset).ToList());

			// ... 


			void NextMember(int i)
			{
				if (i < memberLocations.Count - 1)
				{
					// Move down the line the radius of the next member...
					offset += Talespire.Convert.FeetToTiles(memberLocations[i + 1].Asset.GetBaseRadiusFeet());
				}

				while (offset > cachedDeltaMove.magnitude)
				{
					offset -= cachedDeltaMove.magnitude;  // To maintain the spacing.

					// Now we need the next position in the cache!!!
					referencePosition -= cachedDeltaMove;
					cacheIndex--;
					if (cacheIndex < 0)
					{
						outOfCachedLocations = true;
						startIndex = i + 1;
						break;
					}
					cachedDeltaMove = Data.GetFollowTheLeaderEntryByIndex(cacheIndex);
				}
			}
		}

		private List<MemberLocation> GetFollowTheLeaderMemberLocations(Vector3 deltaMove, CreatureBoardAsset leaderAsset)
		{
			if (followTheLeaderMemberLocations == null)
				followTheLeaderMemberLocations = GetMemberLocationsSortedByQueue(leaderAsset.PlacedPosition - deltaMove, GetMemberLocations(leaderAsset));
			return followTheLeaderMemberLocations;
		}

		public void ClearLastMemberLocationCache()
		{
			lastMemberLocations = null;
		}

		public void ClearLeaderMovementCache()
		{
			if (OwnerCreature != null)
			{
				Talespire.Log.Warning($"ClearLeaderMovementCache / lastGroupPosition = OwnerCreature.PlacedPosition;  <-----------------------------");
				lastGroupPosition = OwnerCreature.PlacedPosition;
			}

			Data.ClearFollowTheLeaderCache();
			DataChanged();
			followTheLeaderMemberLocations = null;
			previousDeltaMove = Vector3.zero;
		}

		public void SetLookTarget()
		{
			selectingLookTarget = true;
			if (OwnerCreature != null)
				OwnerCreature.Creature.Speak("Select the creature to look at.");
		}

		public List<string> SpawnMoreMembers(int numCreaturesToSpawn, List<NGuid> boardAssetIds)
		{
			List<string> creatureIds = new List<string>();
			int boardAssetIndex = 0;
			if (boardAssetIds == null || boardAssetIds.Count == 0)
			{
				Talespire.Log.Error($"{nameof(SpawnMoreMembers)} - Must specify at least one boardAssetId.");
				return null;
			}

			CreatureBoardAsset ownerCreature = OwnerCreature;
			if (Guard.IsNull(ownerCreature, "ownerCreature")) return null;

			float radiusFeetFromLeader = 5;
			float degreesToRotate = 0;
			float degreesDelta = 33;
			float feetDelta = 5;
			const float distanceBetweenMinisFeet = 5;
			while (numCreaturesToSpawn > 0)
			{
				NGuid boardAssetId = boardAssetIds[boardAssetIndex];
				boardAssetIndex++;
				if (boardAssetIndex >= boardAssetIds.Count)
					boardAssetIndex = 0;

				degreesToRotate += degreesDelta;
				float distanceFromCenterTiles = Talespire.Convert.FeetToTiles(radiusFeetFromLeader);
				radiusFeetFromLeader += feetDelta;
				float circumferenceFeet = (float)Math.PI * 2f * radiusFeetFromLeader;
				float numMinisWeCanFitAtThisRadius = circumferenceFeet / distanceBetweenMinisFeet;
				feetDelta = distanceBetweenMinisFeet / numMinisWeCanFitAtThisRadius;
				degreesDelta = 360f / numMinisWeCanFitAtThisRadius;
				Vector3 offset = new Vector3(-distanceFromCenterTiles, 0, 0);
				Vector3 rotatedOffset = Quaternion.Euler(0, degreesToRotate, 0) * offset; // rotate it
				Vector3 newPosition = ownerCreature.PlacedPosition + rotatedOffset; // calculate rotated point

				creatureIds.Add(Talespire.Board.InstantiateCreature(boardAssetId.ToString(), newPosition));
				numCreaturesToSpawn--;
			}

			return creatureIds;
		}

	}
}

