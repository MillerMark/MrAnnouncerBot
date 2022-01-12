using System;
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

namespace TaleSpireExplore
{
	public partial class EdtMiniGrouper : UserControl, IValueEditor, IScriptEditor
	{
		const float GaggleVariance = 0.4f;
		System.Timers.Timer updateMemberListTimer;
		System.Timers.Timer updateGroupNameTimer;
		bool initializing;
		public EdtMiniGrouper()
		{
			initializing = true;
			InitializeComponent();
			Disposed += OnDispose;
			Talespire.Minis.MiniSelected += PersistentEffectsManager_MiniSelected;
			updateMemberListTimer = new System.Timers.Timer();
			updateMemberListTimer.Interval = 500;
			updateMemberListTimer.Elapsed += UpdateMemberListTimer_Elapsed;
			updateGroupNameTimer = new System.Timers.Timer();
			updateGroupNameTimer.Interval = 1500;
			updateGroupNameTimer.Elapsed += UpdateGroupNameTimer_Elapsed;
			initializing = false;
		}

		private void OnDispose(object sender, EventArgs e)
		{
			Talespire.Minis.MiniSelected -= PersistentEffectsManager_MiniSelected;
			if (MiniGrouperScript != null)
			{
				Talespire.Log.Warning($"OnDispose / MiniGrouperScript.StateChanged -= MiniGrouperScript_StateChanged;");
				MiniGrouperScript.StateChanged -= MiniGrouperScript_StateChanged;
			}
		}

		void RefreshMemberList()
		{
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
		}

		private void PersistentEffectsManager_MiniSelected(object sender, CreatureBoardAssetEventArgs ea)
		{
			if (!editing)
				return;

			if (Guard.IsNull(MiniGrouperScript, "Script")) return;

			if (ea.Mini.Creature.CreatureId.ToString() == MiniGrouperScript.OwnerID)
				return;

			MiniGrouperScript.ToggleMember(ea.Mini);
			RefreshMemberList();
		}

		public IValueChangedListener ValueChangedListener { get; set; }
		public MiniGrouper MiniGrouperScript { get; set; }

		public void Initialize(IValueChangedListener valueChangedListener)
		{
			ValueChangedListener = valueChangedListener;
		}

		public BasePropertyChanger GetPropertyChanger()
		{
			Talespire.Log.Debug($"EdtMiniGrouper.GetPropertyChanger() -- lastSerializedData = \"{LastSerializedData}\"");
			ChangeString result = new ChangeString();
			result.SetValue(LastSerializedData);
			return result;
		}

		public void ValueChanged(object newValue, bool committedChange = true)
		{
			Talespire.Log.Indent();
			if (ValueChangedListener != null)
				ValueChangedListener.ValueHasChanged(this, newValue, committedChange);
			Talespire.Log.Unindent();
		}

		public Type GetValueType()
		{
			return typeof(MiniGrouper);
		}

		public void SetValue(object newValue)
		{
			Talespire.Log.Indent($"EdtMiniGrouper.SetValue - \"{newValue}\"");
			try
			{

			}
			finally
			{
				Talespire.Log.Unindent();
			}
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

		private void StartEditMode()
		{
			MiniGrouperScript?.ClearLeaderMovementCache();
			MiniGrouperScript?.StartEditing();
			if (MiniGrouperScript != null)
				Talespire.Minis.Speak(MiniGrouperScript.OwnerID, "Click minis to include/exclude. Move me (to position the leader).");
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

		public void InitializeInstance(MonoBehaviour script)
		{
			MiniGrouperScript = script as MiniGrouper;
			if (MiniGrouperScript != null)
			{
				MiniGrouperScript.StateChanged -= MiniGrouperScript_StateChanged;
				Talespire.Log.Warning($"InitializeInstance - MiniGrouperScript.StateChanged += MiniGrouperScript_StateChanged;");
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
				trkSpacing.Value = MiniGrouperScript.Data.Spacing;
				lblSpacingValue.Text = $"{trkSpacing.Value}ft";
			}
			finally
			{
				changingInternally = false;
			}
			MiniGrouperScript?.RefreshIndicators();
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
			rbLookTowardSpecificCreature.Checked = MiniGrouperScript.Data.Look == LookTowardMode.SpecificCreature;
		}

		private void MiniGrouperScript_StateChanged(object sender, object e)
		{
			Talespire.Log.Indent();
			if (MiniGrouperScript != null)
			{
				Talespire.Log.Warning($"MiniGrouperScript.Data.RingHue = {MiniGrouperScript.Data.RingHue}");
				if (sender is MiniGrouper miniGrouper)
				{
					if (miniGrouper.Data?.RingHue != MiniGrouperScript.Data.RingHue)
						Talespire.Log.Error($"miniGrouper.Data?.RingHue != MiniGrouperScript.Data.RingHue");
				}
				LastSerializedData = JsonConvert.SerializeObject(MiniGrouperScript.Data);
				Talespire.Log.Warning($"MiniGrouperScript.Data = {LastSerializedData}");

				ValueChanged("");
			}
			else
				Talespire.Log.Error($"MiniGrouperScript is null!!!");
			Talespire.Log.Unindent();
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
		
		private void EditingCircular()
		{
			trkColumnsRadius.Minimum = 3;
			trkColumnsRadius.Maximum = 150;
			formationEditingMode = FormationEditingMode.Radius;
			SetVisibilityColumnsRadiusTracker(true);
			lblColumnsRadius.Text = "Radius:";
			toolTip1.SetToolTip(trkColumnsRadius, "Changes the radius in circular formations.");
			UpdateColumnRadiusLabel();
		}

		private void EditingColumns()
		{
			formationEditingMode = FormationEditingMode.Columns;
			SetColumnRange();
			SetVisibilityColumnsRadiusTracker(true);
			lblColumnsRadius.Text = "Columns:";
			toolTip1.SetToolTip(trkColumnsRadius, "Changes the number of columns in rectangular formations.");
			UpdateColumnRadiusLabel();
		}

		private void SetColumnRange()
		{
			trkColumnsRadius.Minimum = 1;
			trkColumnsRadius.Maximum = lstMembers.Items.Count;
		}

		private void rbRectangular_CheckedChanged(object sender, EventArgs e)
		{
			ResetRotationToZero();
			EditingColumns();
			ArrangeInRectangle(0);
		}

		private void rbCircular_CheckedChanged(object sender, EventArgs e)
		{
			ResetRotationToZero();
			EditingCircular();
		}

		private void rbSemiCircle_CheckedChanged(object sender, EventArgs e)
		{
			ResetRotationToZero();
			EditingCircular();
		}

		private void rbTriangle_CheckedChanged(object sender, EventArgs e)
		{
			ResetRotationToZero();
			formationEditingMode = FormationEditingMode.None;
			SetVisibilityColumnsRadiusTracker(false);
		}

		void ArrangeInRectangle(float percentVariance)
		{
			MiniGrouperScript?.ArrangeInRectangle(trkColumnsRadius.Value, trkSpacing.Value, percentVariance);
		}

		private void rbGaggle_CheckedChanged(object sender, EventArgs e)
		{
			ResetRotationToZero();
			EditingColumns();
			ArrangeInRectangle(GaggleVariance);
		}

		private void SetVisibilityColumnsRadiusTracker(bool isVisible)
		{
			lblColumnsRadius.Visible = isVisible;
			lblColumnRadiusValue.Visible = isVisible;
			trkColumnsRadius.Visible = isVisible;

		}

		private void trkColumnsRadius_Scroll(object sender, EventArgs e)
		{
			if (formationEditingMode == FormationEditingMode.Radius)
				lastRadius = trkColumnsRadius.Value;
			else
				lastColumnCount = trkColumnsRadius.Value;
			ResetRotationToZero();
			UpdateColumnRadiusLabel();
			UpdateFormation();
		}

		private void ResetRotationToZero()
		{
			changingInternally = true;
			try
			{
				trkFormationRotation.Value = 0;
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

			if (rbFormation.Checked)
				UpdateFormation();
			if (MiniGrouperScript != null)
			{
				MiniGrouperScript.Data.Spacing = trkSpacing.Value;
				MiniGrouperScript.DataChanged();
			}
		}

		private void UpdateFormation()
		{
			if (rbRectangular.Checked)
				ArrangeInRectangle(0);
			else if (rbGaggle.Checked)
				ArrangeInRectangle(GaggleVariance);
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			GroupMember groupMember = lstMembers.SelectedItem as GroupMember;
			// TODO: Create more members.
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
				MiniGrouperScript.Data.Look = LookTowardMode.SpecificCreature;
			
			MiniGrouperScript.DataChanged();
		}
		private void rbFormation_CheckedChanged(object sender, EventArgs e)
		{
			UpdateMovementMode();
		}

		private void rbFollowTheLeader_CheckedChanged(object sender, EventArgs e)
		{
			MiniGrouperScript?.ClearLeaderMovementCache();
			UpdateMovementMode();
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
			lblRotationValue.Text = $"{trkFormationRotation.Value}°";
			if (changingInternally)
				return;
			MiniGrouperScript?.RotateFormation(trkFormationRotation.Value);
		}
	}
}
