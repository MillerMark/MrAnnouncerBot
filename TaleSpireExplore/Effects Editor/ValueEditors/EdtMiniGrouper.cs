﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static UnityEngine.ParticleSystem;
using UnityEngine;
using TaleSpireCore;
using Newtonsoft.Json;
using Bounce.Unmanaged;

namespace TaleSpireExplore
{
	public partial class EdtMiniGrouper : UserControl, IValueEditor, IScriptEditor
	{
		const float GaggleVariance = 0.3f;
		System.Timers.Timer updateMemberListTimer;
		System.Timers.Timer updateGroupNameTimer;
		System.Timers.Timer updateNewlySpawnedCreaturesTimer;
		bool initializing;
		public EdtMiniGrouper()
		{
			initializing = true;
			InitializeComponent();
			Disposed += OnDispose;
			Talespire.Minis.MiniSelected += Talespire_MiniSelected;
			updateMemberListTimer = new System.Timers.Timer();
			updateMemberListTimer.Interval = 500;
			updateMemberListTimer.Elapsed += UpdateMemberListTimer_Elapsed;
			updateGroupNameTimer = new System.Timers.Timer();
			updateGroupNameTimer.Interval = 1500;
			updateGroupNameTimer.Elapsed += UpdateGroupNameTimer_Elapsed;
			updateNewlySpawnedCreaturesTimer = new System.Timers.Timer();
			updateNewlySpawnedCreaturesTimer.Interval = 500;
			updateNewlySpawnedCreaturesTimer.Elapsed += UpdateNewlySpawnedCreaturesTimer_Elapsed;
			initializing = false;
		}

		public void InitializeInstance(MonoBehaviour script)
		{
			MiniGrouperScript = script as MiniGrouper;
			if (MiniGrouperScript != null)
			{
				MiniGrouperScript.StateChanged -= MiniGrouperScript_StateChanged;
				MiniGrouperScript.StateChanged += MiniGrouperScript_StateChanged;
			}
			else
				Talespire.Log.Error($"InitializeInstance - MiniGrouperScript is null!");

			RefreshMemberList();
			RefreshTrackHue();
			changingInternally = true;
			try
			{
				UpdateGroupControls();
				UpdateFormationControls();
				UpdateLookControls();
				UpdateFormationStyleControls();
				if (MiniGrouperScript != null)
				{
					trkSpacing.Value = MiniGrouperScript.Data.Spacing;
					lblSpacingValue.Text = $"{MiniGrouperScript.Data.Spacing}ft";

					trkArcAngle.Value = MiniGrouperScript.Data.ArcAngle;
					lblArcAngleValue.Text = $"{MiniGrouperScript.Data.ArcAngle}°";

					trkColumnsRadius.Maximum = 1000;
					trkColumnsRadius.Value = MiniGrouperScript.Data.ColumnRadius;

					lastColumnCount = MiniGrouperScript.Data.ColumnRadius;
					lastRadius = MiniGrouperScript.Data.ColumnRadius;

					switch (MiniGrouperScript.Data.FormationStyle)
					{
						case FormationStyle.FreeForm:
							EditingNeitherColumnsNorCircular();
							break;
						case FormationStyle.Gaggle:
						case FormationStyle.Rectangle:
							EditingColumns();
							break;
						case FormationStyle.Circle:
							EditingCircular();
							break;
					}
				}
			}
			finally
			{
				changingInternally = false;
			}
			MiniGrouperScript?.RefreshIndicators();
		}

		private void OnDispose(object sender, EventArgs e)
		{
			Talespire.Minis.MiniSelected -= Talespire_MiniSelected;
			if (MiniGrouperScript != null)
				MiniGrouperScript.StateChanged -= MiniGrouperScript_StateChanged;
		}

		void RefreshMemberList()
		{
			Talespire.Log.Indent();
			lstMembers.Items.Clear();

			if (Guard.IsNull(MiniGrouperScript, "Script")) return;

			List<string> notFoundCreatures = new List<string>();
			foreach (string member in MiniGrouperScript.Data.Members)
			{
				CreatureBoardAsset creatureBoardAsset = Talespire.Minis.GetCreatureBoardAsset(member);
				if (creatureBoardAsset != null)
					lstMembers.Items.Add(new GroupMember(creatureBoardAsset));
				else
					notFoundCreatures.Add(member);
			}

			MiniGrouperScript.RemoveMembers(notFoundCreatures);
			Talespire.Log.Unindent();
		}

		private void Talespire_MiniSelected(object sender, CreatureBoardAssetEventArgs ea)
		{
			if (Guard.IsNull(MiniGrouperScript, "Script")) return;
			if (Guard.IsNull(ea?.Mini, "Mini")) return;

			Talespire.Log.Warning($"EdtMiniGrouper / Mini Selected - {ea.Mini.GetOnlyCreatureName()}");

			if (editing)
				ToggleGroupMembership(ea.Mini);
		}

		private void ToggleGroupMembership(CreatureBoardAsset mini)
		{
			if (mini.CreatureId.ToString() != MiniGrouperScript.OwnerID)
			{
				MiniGrouperScript.ToggleMember(mini);
				RefreshMemberList();
			}
		}

		public IValueChangedListener ValueChangedListener { get; set; }
		public MiniGrouper MiniGrouperScript { get; set; }

		public void Initialize(IValueChangedListener valueChangedListener)
		{
			ValueChangedListener = valueChangedListener;
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			//Talespire.Log.Debug($"EdtMiniGrouper.GetPropertyChanger() -- lastSerializedData = \"{LastSerializedData}\"");
			ChangeString result = new ChangeString();
			result.SetValue(LastSerializedData);
			return result;
		}

		public void ValueChanged(object newValue, bool committedChange = true)
		{
			if (ValueChangedListener != null)
				ValueChangedListener.ValueHasChanged(this, newValue, committedChange);
		}

		public Type GetValueType()
		{
			return typeof(MiniGrouper);
		}

		public void SetValue(object newValue)
		{
			//Talespire.Log.Indent($"EdtMiniGrouper.SetValue - \"{newValue}\"");
			//try
			//{

			//}
			//finally
			//{
			//	Talespire.Log.Unindent();
			//}
		}

		public void EditingProperty(string name, string paths)
		{
			// TODO: Change any editing style options in this editor based on name heuristics.
		}

		bool editing;
		public string LastSerializedData { get; set; }
		private void btnEdit_Click(object sender, EventArgs e)
		{
			if (editing)
				StopEditMode();
			else
				StartEditMode();
		}

		private void StopEditMode()
		{
			MiniGrouperScript?.StopEditing();
			btnMatchAltitude.Visible = true;
			PersistentEffectsManager.SuppressPersistentEffectUI = false;
			editing = false;
			lbInstructions.Text = "<< Click to Edit the group.";
			btnEdit.Text = "Edit";
			if (MiniGrouperScript != null)
				MiniGrouperScript.UpdateMemberList();
			btnEdit.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
			btnEdit.ForeColor = System.Drawing.Color.White;
			EnableControlsBasedOnMemberCount();
		}

		bool alreadyShowedDiscoverabilityHint = false;
		private void StartEditMode()
		{
			MiniGrouperScript?.ClearLeaderMovementCache();
			MiniGrouperScript?.StartEditing();
			if (MiniGrouperScript != null && !alreadyShowedDiscoverabilityHint)
			{
				alreadyShowedDiscoverabilityHint = true;
				Talespire.Minis.Speak(MiniGrouperScript.OwnerID, "Click minis to include/exclude. Move me (to position the leader).");
			}

			btnMatchAltitude.Visible = false;
			editing = true;
			PersistentEffectsManager.SuppressPersistentEffectUI = true;  // So we no longer show or hide the PE UI when selecting minis.
			lbInstructions.Text = "Click a mini to add or remove it from the group.";
			btnEdit.Text = "Done";
			btnEdit.BackColor = System.Drawing.Color.DarkRed;
			btnEdit.ForeColor = System.Drawing.Color.White;
		}

		void EnableControlsBasedOnMemberCount()
		{
			EnableMemberControls(lstMembers.Items.Count > 0);
			if (formationEditingMode == FormationEditingMode.Columns)
				SetColumnRange();
		}

		void EnableMemberControls(bool enabled)
		{
			txtNewName.Enabled = enabled;
			lblNewCreatureName.Enabled = enabled;
			tbxSetHp.Enabled = enabled;
			lblOutOf.Enabled = enabled;
			tbxMaxHp.Enabled = enabled;
			tbxDamage.Enabled = enabled;
			lbHP1.Enabled = enabled;
			lbHP2.Enabled = enabled;
			tbxHealth.Enabled = enabled;
		}

		bool changingInternally;

		void UpdateFormationStyleControls()
		{
			changingInternally = true;
			try
			{
				rbCircular.Checked = MiniGrouperScript.Data.FormationStyle == FormationStyle.Circle;
				rbRectangular.Checked = MiniGrouperScript.Data.FormationStyle == FormationStyle.Rectangle;
				rbGaggle.Checked = MiniGrouperScript.Data.FormationStyle == FormationStyle.Gaggle;
				rbFreeform.Checked = MiniGrouperScript.Data.FormationStyle == FormationStyle.FreeForm;
			}
			finally
			{
				changingInternally = false;
			}
		}

		private void UpdateGroupControls()
		{
			tbxGroupName.Text = MiniGrouperScript.GetGroupName();
		}

		private void UpdateFormationControls()
		{
			rbFormation.Checked = MiniGrouperScript.Data.Movement == GroupMovementMode.Formation;
			rbFollowTheLeader.Checked = MiniGrouperScript.Data.Movement == GroupMovementMode.FollowTheLeader;
		}

		private void UpdateLookControls()
		{
			rbLookTowardLeader.Checked = MiniGrouperScript.Data.Look == LookTowardMode.Leader;
			rbLookTowardMovement.Checked = MiniGrouperScript.Data.Look == LookTowardMode.Movement;
			rbLookTowardNearestOutsider.Checked = MiniGrouperScript.Data.Look == LookTowardMode.NearestOutsider;
			rbLookTowardNearestMember.Checked = MiniGrouperScript.Data.Look == LookTowardMode.NearestMember;
			rbLookTowardSpecificCreature.Checked = MiniGrouperScript.Data.Look == LookTowardMode.Creature;
		}

		private void MiniGrouperScript_StateChanged(object sender, object e)
		{
			if (MiniGrouperScript != null)
			{
				LastSerializedData = JsonConvert.SerializeObject(MiniGrouperScript.Data);
				//Talespire.Log.Warning($"MiniGrouperScript.Data = {LastSerializedData}");

				ValueChanged("");
			}
			else
				Talespire.Log.Error($"MiniGrouperScript is null!!!");
		}

		private void RefreshTrackHue()
		{
			if (MiniGrouperScript == null)
			{
				trkHue.Value = 0;
				return;
			}

			byte red = (byte)Math.Round(MiniGrouperScript.IndicatorColor.r * 255.0);
			byte green = (byte)Math.Round(MiniGrouperScript.IndicatorColor.g * 255.0);
			byte blue = (byte)Math.Round(MiniGrouperScript.IndicatorColor.b * 255.0);
			System.Drawing.Color color = System.Drawing.Color.FromArgb(red, green, blue);
			HueSatLight hueSatLight = new HueSatLight(color);
			trkHue.Value = (int)Math.Round(hueSatLight.Hue * 360.0);
		}

		private void trkHue_Scroll(object sender, EventArgs e)
		{
			if (MiniGrouperScript == null)
				return;
			MiniGrouperScript.UpdateRingHue(trkHue.Value);
		}

		private void btnMatchAltitude_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.MatchAltitude();
		}

		private void btnHideAll_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.HideAll();
		}

		private void btnShowAll_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.ShowAll();
		}

		private void trkHue_MouseUp(object sender, MouseEventArgs e)
		{
			MiniGrouperScript.CommitRingHue(trkHue.Value);
		}

		private void lstMembers_SelectedIndexChanged(object sender, EventArgs e)
		{
			GroupMember groupMember = lstMembers.SelectedItem as GroupMember;
			if (groupMember != null)
				lblSelectedCreatureName.Text = $"Create more creatures (like \"{groupMember.Name}\")";
			else
				lblSelectedCreatureName.Text = $"Create more creatures.";
		}

		private void btnHeal_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.Heal(GetInt(tbxHealth.Text));
		}

		private void btnDamage_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.Damage(GetInt(tbxDamage.Text));
		}

		FormationEditingMode formationEditingMode = FormationEditingMode.Columns;
		int lastRadius = 3;
		int lastColumnCount = 1;
		object newlySpawnedCreatureLock = new object();
		List<string> newlySpawnedCreatureIds = new List<string>();

		private void EditingCircular()
		{
			trkArcAngle.Visible = true;
			lblArcAngle.Visible = true;
			lblArcAngleValue.Visible = true;
			SetColumnRadiusTrackbarVisibility(true);
			SetCircularRange();
			KeepValueInBounds();
			formationEditingMode = FormationEditingMode.Radius;
			lblColumnsRadius.Text = "Radius:";
			toolTip1.SetToolTip(trkColumnsRadius, "Changes the radius in circular formations.");
			UpdateColumnRadiusLabel();
		}

		private void KeepValueInBounds()
		{
			if (trkColumnsRadius.Value > trkColumnsRadius.Maximum)
				trkColumnsRadius.Value = trkColumnsRadius.Maximum;
		}

		private void EditingColumns()
		{
			SetColumnRadiusTrackbarVisibility(true);
			formationEditingMode = FormationEditingMode.Columns;
			SetColumnRange();
			lblColumnsRadius.Text = "Columns:";
			toolTip1.SetToolTip(trkColumnsRadius, "Changes the number of columns in rectangular formations.");
			UpdateColumnRadiusLabel();
		}

		private void SetColumnRange()
		{
			trkColumnsRadius.Minimum = 1;
			trkColumnsRadius.Maximum = lstMembers.Items.Count;
			KeepValueInBounds();
		}
		private void SetCircularRange()
		{
			trkColumnsRadius.Minimum = 1;
			trkColumnsRadius.Maximum = 150;
			KeepValueInBounds();
		}

		private void NewFormationSelected()
		{
			ResetRotationToZero();
			MiniGrouperScript?.ClearLastMemberLocationCache();
		}

		private void rbRectangular_CheckedChanged(object sender, EventArgs e)
		{
			if (!rbRectangular.Checked)
				return;

			if (changingInternally)
				return; 
			
			SetFormationStyle(FormationStyle.Rectangle);
			NewFormationSelected();
			EditingColumns();
			ArrangeInRectangle(0);
		}

		private void rbCircular_CheckedChanged(object sender, EventArgs e)
		{
			if (!rbCircular.Checked)
			{
				trkArcAngle.Visible = false;
				lblArcAngle.Visible = false;
				lblArcAngleValue.Visible = false;
				return;
			}

			if (changingInternally)
				return;
			
			SetFormationStyle(FormationStyle.Circle);
			NewFormationSelected();
			EditingCircular();
			ArrangeInCircle();
		}

		private void rbGaggle_CheckedChanged(object sender, EventArgs e)
		{
			if (!rbGaggle.Checked)
				return;

			if (changingInternally)
				return;

			SetFormationStyle(FormationStyle.Gaggle);
			NewFormationSelected();
			EditingColumns();
			ArrangeInRectangle(GaggleVariance);
		}

		private void rbFreeform_CheckedChanged(object sender, EventArgs e)
		{
			if (!rbFreeform.Checked)
				return;

			if (changingInternally)
				return;

			SetFormationStyle(FormationStyle.FreeForm);
			EditingNeitherColumnsNorCircular();
			NewFormationSelected();
		}

		private void EditingNeitherColumnsNorCircular()
		{
			formationEditingMode = FormationEditingMode.None;
			SetColumnRadiusTrackbarVisibility(false);
		}

		private void SetColumnRadiusTrackbarVisibility(bool visible)
		{
			trkColumnsRadius.Visible = visible;
			lblColumnRadiusValue.Visible = visible;
			lblColumnsRadius.Visible = visible;
		}

		private void SetFormationStyle(FormationStyle formationStyle)
		{
			if (formationStyle != FormationStyle.FreeForm && !rbFormation.Checked)
			{
				bool saveChangingInternally = changingInternally;
				changingInternally = true;
				rbFormation.Checked = true;
				changingInternally = saveChangingInternally;
			}

			if (MiniGrouperScript != null)
			{
				MiniGrouperScript.Data.FormationStyle = formationStyle;
				MiniGrouperScript.DataChanged();
			}
		}

		void ArrangeInRectangle(float percentVariance)
		{
			MiniGrouperScript?.ArrangeInRectangle(trkColumnsRadius.Value, trkSpacing.Value, percentVariance, trkFormationRotation.Value);
		}

		void ArrangeInCircle()
		{
			MiniGrouperScript?.ArrangeInCircle(trkColumnsRadius.Value, trkArcAngle.Value, trkSpacing.Value, trkFormationRotation.Value);
		}

		private void trkColumnsRadius_Scroll(object sender, EventArgs e)
		{
			if (formationEditingMode == FormationEditingMode.None)
				return;

			EnsureFormationMode();

			if (formationEditingMode == FormationEditingMode.Radius)
			{
				lastRadius = trkColumnsRadius.Value;
			}
			else
			{
				lastColumnCount = trkColumnsRadius.Value;
				MiniGrouperScript?.ClearLastMemberLocationCache();
			}

			if (MiniGrouperScript != null)
			{
				MiniGrouperScript.Data.ColumnRadius = trkColumnsRadius.Value;
				MiniGrouperScript.DataChanged();
			}

			//ResetRotationToZero();
			UpdateColumnRadiusLabel();
			UpdateFormation();
		}

		private void ResetRotationToZero()
		{
			changingInternally = true;
			try
			{
				trkFormationRotation.Value = 0;
				UpdateRotationValue();
			}
			finally
			{
				changingInternally = false;
			}
		}

		private void UpdateColumnRadiusLabel()
		{
			if (formationEditingMode == FormationEditingMode.Radius)
				lblColumnRadiusValue.Text = $"{lastRadius}ft";
			else
				lblColumnRadiusValue.Text = $"{lastColumnCount}";
		}

		private void trkSpacing_Scroll(object sender, EventArgs e)
		{
			lblSpacingValue.Text = $"{trkSpacing.Value}ft";
			
			if (changingInternally || initializing)
				return;

			if (MiniGrouperScript != null)
				MiniGrouperScript.Data.Spacing = trkSpacing.Value;

			if (rbFormation.Checked)
				UpdateFormation();
			else
				MiniGrouperScript?.RefreshFollowTheLeaderLine();
		}

		private void trkArcAngle_Scroll(object sender, EventArgs e)
		{
			lblArcAngleValue.Text = $"{trkArcAngle.Value}°";

			if (changingInternally || initializing)
				return;

			EnsureFormationMode();

			if (MiniGrouperScript != null)
				MiniGrouperScript.Data.ArcAngle = trkArcAngle.Value;

			if (rbFormation.Checked)
				UpdateFormation();
		}

		private void EnsureFormationMode()
		{
			if (MiniGrouperScript == null)
				return;

			changingInternally = true;
			try
			{
				rbFormation.Checked = true;
			}
			finally
			{
				changingInternally = false;
			}

			if (MiniGrouperScript.Data.Movement == GroupMovementMode.Formation)
				return;

			MiniGrouperScript.Data.Movement = GroupMovementMode.Formation;
			MiniGrouperScript.DataChanged();
		}

		private void trkArcAngle_MouseUp(object sender, MouseEventArgs e)
		{
			MiniGrouperScript?.DataChanged();
		}

		private void UpdateFormation()
		{
			if (rbRectangular.Checked)
				ArrangeInRectangle(0);
			else if (rbCircular.Checked)
				ArrangeInCircle();
			else if (rbGaggle.Checked)
				ArrangeInRectangle(GaggleVariance);
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			GroupMember groupMember = lstMembers.SelectedItem as GroupMember;
			List<NGuid> boardAssetIds = new List<NGuid>();
			if (groupMember != null)
				boardAssetIds.Add(groupMember.CreatureBoardAsset.BoardAssetId);
			else
				foreach (GroupMember member in lstMembers.Items)
					boardAssetIds.Add(member.CreatureBoardAsset.BoardAssetId);

			if (!int.TryParse(tbxNumCreatures.Text, out int numCreaturesToSpawn))
			{
				numCreaturesToSpawn = 1;
				tbxNumCreatures.Text = "1";
			}

			List<string> spawnedCreatureIds = MiniGrouperScript?.SpawnMoreMembers(numCreaturesToSpawn, boardAssetIds);

			lock (newlySpawnedCreatureLock)
				newlySpawnedCreatureIds.AddRange(spawnedCreatureIds);

			UpdateNewCreaturesSoon();
		}

		private void UpdateNewlySpawnedCreaturesTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			updateNewlySpawnedCreaturesTimer.Stop();

			if (MiniGrouperScript == null)
				return;

			List<string> foundCreatureIds = new List<string>();
			List<CreatureBoardAsset> foundCreatureBoardAssets = new List<CreatureBoardAsset>();

			bool readyToInitializePlayers = false;
			lock (newlySpawnedCreatureLock)
			{
				foreach (string newlySpawnedCreatureId in newlySpawnedCreatureIds)
				{
					CreatureBoardAsset creatureBoardAsset = Talespire.Minis.GetCreatureBoardAsset(newlySpawnedCreatureId);
					if (creatureBoardAsset != null)
					{
						foundCreatureIds.Add(newlySpawnedCreatureId);
						foundCreatureBoardAssets.Add(creatureBoardAsset);
					}
				}

				foreach (string foundCreatureId in foundCreatureIds)
					newlySpawnedCreatureIds.Remove(foundCreatureId);

				readyToInitializePlayers = newlySpawnedCreatureIds.Count == 0;
			}

			foreach (CreatureBoardAsset creatureBoardAsset in foundCreatureBoardAssets)
				MiniGrouperScript.AddMemberToGroup(creatureBoardAsset);

			if (readyToInitializePlayers)
			{
				InitializeNewlySpawnedCreatures();
			}
			else
				updateNewlySpawnedCreaturesTimer.Start();  // Try again in a moment.
		}

		void InitializeNewlySpawnedCreatures()
		{
			Talespire.Log.Indent();
			UnityMainThreadDispatcher.ExecuteOnMainThread(() =>
			{
				RefreshMemberList();
				MiniGrouperScript.RefreshIndicators();
				UpdateFormation();
			});

			MiniGrouperScript.DataChanged();
			Talespire.Log.Unindent();
		}

		void UpdateNewCreaturesSoon()
		{
			updateNewlySpawnedCreaturesTimer.Start();
		}

		private void tbxGroupName_TextChanged(object sender, EventArgs e)
		{
			if (changingInternally)
				return;
			updateGroupNameTimer.Stop();
			updateGroupNameTimer.Start();
		}

		int GetInt(string text)
		{
			if (int.TryParse(text, out int result))
				return result;

			return 0;
		}
		private void btnSetHp_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.SetHp(GetInt(tbxSetHp.Text), GetInt(tbxMaxHp.Text));
		}

		private void txtNewName_TextChanged(object sender, EventArgs e)
		{
			btnRenameAll.Enabled = !string.IsNullOrWhiteSpace(txtNewName.Text);
		}

		private void btn_MouseEnter(object sender, EventArgs e)
		{
			if (sender is Button button)
				button.BackColor = System.Drawing.Color.FromArgb(12, 12, 12);
		}

		private void btn_MouseLeave(object sender, EventArgs e)
		{
			if (sender is Button button)
				button.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
		}

		private void btnEdit_MouseEnter(object sender, EventArgs e)
		{
			if (editing)
				return;

			btn_MouseEnter(sender, e);
		}

		private void btnEdit_MouseLeave(object sender, EventArgs e)
		{
			if (editing)
				return;

			btn_MouseLeave(sender, e);
		}

		private void UpdateMemberListTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			updateMemberListTimer.Stop();
			RefreshMemberList();
		}

		private void UpdateGroupNameTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			updateGroupNameTimer.Stop();
			MiniGrouperScript?.SetGroupName(tbxGroupName.Text);
		}

		void RefreshMemberListSoon()
		{
			updateMemberListTimer.Start();
		}

		private void btnRenameAll_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.RenameAll(txtNewName.Text);
			RefreshMemberListSoon();
		}

		void UpdateMovementMode()
		{
			if (changingInternally || MiniGrouperScript == null)
				return;

			if (rbFormation.Checked)
				MiniGrouperScript.Data.Movement = GroupMovementMode.Formation;
			else
				MiniGrouperScript.Data.Movement = GroupMovementMode.FollowTheLeader;
			MiniGrouperScript.DataChanged();
		}

		void UpdateLookSettings()
		{
			if (changingInternally || MiniGrouperScript == null)
				return;

			if (rbLookTowardLeader.Checked)
				MiniGrouperScript.Data.Look = LookTowardMode.Leader;
			else if (rbLookTowardMovement.Checked)
				MiniGrouperScript.Data.Look = LookTowardMode.Movement;
			else if (rbLookTowardNearestOutsider.Checked)
				MiniGrouperScript.Data.Look = LookTowardMode.NearestOutsider;
			else if (rbLookTowardNearestMember.Checked)
				MiniGrouperScript.Data.Look = LookTowardMode.NearestMember;
			else if (rbLookTowardSpecificCreature.Checked)
				MiniGrouperScript.Data.Look = LookTowardMode.Creature;
			
			MiniGrouperScript.DataChanged();
			MiniGrouperScript.UpdateLook();
		}

		private void rbFormation_CheckedChanged(object sender, EventArgs e)
		{
			if (changingInternally)
				return;

			if (rbFormation.Checked)
			{
				MiniGrouperScript?.ClearLastMemberLocationCache();
				UpdateMovementMode();
			}
		}

		private void rbFollowTheLeader_CheckedChanged(object sender, EventArgs e)
		{
			if (changingInternally)
				return;

			if (rbFollowTheLeader.Checked)
			{
				MiniGrouperScript?.ClearLeaderMovementCache();
				UpdateMovementMode();
			}
		}

		private void rbLookToward_CheckedChanged(object sender, EventArgs e)
		{
			if (changingInternally)
				return;
			UpdateLookSettings();
		}

		private void btnDestroyGroup_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Destroy ALL THE MINIS in the group? Are You Sure?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
			{
				MiniGrouperScript?.DestroyAll();
			}
		}

		private void trkFormationRotation_MouseDown(object sender, MouseEventArgs e)
		{
			Talespire.Log.Warning($"trkFormationRotation.Value = {trkFormationRotation.Value}");
			MiniGrouperScript?.MarkPositionsForLiveRotation(trkFormationRotation.Value);
		}

		private void trkFormationRotation_Scroll(object sender, EventArgs e)
		{
			UpdateRotationValue();
			if (changingInternally)
				return;
			MiniGrouperScript?.RotateFormation(trkFormationRotation.Value);
		}

		private void UpdateRotationValue()
		{
			lblRotationValue.Text = $"{trkFormationRotation.Value}°";
		}

		private void trkSpacing_MouseUp(object sender, MouseEventArgs e)
		{
			MiniGrouperScript?.DataChanged();
		}

		private void btnReverseLine_Click(object sender, EventArgs e)
		{
			MiniGrouperScript?.ReverseFollowTheLeaderLine();
		}

		private void btnLookAt_Click(object sender, EventArgs e)
		{
			if (editing)
				StopEditMode();

			MiniGrouperScript?.SetLookTarget();
		}
	}
}