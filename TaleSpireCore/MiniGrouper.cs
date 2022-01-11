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
			if (deltaMove.x == 0 && deltaMove.y == 0 && deltaMove.z == 0)
				return;  // No movement.

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
				Talespire.Minis.Heal(memberId, hp, Data.AutoKnockdown);
		}

		public void Damage(int hp)
		{
			foreach (string memberId in Data.Members)
				Talespire.Minis.Damage(memberId, hp, Data.AutoKnockdown);
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
	}
}
